# PROJECT REPORT

## DISTRIBUTED PRINT QUEUE MANAGEMENT SYSTEM USING .NET REMOTING

**A Course Project Report submitted in partial fulfillment of the requirements for the degree of Bachelor of Technology in Computer Science and Engineering**

---

### ABSTRACT
In networked enterprise and educational environments, print spooling and document processing represent a quintessential shared-resource contention problem. Without centralized coordination, concurrent client requests result in collision, race conditions, device starvation, and lack of priority arbitration. This project implements a **Distributed Print Queue Management System** using **C#** and **Microsoft .NET Remoting** over a **TCP Channel**. The architecture models a centralized print spooler service that exposes remote interfaces to multiple desktop clients. The server hosts a thread-safe priority queue, a background print processor simulation engine, persistent storage management with path traversal defense, and live administrative telemetry. Client applications built with Windows Forms enable multi-user document submission, real-time progress tracking, and remote job control (pause, resume, cancel, retry). The system demonstrates core distributed computing principles including remote method invocation (RMI), location transparency, concurrency synchronization, and failure isolation.

---

### 1. INTRODUCTION
Distributed computing systems distribute computational tasks and resources across multiple autonomous processing nodes communicating via network messages. One of the classic paradigms in distributed systems is centralized resource arbitration—managing access to a single non-shareable physical or logical peripheral among competing clients.

The Distributed Print Queue Management System provides an extensible, distributed client-server architecture demonstrating how remote method calls, network transport channels, and server-side worker synchronization cooperate to deliver a cohesive enterprise service.

---

### 2. PROBLEM STATEMENT
In an unmanaged peer-to-peer or naive network setup:
1. Multiple workstations attempting to send document byte-streams directly to a single printer result in packet interleaving and corrupted printouts.
2. High-priority documents (e.g. urgent examination papers or executive briefs) cannot bypass voluminous low-priority background drafts.
3. Client workstations remain locked or frozen during the prolonged duration of mechanical printing.
4. There is no centralized auditing, logging, or administrative oversight of printer utilization and failure recovery.

Hence, a dedicated distributed queue management architecture is essential to decouple job submission from mechanical execution while maintaining priority ordering and fault tolerance.

---

### 3. OBJECTIVES
* To design and implement a distributed client-server architecture using **.NET Remoting** and **TCP Sockets**.
* To create a centralized, thread-safe print spooler queue supporting **Priority Scheduling** (High, Normal, Low) and FIFO tie-breaking.
* To develop a deterministic **Simulated Print Engine** modeling page-by-page progress without requiring physical hardware.
* To provide robust **Remote Job Control** enabling clients to pause, resume, cancel, and retry jobs dynamically.
* To guarantee **Security and Path Traversal Protection** for uploaded document payloads.
* To deliver an intuitive Windows Forms client dashboard displaying real-time telemetry and server queue status.

---

### 4. EXISTING SYSTEM VS. PROPOSED SYSTEM

| Feature | Existing Naive System | Proposed Distributed System |
| :--- | :--- | :--- |
| **Communication** | Direct uncoordinated socket or local port | .NET Remoting over TCP Channel with Binary Serialization |
| **Queue Management** | Local workstation spooler (decentralized) | Centralized Server Queue with strict synchronization locks |
| **Priority Handling** | Strictly FIFO (first-come, first-served) | Dynamic Priority Scheduling (High > Normal > Low) |
| **Resource Contention** | Device collision and printer lockup | Centralized spooler protects printer resource |
| **Client Responsiveness**| Client UI freezes during printing | Asynchronous background processing; instant client release |
| **Failure Recovery** | Job dropped on error | Dedicated Retry, Pause, and Error reporting states |
| **Observability** | No central visibility | Comprehensive Server Console & Client Telemetry Dashboards |

---

### 5. SYSTEM ARCHITECTURE

The system is organized into a clean 3-tier distributed architecture:

```text
+-------------------------------------------------------------+
|                        CLIENT TIER                          |
|  - Windows Forms User Interface (MainForm, Dialogs)         |
|  - ClientService (Remoting Proxy Consumer)                  |
|  - UI Polling Timer (1000ms Refresh)                        |
+------------------------------+------------------------------+
                               |
                               | TCP Channel (.NET Remoting)
                               v
+-------------------------------------------------------------+
|                        SHARED TIER                          |
|  - IRemotePrintManager (Remote Interface Contract)          |
|  - PrintJob, PrintOperationResult, ServerStatistics (DTOs)  |
|  - PrintPriority & PrintJobStatus (Enums)                   |
+------------------------------+------------------------------+
                               |
                               | Server Marshaling & Implementation
                               v
+-------------------------------------------------------------+
|                        SERVER TIER                          |
|  - PrintManager (MarshalByRefObject Singleton)              |
|  - PrintQueueProcessor (Dedicated Background Worker Thread) |
|  - StorageManager (ServerStorage/PrintJobs Persistence)     |
|  - ServerLogger (Console & Flat File Logging)               |
+-------------------------------------------------------------+
```

---

### 6. COMMUNICATION FLOW
1. **Channel Setup**:
   * The server initializes `TcpChannel` bound to Port `9000` with `TypeFilterLevel.Full`.
   * The client initializes a client `TcpChannel` with a 3000ms connection timeout.
2. **Object Activation**:
   * The server publishes the singleton instance `PrintManager` via `RemotingServices.Marshal`.
   * The client acquires a transparent proxy via `Activator.GetObject(typeof(IRemotePrintManager), "tcp://localhost:9000/RemotePrintManager")`.
3. **Remote Invocation**:
   * When the client invokes `SubmitPrintJob(...)`, the .NET Remoting runtime serializes arguments into a binary stream, transmits them over TCP, unpacks them on the server, and executes the server method.
   * Return values (`PrintOperationResult`) are serialized and returned to the client proxy seamlessly.

