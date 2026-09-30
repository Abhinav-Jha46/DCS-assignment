# Distributed Print Queue Management System Using .NET Remoting

> **Academic Course Project**: Distributed Computing Systems (DCS)  
> **Platform & Framework**: C# | .NET Framework 4.8 | .NET Remoting (TCP Channel) | Windows Forms | MSBuild  
> **Development Environment**: Visual Studio 2022 / Windows 10 & 11  

---

## Table of Contents
1. [Project Overview](#1-project-overview)
2. [What the System Does](#2-what-the-system-does)
3. [Why This is a Distributed System](#3-why-this-is-a-distributed-system)
4. [Technologies Used](#4-technologies-used)
5. [Prerequisites and System Requirements](#5-prerequisites-and-system-requirements)
6. [Architecture & Communication Flow](#6-architecture--communication-flow)
7. [Project Solution Structure](#7-project-solution-structure)
8. [Role of .NET Remoting](#8-role-of-net-remoting)
9. [Role of Client Project](#9-role-of-client-project)
10. [Role of Server Project](#10-role-of-server-project)
11. [Role of Shared Project](#11-role-of-shared-project)
12. [Print Queue Concept](#12-print-queue-concept)
13. [Job Lifecycle State Machine](#13-job-lifecycle-state-machine)
14. [Priority Scheduling Algorithm](#14-priority-scheduling-algorithm)
15. [Simulated Print Engine](#15-simulated-print-engine)
16. [File Transfer Mechanism (Client to Server)](#16-file-transfer-mechanism-client-to-server)
17. [Port & Configuration Management](#17-port--configuration-management)
18. [How to Build the Solution](#18-how-to-build-the-solution)
19. [How to Start the Print Server](#19-how-to-start-the-print-server)
20. [How to Start the Client Application](#20-how-to-start-the-client-application)
21. [How to Run Multiple Clients Simultaneously](#21-how-to-run-multiple-clients-simultaneously)
22. [Step-by-Step Viva Demonstration Workflow](#22-step-by-step-viva-demonstration-workflow)
23. [Troubleshooting & Windows Firewall](#23-troubleshooting--windows-firewall)
24. [Design Decisions & Assumptions](#24-design-decisions--assumptions)
25. [Limitations & Future Enhancements](#25-limitations--future-enhancements)

---

## 1. Project Overview
The **Distributed Print Queue Management System** simulates a centralized enterprise network print server. Multiple desktop client workstations submit document print jobs simultaneously across a local network. The print server coordinates incoming jobs into a thread-safe centralized queue, prioritizing urgent documents, persisting file payloads securely, and managing simulated printer hardware resources.

---

## 2. What the System Does
* **Remote Job Submission**: Clients select local documents (`.txt`, `.pdf`, `.docx`, `.xlsx`, `.jpg`, `.png`), set page count, select priority (`Low`, `Normal`, `High`), and choose target virtual printers.
* **Centralized Priority Queueing**: The server places jobs in an in-memory queue. High priority jobs are scheduled ahead of Normal and Low priority jobs.
* **Deterministic Simulation Engine**: Simulates printing page-by-page with configurable time delays (default: 2 seconds per page).
* **Live Job Control**: Connected clients can pause, resume, cancel, retry, or delete their jobs remotely.
* **Real-time Telemetry Dashboard**: Clients and the server console track aggregate metrics: waiting jobs, active printing, completed jobs, cancellations, total printed pages, and uptime.
* **Isolated File Persistence**: Uploaded files are sanitized and stored safely in `ServerStorage/PrintJobs/` with anti-path-traversal protection.

---

## 3. Why This is a Distributed System
A computing system is **distributed** when multiple autonomous computing entities (processes/nodes) communicate over a network by exchanging messages to coordinate actions and share centralized resources.

In this project:
1. **Multiple Distinct Processes**: The Server runs as an autonomous background process. Clients run as independent Windows Forms applications on different workstations or terminals.
2. **No Shared Memory**: The Client and Server do not share RAM or pointers. All communication occurs over **TCP network sockets**.
3. **Location Transparency**: The client interacts with the remote print manager through an interface (`IRemotePrintManager`). The .NET Remoting proxy makes remote method calls look identical to local in-process calls.
4. **Centralized Resource Coordination**: A shared physical/virtual printer cannot service multiple print streams simultaneously. The server acts as the central coordinator arbitrating concurrent access.

---

## 4. Technologies Used
* **Language**: C# (.NET Framework 4.8, C# 5 syntax for maximum native MSBuild compatibility)
* **Remoting Framework**: Microsoft .NET Remoting (`System.Runtime.Remoting`, `System.Runtime.Remoting.Channels.Tcp`)
* **Transport Protocol**: TCP Channel with Binary Formatter Serialization
* **Security Sink**: `TypeFilterLevel.Full` on Binary Server Formatter
* **User Interface**: Windows Forms (`System.Windows.Forms`)
* **Concurrency**: Multithreading (`Thread`, `lock`, `AutoResetEvent`)
* **Build Tools**: Classic MSBuild / Visual Studio 2022 Solution

---

## 5. Prerequisites and System Requirements
* **Operating System**: Windows 10 or Windows 11 (64-bit or 32-bit)
* **Framework**: .NET Framework 4.8 (pre-installed on modern Windows)
* **Build Utility**: MSBuild 4.8 (bundled with Windows in `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\`) or Visual Studio 2022 / 2019
* **Network Port**: TCP Port 9000 (configurable via `App.config`)

---

## 6. Architecture & Communication Flow

### High-Level Topology
```text
+-----------------------+       +-----------------------+
|  Client Instance 1    |       |  Client Instance 2    |
|   (Student-PC-01)     |       |   (Student-PC-02)     |
|  [Windows Forms UI]   |       |  [Windows Forms UI]   |
+-----------+-----------+       +-----------+-----------+
            |                               |
            | TCP .NET Remoting (Port 9000) |
            +---------------+---------------+
                            |
                            v
       +-----------------------------------------+
       |       Centralized Print Server          |
       |  - TcpServerChannel (Port 9000)         |
       |  - MarshalByRefObject (Singleton)       |
       |  - Thread-Safe Job Queue (_queueLock)   |
       |  - PrintQueueProcessor (Worker Thread)  |
       |  - StorageManager (ServerStorage/)      |
       |  - ServerLogger (Console & server.log)  |
       +-----------------------------------------+
```

### Detailed Sequence Diagram
```text
Client                     Remoting Proxy                  Print Server                      Storage / Worker
  |                              |                              |                                   |
  |-- SubmitPrintJob(args) ----->|                              |                                   |
  |                              |-- TCP Binary Payload ------->|                                   |
  |                              |                              |-- ValidateFile()                  |
  |                              |                              |-- SaveJobFile() ----------------->| [Write to Disk]
  |                              |                              |-- lock(_queueLock)                |
  |                              |                              |-- Insert into Queue               |
  |                              |                              |-- Recalculate Positions           |
  |                              |                              |-- _wakeEvent.Set() -------------->| [Awaken Worker]
  |                              |<-- PrintOperationResult -----|                                   |
  |<-- Display Result -----------|                              |                                   |
  |                                                             |                                   |
  |                                                             |                                   |-- While (Queue Has Jobs):
  |                                                             |                                   |   Status = Printing
  |                                                             |                                   |   Simulate Pages (delay)
  |                                                             |                                   |   Status = Completed
  |-- Refresh / Poll Timer ----->|                              |                                   |
  |                              |-- GetAllJobs() ------------->|                                   |
  |<-- Update DataGridView ------|<-- PrintJob[] (Snapshot) ----|                                   |
```

---

## 7. Project Solution Structure
```text
DistributedPrintQueueSystem/
│
├── DistributedPrintQueueSystem.sln         # Master Visual Studio 2022 Solution file
├── build.bat                               # Automated 1-click build script
├── run_demo.bat                            # 1-click launcher: Server + 2 Clients
├── .gitignore                              # Ignores bin/, obj/, temp files
├── README.md                               # Comprehensive project documentation
├── PROJECT_REPORT.md                       # Formal B.Tech Academic Project Report
├── TESTING.md                              # 25 Detailed test cases and audit results
├── VIVA_NOTES.md                           # Viva preparation guide & Q&A
│
├── DistributedPrintQueue.Shared/           # Shared Class Library (.dll)
│   ├── DistributedPrintQueue.Shared.csproj # Non-SDK classic csproj (v4.8)
│   ├── IRemotePrintManager.cs              # .NET Remoting Service Contract
│   ├── PrintJob.cs                         # [Serializable] Print Job DTO
│   ├── PrintJobStatus.cs                   # Lifecycle Enum (Queued..Failed)
│   ├── PrintPriority.cs                    # Priority Enum (Low, Normal, High)
│   ├── PrintOperationResult.cs             # [Serializable] Operation outcome DTO
│   ├── ServerStatistics.cs                 # [Serializable] Telemetry metrics DTO
│   └── Properties/AssemblyInfo.cs
│
├── DistributedPrintQueue.Server/           # Server Console Host (.exe)
│   ├── DistributedPrintQueue.Server.csproj # Non-SDK classic csproj (v4.8)
│   ├── ServerProgram.cs                    # TCP channel setup, admin CLI, lifecycle
│   ├── PrintManager.cs                     # MarshalByRefObject Singleton implementation
│   ├── PrintQueueProcessor.cs              # Background simulation worker thread
│   ├── StorageManager.cs                   # File I/O, path traversal defense
│   ├── ServerLogger.cs                     # Console color logger & server.log
│   ├── App.config                          # Port 9000 & seconds-per-page config
│   ├── Properties/AssemblyInfo.cs
│   └── ServerStorage/                      # Runtime storage
│       └── PrintJobs/                      # Saved uploaded documents
│
└── DistributedPrintQueue.Client/           # Windows Forms Client (.exe)
    ├── DistributedPrintQueue.Client.csproj # Non-SDK classic csproj (v4.8)
    ├── ClientProgram.cs                    # WinForms Application entry point
    ├── ClientService.cs                    # Remoting proxy manager & error handler
    ├── MainForm.cs / .Designer.cs          # Main Dashboard & Queue DataGridView
    ├── SubmitJobDialog.cs / .Designer.cs   # File upload dialog & priority picker
    ├── JobDetailsForm.cs / .Designer.cs    # Detailed telemetry modal & controls
    ├── App.config                          # Client connection configuration
    └── Properties/AssemblyInfo.cs
```

---

## 8. Role of .NET Remoting
.NET Remoting is the communication backbone that allows objects in different application domains and processes to seamlessly invoke methods on each other.

1. **MarshalByRefObject**: `PrintManager` inherits from `System.MarshalByRefObject`. Instead of copying the object to the client, .NET Remoting passes a **Proxy Reference** across the network.
2. **Singleton Activation**: Registered as `WellKnownObjectMode.Singleton`. All connected clients communicate with the exact same server-side instance, ensuring one unified print queue.
3. **Infinite Lifetime**: `PrintManager` overrides `InitializeLifetimeService()` returning `null`, preventing the .NET garbage collector or remoting lease manager from disposing of the remote singleton.
4. **Binary Serialization**: Parameters and returned DTOs (`PrintJob`, `ServerStatistics`) are marked `[Serializable]` and serialized efficiently across the TCP stream.

---

## 9. Role of Client Project
* Exclusively references the **Shared** project (`DistributedPrintQueue.Shared.dll`).
* Never references `DistributedPrintQueue.Server.dll`.
* Obtains a remote proxy using `Activator.GetObject(typeof(IRemotePrintManager), "tcp://localhost:9000/RemotePrintManager")`.
* Configures a 3-second connection timeout to ensure offline servers produce immediate, friendly error messages rather than interface freezing.
* Wraps all remote network invocations in exception handlers (`RemotingException`, `SocketException`).
* Updates Windows Forms UI controls safely via a 1000ms polling timer.

---

## 10. Role of Server Project
* Hosts the TCP listening socket (Port 9000) using `TcpChannel`.
* Configures `TypeFilterLevel.Full` on the `BinaryServerFormatterSinkProvider` to allow full deserialization of complex object graphs.
* Hosts the centralized in-memory print queue protected by a single synchronization lock (`_queueLock`).
* Runs the `PrintQueueProcessor` as a dedicated background thread (`IsBackground = true`).
* Interactively responds to administrator commands: `[Q]` Queue, `[S]` Stats, `[L]` Logs, `[B]` Broadcast, `[C]` Clear Finished, `[X]` Exit.

---

## 11. Role of Shared Project
* Acts as the immutable contract layer between Client and Server.
* Contains the interface `IRemotePrintManager`.
* Contains pure serializable data transfer objects (DTOs) with zero business logic.
* Guarantees zero `MarshalByRefObject` references inside DTOs to avoid secondary network round-trips when reading job properties.

---

## 12. Print Queue Concept
A print queue decouples document submission from mechanical print execution. Without a queue, a slow printer would block submitting client workstations. The centralized server queue:
1. Accepts documents rapidly into disk/memory storage.
2. Immediately releases the client workstation.
3. Organizes waiting documents based on priority and submission time.
4. Feeds documents sequentially to the print engine.

---

## 13. Job Lifecycle State Machine

```text
       [Client Submit]
              |
              v
         +----------+
         |  Queued  |<---------------+
         +----+-----+                |
              |                      |
      [Processor Picks]              |
              |                      |
              v                      | [Retry Job]
        +-----------+                |
   +--->|  Printing |                |
   |    +-----+-----+                |
   |          |                      |
[Resume]  [Simulate]                 |
   |          |                      |
   |          v                      |
   |    +-----------+          +-----+-----+
   +----+  Paused   |          | Cancelled |
        +-----------+          +-----------+
              |                      ^
         [Cancel Job]                |
              +----------------------+
              |                      |
              | [Cancel]             |
              v                      |
        +-----------+                |
        | Completed |          +-----+-----+
        +-----------+          |  Failed   |
                               +-----------+
```

### State Definitions
* **Queued**: Document validated and waiting in queue ordered by priority and timestamp.
* **Printing**: Actively being processed by the simulated print engine.
* **Paused**: Temporarily halted during printing or queueing upon client request.
* **Completed**: Successfully printed 100% of pages.
* **Cancelled**: Aborted before or during printing.
* **Failed**: Processing error occurred (e.g. disk or formatting issue).

---

## 14. Priority Scheduling Algorithm
The server enforces a deterministic **Non-Preemptive Priority Scheduling with Aging/FIFO Tie-Breaker**:

$$\text{Sort Order} = \text{Priority (Descending)} \rightarrow \text{SubmittedAt (Ascending)}$$

* **High (3)**: Urgent documents (rush reports, executive memos).
* **Normal (2)**: Standard departmental print jobs.
* **Low (1)**: Background drafts and large batch prints.

When the print processor finishes a job, it evaluates all jobs currently in `Queued` state. It immediately picks the highest priority job. If two jobs share identical priority, the one submitted earlier is serviced first (FIFO).

---

## 15. Simulated Print Engine
Because physical printers are noisy, slow, and unavailable in computer science laboratories, the system simulates document printing:
* **Configurable Rate**: Configured via `SecondsPerPage` in `App.config` (default: 2 seconds per page).
* **Chunked Slicing**: The worker thread sleeps in 200ms slices, allowing immediate responsiveness if a client pauses or cancels an active job.
* **Page Progress**: Updates `ProgressPercentage` after each page:
  $$\text{Progress} = \left\lfloor \frac{\text{Current Page}}{\text{Total Pages}} \times 100 \right\rfloor$$

---

## 16. File Transfer Mechanism (Client to Server)
1. **File Selection**: User picks a document in the client.
2. **Byte Array Conversion**: The client reads file bytes: `byte[] fileBytes = File.ReadAllBytes(path)`.
3. **Remote Parameter Transport**: The byte array is passed as an argument to `SubmitPrintJob(...)` across the .NET Remoting TCP channel.
4. **Server Disk Isolation**: The server creates a unique file name:
   `ServerStorage/PrintJobs/<JobId>_<SanitizedFileName>`
5. **Path Traversal Defense**: The server verifies that the destination path strictly resides within `ServerStorage/PrintJobs/` and rejects `..`, absolute paths, and illegal characters.

---

## 17. Port & Configuration Management
Both Server and Client configure settings in standard `.NET App.config` files:

### Server `App.config`
```xml
<configuration>
  <appSettings>
    <add key="ServerPort" value="9000" />
    <add key="SecondsPerPage" value="2" />
    <add key="MaxUploadMb" value="20" />
    <add key="StoragePath" value="ServerStorage" />
  </appSettings>
  <system.runtime.remoting>
    <application name="DistributedPrintServer">
      <service>
        <wellknown mode="Singleton"
                   type="DistributedPrintQueue.Server.PrintManager, DistributedPrintQueue.Server"
                   objectUri="RemotePrintManager" />
      </service>
      <channels>
        <channel ref="tcp" port="9000">
          <serverProviders>
            <formatter ref="binary" typeFilterLevel="Full" />
          </serverProviders>
        </channel>
      </channels>
    </application>
  </system.runtime.remoting>
</configuration>
```

---

## 18. How to Build the Solution

### Option A: Using the Automated Build Script (Recommended)
Double-click or run from command prompt:
```cmd
build.bat
```
The script locates MSBuild automatically (checking Visual Studio paths and the built-in Windows .NET 4.8 MSBuild) and builds the entire solution in **Release** configuration.

### Option B: Using Visual Studio 2022
1. Open `DistributedPrintQueueSystem.sln` in Visual Studio 2022.
2. Select **Release** or **Debug** from the Solution Configurations dropdown.
3. Click **Build > Build Solution** (or press `Ctrl+Shift+B`).

---

## 19. How to Start the Print Server
Run the compiled server executable:
```cmd
DistributedPrintQueue.Server\bin\Release\DistributedPrintQueue.Server.exe
```
The console will display:
```text
===============================================================================
             DISTRIBUTED PRINT QUEUE SERVER (.NET REMOTING)                   
===============================================================================
 Storage Directory : C:\DCS\ServerStorage\PrintJobs
 Remoting Endpoint : tcp://localhost:9000/RemotePrintManager
 Print Rate        : 2 sec/page (Simulated Engine)
 Status            : RUNNING (Waiting for client connections)
 Commands: [Q] Queue  [S] Stats  [L] Logs  [B] Broadcast  [C] Clear Completed  [X] Exit
===============================================================================
```

---

## 20. How to Start the Client Application
Run the client executable:
```cmd
DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe
```
1. Verify Server Host is `localhost` (or the IP address of the server machine).
2. Port is `9000`.
3. Set your Client Name (e.g. `Student-PC-01`).
4. Click **Connect**. The status indicator turns green (● Connected).

---

## 21. How to Run Multiple Clients Simultaneously
You can run as many independent client instances as desired on the same computer or across networked lab PCs:

### Option A: Quick Demonstration Script
- **Via Batch**: Double-click or run `run_demo.bat`
- **Via PowerShell**: Run `.\run_demo.ps1`

Both automatically start:
1. The Print Server console.
2. Client 1 initialized as `Student-PC-01`.
3. Client 2 initialized as `Student-PC-02`.

### Option B: Manual Multi-Client Launch (PowerShell or CMD)
**PowerShell:**
```powershell
Start-Process ".\DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe" -ArgumentList "Lab-PC-01"
Start-Process ".\DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe" -ArgumentList "Lab-PC-02"
```

**Command Prompt:**
```cmd
start DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe Lab-PC-01
start DistributedPrintQueue.Client\bin\Release\DistributedPrintQueue.Client.exe Lab-PC-02
```

---

## 22. Step-by-Step Viva Demonstration Workflow

### Step 1: Demonstrate Server Startup & Remoting Setup
1. Launch the server.
2. Point out the TCP listening port (`9000`) and the remoting URI (`RemotePrintManager`).
3. Explain that the server published a **Singleton** `MarshalByRefObject`.

### Step 2: Demonstrate Multi-Client Connection
1. Launch Client 1 (`Student-PC-01`) and Client 2 (`Student-PC-02`).
2. Click **Connect** on both.
3. Observe the Server Console instantly logging:
   `[CONNECT] Client connected and registered: 'Student-PC-01'`
   `[CONNECT] Client connected and registered: 'Student-PC-02'`

### Step 3: Demonstrate Priority Scheduling
1. On Client 1, submit `document_low.txt` (3 pages) with **Low** priority.
2. While `document_low.txt` begins printing, submit `document_normal.txt` (3 pages) with **Normal** priority from Client 2.
3. Immediately submit `urgent_thesis.pdf` (3 pages) with **High** priority from Client 1.
4. **Observe the Queue**:
   * Even though `document_normal.txt` arrived before `urgent_thesis.pdf`, `urgent_thesis.pdf` jumps ahead to **Queue Position 1**!
   * When `document_low.txt` finishes printing, the server immediately begins printing `urgent_thesis.pdf`, demonstrating **Priority Inversion prevention**!

### Step 4: Demonstrate Live Pause, Resume, and Cancel
1. While `urgent_thesis.pdf` is printing, select it and click **Pause**.
2. Notice the status changes to `Paused` and progress freezes on both clients.
3. Click **Resume** to continue printing.
4. On Client 2, select `document_normal.txt` and click **Cancel Job**. The job transitions to `Cancelled` and is skipped by the print processor.

### Step 5: Demonstrate Server Statistics & Admin Commands
1. On the server console, press `[S]` to inspect real-time statistics (total jobs, pages printed, active clients).
2. Press `[Q]` to view the server-side queue table.
3. Press `[L]` to audit recent logs.

---

## 23. Troubleshooting & Windows Firewall
* **Unable to connect to print server**: Ensure the server console is running before clicking Connect on the client. Verify that both applications are configured to use Port 9000.
* **Firewall Warning / Network Connections**: If running across two different physical computers on a local Wi-Fi or LAN, Windows Firewall may block incoming TCP connections on port 9000. To open the port on the server machine, run PowerShell as Administrator:
  ```powershell
  New-NetFirewallRule -DisplayName "DCS Print Server" -Direction Inbound -LocalPort 9000 -Protocol TCP -Action Allow
  ```
* **"Port already in use" Error**: If a previous server process was not closed cleanly, terminate it using:
  ```powershell
  Stop-Process -Name DistributedPrintQueue.Server -Force
  ```

---

## 24. Design Decisions & Assumptions
1. **Classic .NET 4.8 MSBuild Compatibility**: The code utilizes robust C# 5 constructs supported natively by Windows built-in `MSBuild.exe` (`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\`) so any computer can compile the project without requiring external SDK downloads.
2. **Deterministic Simulation**: Document page printing is simulated based on page count (2 sec/page) rather than requiring a physical hardware printer.
3. **No Database Requirement**: In-memory thread-safe collections and flat file logging are utilized to keep the project completely self-contained and easy to evaluate in a college lab environment.

---

## 25. Limitations & Future Enhancements
* **In-Memory Volatility**: If the server process is forcefully terminated, active jobs in memory are cleared (although uploaded files persist in `ServerStorage/PrintJobs/`). Future versions could serialize job state to an XML or JSON persistence store.
* **Authentication**: Currently uses client-supplied computer names for demonstration. Enterprise versions would integrate Windows Active Directory / Kerberos authentication.
* **Multiple Concurrent Physical Printers**: The current simulation models one print engine spooling jobs sequentially. Can be extended to a printer farm with multiple parallel workers.
