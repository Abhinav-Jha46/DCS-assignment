# SYSTEM TESTING & VERIFICATION DOCUMENTATION

## Distributed Print Queue Management System (.NET Remoting)

---

### Test Execution Summary
* **Total Test Cases**: 25
* **Passed**: 25
* **Failed**: 0
* **Execution Environment**: Windows 10/11 x64, .NET Framework 4.8, TCP Port 9000

---

### Comprehensive Test Matrix

| Test ID | Description | Input | Expected Result | Actual Result | Status |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **TC-01** | Server Startup & Port Binding | Launch `DistributedPrintQueue.Server.exe` | Server registers TCP channel on port 9000, initializes storage, and awaits requests. | Server console shows "RUNNING" on port 9000 with Singleton marshaled. | **PASS** |
| **TC-02** | Client Connection | Host: `localhost`, Port: `9000`, Client: `Student-PC-01` | Connection succeeds; UI status turns green (● Connected); buttons enabled. | Connected successfully; server console logs client connection. | **PASS** |
| **TC-03** | Invalid Server Connection | Host: `localhost`, Port: `9999` (Offline port) | Friendly error message ("Unable to connect to the print server"); UI does not freeze. | Error dialog displayed within 3 seconds; client UI remains responsive. | **PASS** |
| **TC-04** | Submit Print Job | File: `report.txt`, Pages: 3, Priority: Normal, Client: `Student-PC-01` | Server saves file, creates job, adds to queue, returns success message. | Job submitted; returned ID created; file saved to `ServerStorage/PrintJobs/`. | **PASS** |
| **TC-05** | Multiple Clients Submitting | Client 1 submits `doc1.pdf`; Client 2 submits `doc2.pdf` simultaneously | Both jobs appear in central queue without race conditions. | Both jobs registered in queue with respective client names logged on server. | **PASS** |
| **TC-06** | High Priority Job Ordering | High priority job submitted while Normal and Low priority jobs are queued | High priority job is inserted ahead of Normal and Low jobs in queue order (Position 1). | High priority job assigned Position 1; processed ahead of lower priority jobs. | **PASS** |
| **TC-07** | Normal Priority Job Scheduling | Normal priority job submitted after Low priority job | Normal priority job takes precedence over Low priority job. | Position updated so Normal is ordered ahead of Low priority jobs. | **PASS** |
| **TC-08** | Low Priority Job Scheduling | Low priority job submitted | Job placed at tail of queue unless all other jobs are Low priority. | Position assigned to end of queue behind High and Normal jobs. | **PASS** |
| **TC-09** | Queue Display | Client views queue grid | Shows Position, Job ID, File Name, Client, Pages, Priority, Status, Progress, Time. | DataGridView renders all active and queued jobs with distinct status colors. | **PASS** |
| **TC-10** | Print Progress Tracking | Background worker processes active job (3 pages) | Progress percentage updates proportionally (33%, 66%, 100%) every 2 seconds. | Client ProgressBar and % label update smoothly via polling timer. | **PASS** |
| **TC-11** | Job Completion | Active job finishes all pages | Status transitions to `Completed`, `CompletedAt` timestamp recorded, position becomes 0. | Job marked `Completed`; server increments total pages printed counter. | **PASS** |
| **TC-12** | Pause Job | User selects active printing or queued job and clicks Pause | Status transitions to `Paused`; print simulation halts; progress freezes. | Job status changes to `Paused`; server logs pause event. | **PASS** |
| **TC-13** | Resume Job | User selects paused job and clicks Resume | Status transitions back to `Printing` or `Queued`; simulation continues. | Job resumes smoothly from its last paused page. | **PASS** |
| **TC-14** | Cancel Job | User selects queued or printing job and clicks Cancel | Status transitions to `Cancelled`; job removed from active processing. | Job marked `Cancelled`; worker thread immediately moves to next job. | **PASS** |
| **TC-15** | Retry Failed/Cancelled Job | User selects cancelled/failed job and clicks Retry | Job re-enters queue with status `Queued`, progress reset to 0%, new timestamp. | Job successfully re-queued and processed when printer is available. | **PASS** |
| **TC-16** | Delete Completed Job | User selects completed job and clicks "Delete Finished" | Job removed from server memory list and physical file deleted from storage. | Job removed from queue; associated file deleted from `ServerStorage/PrintJobs/`. | **PASS** |
| **TC-17** | Invalid File Format | User attempts upload of `malicious.exe` | Server rejects submission with error explaining unsupported extension. | Validation error: "Unsupported file extension '.exe'". Job rejected. | **PASS** |
| **TC-18** | Empty File Upload | User submits a 0-byte file | Server rejects submission: "Uploaded file data cannot be empty." | Operation rejected with friendly error dialog; no corrupt job created. | **PASS** |
| **TC-19** | Invalid Page Count | User submits page count 0 or -5 | Validation rejects: "Page count must be between 1 and 1000 pages." | Submission rejected cleanly before queue insertion. | **PASS** |
| **TC-20** | Invalid Filename | Filename with illegal characters `doc*?.txt` | Server detects invalid filesystem characters and rejects submission. | Validation error: "File name contains illegal filesystem characters." | **PASS** |
| **TC-21** | Path Traversal Attempt | Filename: `../../dangerous.txt` or `..\boot.ini` | Server detects path traversal pattern (`..`, `/`, `\`) and blocks request. | Attack blocked: "Invalid file name. Path traversal characters are strictly forbidden." | **PASS** |
| **TC-22** | Server Unavailable / Crash | Print Server process terminated while client is connected | Client displays "Connection interrupted" in status bar without crashing. | WinForms UI remains responsive; attempts to reconnect notify user gracefully. | **PASS** |
| **TC-23** | Remoting Exception Recovery | Network packet drop or serialization error | Handled by try-catch blocks in `ClientService.cs`; returns `PrintOperationResult.Fail`. | Client displays clean message box without uncaught exception crash. | **PASS** |
| **TC-24** | Server Restart | Server restarted after shutdown | Client reconnects by clicking Connect; starts with fresh or re-initialized queue. | Connection re-established cleanly; new requests processed normally. | **PASS** |
| **TC-25** | Server Statistics Audit | Query statistics via server console `[S]` and client dashboard | Displays Total Jobs, Waiting, Printing, Completed, Cancelled, Pages, Clients, Uptime. | All metrics accurately match physical count of jobs in system. | **PASS** |

---

### Automated Verification Proof
The automated end-to-end verification program (`TestHarness.exe`) was executed against the running release server over the live TCP channel:

```text
=== STARTING END-TO-END VERIFICATION SUITE ===
Connecting to remote object at tcp://localhost:9000/RemotePrintManager

[TEST 1] Server Ping / Connectivity Test...
  Ping response: True

[TEST 2] Multi-client Registration...
  Registered Client 1: Student-PC-01 and Client 2: Student-PC-02

[TEST 3] Submitting 3 jobs with Low, Normal, High priorities to test Priority Scheduling...
  Submitted Low: Print job submitted successfully. (JobId: 87929034)
  Submitted Normal: Print job submitted successfully. (JobId: 903f8b85)
  Submitted High: Print job submitted successfully. (JobId: effb5439)

  Current Queue Snapshot:
   - low_doc.txt | Priority: Low | Status: Printing | Pos: 0 | Client: Student-PC-01
   - urgent_report.txt | Priority: High | Status: Queued | Pos: 1 | Client: Student-PC-01
   - normal_doc.txt | Priority: Normal | Status: Queued | Pos: 2 | Client: Student-PC-02
  Checking Queued ordering: Position 1 Priority = High, Position 2 Priority = Normal
  VERIFIED: High priority is properly prioritized in the queue!

[TEST 4] Testing Pause and Resume...
  Pause result for 'low_doc.txt': Print job paused. (Success=True)
  Status after pause: Paused
  Resume result: Print job resumed. (Success=True)
  Status after resume: Queued
  VERIFIED: Pause and Resume work correctly!

[TEST 5] Testing Cancel Job...
  Submitted job to cancel (JobId: 35899d6e)
  Cancel result: Print job cancelled successfully. (Success=True)
  Status after cancel: Cancelled
  VERIFIED: Cancel works correctly!

[TEST 6] Testing Retry Job...
  Retry result: Print job re-queued for processing. (Success=True)
  Status after retry: Queued
  VERIFIED: Retry works correctly!

[TEST 7] Testing Server Telemetry & Statistics...
  Total Jobs: 4
  Waiting (Queued): 4
  Printing: 0
  Completed: 0
  Total Pages Printed: 0
  Connected Clients: 2
  Server Uptime: 00:00:09.5380451
  VERIFIED: Server statistics work correctly!

[TEST 8] Testing Storage Security & Path Traversal Defense...
  VERIFIED: Path traversal attempt correctly rejected: Invalid file name. Path traversal characters are strictly forbidden.

[TEST 9] Observing simulated printing execution...
  [SIMULATION] Completed / Queue cycle tick

=======================================================
 >>> ALL 9 END-TO-END VERIFICATION TESTS PASSED! <<<
=======================================================
```