---

### 7. MODULE DESCRIPTION

#### 7.1 DistributedPrintQueue.Shared
* `IRemotePrintManager`: Interface declaring remote methods (`SubmitPrintJob`, `GetAllJobs`, `CancelJob`, `PauseJob`, `ResumeJob`, `RetryJob`, `GetServerStatistics`).
* `PrintJob`: Serializable DTO carrying job metadata across the network boundary.
* `PrintPriority`: Enum specifying `Low = 1`, `Normal = 2`, `High = 3`.
* `PrintJobStatus`: Enum defining lifecycle states (`Queued`, `Printing`, `Paused`, `Completed`, `Cancelled`, `Failed`).

#### 7.2 DistributedPrintQueue.Server
* `PrintManager`: Core singleton inheriting `MarshalByRefObject`. Implements queue concurrency control and overrides `InitializeLifetimeService()` to return `null`.
* `PrintQueueProcessor`: Background worker thread that pulls jobs according to priority, simulates page printing, and updates completion state.
* `StorageManager`: Manages document saving to disk, enforces max upload limits (20MB), and defends against path traversal attacks.
* `ServerLogger`: Provides thread-safe console logging with ANSI colors and append-only file persistence (`ServerStorage/server.log`).

#### 7.3 DistributedPrintQueue.Client
* `ClientService`: Isolates remoting communication, enforces timeouts, and catches `RemotingException` and `SocketException`.
* `MainForm`: Main dashboard featuring a live DataGridView of queue jobs, priority indicators, active job progress bar, and system telemetry metrics.
* `SubmitJobDialog`: Modal dialog enabling users to select document files, adjust page counts, assign priorities, and select target printers.
* `JobDetailsForm`: Deep inspection modal detailing timestamps, file sizes, error notes, and contextual job controls.

---

### 8. FUNCTIONAL REQUIREMENTS
* **FR-01**: System shall allow multiple client instances to connect concurrently to a single server instance.
* **FR-02**: System shall allow clients to submit document files with designated priorities and page counts.
* **FR-03**: System shall store submitted document files in isolated server storage.
* **FR-04**: System shall sort queued jobs such that High priority jobs precede Normal and Low priority jobs.
* **FR-05**: System shall simulate printing with realistic time delays proportional to page count.
* **FR-06**: System shall allow clients to pause actively printing or queued jobs.
* **FR-07**: System shall allow clients to resume paused jobs.
* **FR-08**: System shall allow clients to cancel uncompleted jobs.
* **FR-09**: System shall allow clients to retry failed or cancelled jobs.
* **FR-10**: System shall calculate and transmit real-time statistics (total jobs, pages printed, uptime).

---

### 9. NON-FUNCTIONAL REQUIREMENTS
* **NFR-01 (Performance)**: Client UI must remain responsive at all times without freezing during network calls.
* **NFR-02 (Reliability)**: The server worker thread must continue processing jobs autonomously even if clients disconnect.
* **NFR-03 (Security)**: The server must reject path traversal characters (`..`, `/`, `\`) and unauthorized file extensions.
* **NFR-04 (Maintainability)**: Clear separation of concerns between Shared contracts, Server implementation, and Client presentation.
* **NFR-05 (Compatibility)**: Native compilation using .NET Framework 4.8 on modern Windows OS without requiring third-party package dependencies.

---

### 10. QUEUE SCHEDULING ALGORITHM
The queue implements a non-preemptive priority queue algorithm:
$$\forall j_a, j_b \in \text{QueuedJobs}, \quad j_a \prec j_b \iff (\text{Priority}_a > \text{Priority}_b) \lor (\text{Priority}_a = \text{Priority}_b \land \text{SubmittedAt}_a < \text{SubmittedAt}_b)$$

When a printer becomes idle, the server queries the waiting queue using LINQ:
```csharp
return _jobs
    .Where(j => j.Status == PrintJobStatus.Queued)
    .OrderByDescending(j => (int)j.Priority)
    .ThenBy(j => j.SubmittedAt)
    .FirstOrDefault();
```
This guarantees that urgent documents are processed immediately after the current active document finishes.

---

### 11. SECURITY AND VALIDATION
* **Filename Sanitization**: Only the filename component is preserved; directory paths are discarded via `Path.GetFileName`.
* **Path Traversal Defense**: Submissions containing `..`, `:`, or null bytes are rejected with explicit security warnings.
* **Whitelisted Extensions**: Only `.txt`, `.pdf`, `.docx`, `.xlsx`, `.jpg`, `.png`, and `.csv` are accepted. Executables (`.exe`, `.bat`, `.dll`) are strictly blocked.
* **File Size Caps**: Uploads exceeding 20 MB are rejected before persistence.

---

### 12. TESTING & VERIFICATION SUMMARY
Comprehensive end-to-end verification was conducted across all subsystem interfaces:
* Server startup and TCP port binding: **PASS**
* Multi-client concurrent registration: **PASS**
* Priority scheduling assertion (High preceding Normal/Low): **PASS**
* Dynamic pause and resume state validation: **PASS**
* Cancellation of active and queued jobs: **PASS**
* Retrying cancelled/failed jobs: **PASS**
* Telemetry aggregation and uptime calculation: **PASS**
* Path traversal attack rejection: **PASS**

---

### 13. CONCLUSION
The Distributed Print Queue Management System successfully illustrates the design, construction, and deployment of a reliable distributed system using .NET Remoting. By leveraging remote method invocation, strong interface decoupling, thread synchronization, and priority scheduling, the project solves real-world printing contention problems while offering a clear and demonstrable platform for academic evaluation.
