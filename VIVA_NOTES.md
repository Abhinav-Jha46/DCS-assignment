# VIVA VOCE PREPARATION GUIDE & DCS INTERVIEW NOTES

## Distributed Print Queue Management System Using .NET Remoting

---

### The 2-Minute Project Pitch (Perfect for Viva Introduction)

> *"Good morning, Examiner. My project is titled **Distributed Print Queue Management System Using .NET Remoting**.*  
>  
> *In enterprise environments, a printer is a non-shareable resource—two users cannot print simultaneously without colliding. My system solves this by building a centralized, distributed print server.*  
>  
> *The system uses **.NET Remoting** over a **TCP Channel**. Desktop clients run a Windows Forms application and connect to the central server via an interface contract called `IRemotePrintManager`. When a student or employee submits a document, the client sends the file bytes over TCP. The server saves the document in secure storage with path-traversal protection, creates a job record, and inserts it into a centralized, thread-safe queue.*  
>  
> *The server implements **Priority Scheduling**: High priority jobs automatically jump ahead of Normal and Low priority jobs. A dedicated **background worker thread** simulates the physical printing page by page, updating progress in real-time. Connected clients can monitor the queue, pause, resume, cancel, or retry jobs, and inspect system telemetry.*  
>  
> *The solution adheres strictly to distributed system principles: location transparency, remote method invocation, thread synchronization, and clean interface decoupling between Client, Server, and Shared contracts."*

---

### Essential Viva Questions & Conceptual Answers

#### 1. What is Distributed Computing?
Distributed computing is a model where multiple autonomous computers or processes communicate over a network by exchanging messages, coordinating their actions to solve a common problem and share resources transparently.

#### 2. Why is this project classified as a Distributed System?
* **Multiple Autonomous Processes**: The Print Server runs as an independent service; clients run as separate desktop applications on different workstations.
* **No Shared Memory**: Processes communicate strictly through network sockets using TCP packets; they do not share RAM or direct memory pointers.
* **Location Transparency**: The client calls methods on a remote interface without needing to know whether the server code is running locally or across a global network.
* **Concurrency and Resource Sharing**: Multiple clients compete for access to a single simulated printer managed by a central arbiter.

#### 3. What is Client-Server Architecture?
It is a distributed computing model that partitions tasks between resource/service providers (called **servers**) and service requesters (called **clients**). In our project, the Server owns the physical storage, the queue, and the print engine. Clients merely issue requests (Submit, Pause, Cancel) and view results.

#### 4. What is .NET Remoting?
.NET Remoting is Microsoft's legacy, high-performance distributed object framework built into the .NET Framework. It allows an application in one AppDomain or operating system process to invoke methods on an object residing in a different AppDomain or host machine seamlessly.

#### 5. Why use a TCP Channel instead of an HTTP Channel?
* **Performance & Low Overhead**: The TCP channel uses raw binary sockets, avoiding the heavy overhead of HTTP headers and XML/JSON parsing.
* **Binary Serialization**: The `BinaryFormatter` packs C# objects directly into compact binary representations, which is significantly faster and more bandwidth-efficient for file transfers.
* **Stateful Connection**: TCP guarantees ordered, reliable, connection-oriented packet delivery.

#### 6. What is a Remote Object and MarshalByRefObject?
* In .NET, an object that inherits from `System.MarshalByRefObject` communicates across application domain boundaries by using a **Proxy Reference** (message-passing) rather than being copied by value.
* Our `PrintManager` class inherits from `MarshalByRefObject`. When a client calls a method on it, .NET Remoting intercepts the call, packages the parameters, sends them to the server, executes the method on the server, and returns the result.

#### 7. Why do we register PrintManager as a Singleton?
* If registered as `SingleCall`, a new `PrintManager` object would be instantiated for every remote method call and destroyed immediately, losing the queue state!
* By registering as a **Singleton** (`WellKnownObjectMode.Singleton`), all connected clients share the exact same instance and queue in server memory.
* We also override `InitializeLifetimeService()` returning `null` so the remoting lease manager never garbage-collects the singleton.

#### 8. What is a Shared Interface and why is it necessary?
* `IRemotePrintManager` lives in the `DistributedPrintQueue.Shared` library.
* The Client references **only** `Shared`, not the Server implementation.
* This provides **loose coupling** and **security**: the client cannot inspect or tamper with internal server implementation logic, database access, or file system paths.

#### 9. What is a Print Queue and why is it located on the server?
A print queue is a temporary buffer that holds print jobs waiting for printer availability. It must reside on the server because the server is the central authority that coordinates hardware access across all clients. If queues were on clients, clients would have to coordinate among themselves via complex distributed consensus algorithms.

#### 10. How is Concurrency Handled on the Server?
* Multiple clients can submit or modify jobs at the exact same instant on different network threads.
* We use a single synchronization lock object: `private readonly object _queueLock = new object();`.
* All operations that read, write, or reorder the job list are wrapped in `lock (_queueLock) { ... }`.
* This prevents race conditions, dirty reads, and collection modification exceptions.

#### 11. Why is a Background Thread required for the Print Engine?
* If the main thread handled printing, the server would block and freeze during the print delay! New clients would not be able to connect or submit jobs.
* By using a dedicated background worker thread (`PrintQueueProcessor` with `IsBackground = true`), the printer simulation runs concurrently and asynchronously without blocking incoming client requests.

#### 12. How does Priority Scheduling work in your project?
* Each job has a priority: `High (3)`, `Normal (2)`, or `Low (1)`.
* When the printer becomes idle, the processor queries the queue:
  1. Filter jobs where `Status == Queued`.
  2. Order by `Priority` descending (High first).
  3. Order by `SubmittedAt` ascending (earlier submissions first for equal priority).
* A High priority job submitted later will jump ahead of waiting Normal/Low priority jobs.

#### 13. What happens when a Client submits a Print Job? (Step-by-step)
1. User selects a file and sets page count/priority in the Windows Forms UI.
2. Client reads the file bytes into a `byte[]` array.
3. Client calls `_service.SubmitPrintJob(...)` via the remoting proxy.
4. Proxy serializes arguments and sends them over TCP port 9000.
5. Server validates filename, extension, and file size.
6. Server saves bytes to `ServerStorage/PrintJobs/<JobId>_<FileName>`.
7. Server creates a `PrintJob` DTO, adds it to the list under `_queueLock`, recalculates queue positions, and signals the processor thread via `_wakeEvent.Set()`.
8. Server returns a `PrintOperationResult` with success message.
9. Client receives the result and refreshes the DataGridView.

#### 14. What happens when the Print Server goes down?
* In our client, every remote call is wrapped in a `try / catch` block catching `RemotingException` and `SocketException`.
* The client channel is configured with a **3000ms timeout**.
* Instead of crashing or freezing the Windows Forms UI, the client displays a user-friendly error ("Print server is unavailable") and updates the status strip to "Connection interrupted".

#### 15. What Security Validation is implemented?
* **Path Traversal Protection**: Rejects file paths containing `..`, `/`, `\`, or drive colons.
* **Extension Whitelisting**: Only document formats (`.txt`, `.pdf`, `.docx`, `.xlsx`, `.jpg`, `.png`, `.csv`) are accepted. Executables (`.exe`, `.bat`, `.dll`) are strictly blocked.
* **Size Enforcement**: Caps files at 20 MB max.
* **Sink Deserialization Filter**: Server specifies `TypeFilterLevel.Full` to securely control formatter deserialization behavior.

#### 16. Why is there no Database in this project?
* The educational objective of this project is to study **Distributed Systems & .NET Remoting**, not database CRUD operations.
* Using an in-memory synchronized collection paired with server-side flat files keeps the solution lightweight, fast, self-contained, and easy to run in any college lab without configuring SQL Server or MySQL.

#### 17. What is the difference between a Local Method Call and a Remote Method Call?

| Aspect | Local Method Call | Remote Method Call (.NET Remoting) |
| :--- | :--- | :--- |
| **Execution** | Same memory space / call stack | Across network / process boundary |
| **Speed** | Nanoseconds | Milliseconds (network latency) |
| **Parameter Passing** | References / pointers in memory | Deep copy via binary serialization |
| **Failure Modes** | Standard code exceptions | Network dropped, socket timeout, host unreachable |
| **Overhead** | Minimal CPU instructions | Serialization, socket buffering, deserialization |

---

### Quick Cheat-Sheet for Code Locations
* **Where is the remote interface?**  
  `DistributedPrintQueue.Shared/IRemotePrintManager.cs`
* **Where is the server singleton published?**  
  `DistributedPrintQueue.Server/ServerProgram.cs` (`RemotingServices.Marshal` / `App.config`)
* **Where is the queue priority logic?**  
  `DistributedPrintQueue.Server/PrintManager.cs` (`GetNextEligibleJobInternal` and `RecalculateQueuePositionsInternal`)
* **Where does simulated printing run?**  
  `DistributedPrintQueue.Server/PrintQueueProcessor.cs`
* **Where does the client connect to the server?**  
  `DistributedPrintQueue.Client/ClientService.cs` (`Activator.GetObject`)
