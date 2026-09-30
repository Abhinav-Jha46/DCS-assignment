using System;
using System.IO;
using System.Linq;

namespace DistributedPrintQueue.Server
{
    /// <summary>
    /// Manages file persistence on the print server.
    /// Implements strict path validation and traversal protection.
    /// </summary>
    public class StorageManager
    {
        private readonly string _baseDirectory;
        private readonly string _printJobsDirectory;
        private readonly long _maxFileSizeBytes;

        private static readonly string[] AllowedExtensions = new[]
        {
            ".txt", ".pdf", ".docx", ".xlsx", ".doc", ".xls", ".jpg", ".jpeg", ".png", ".rtf", ".csv"
        };

        public StorageManager(string baseDir = "ServerStorage", long maxFileSizeMb = 20)
        {
            _baseDirectory = Path.GetFullPath(baseDir);
            _printJobsDirectory = Path.Combine(_baseDirectory, "PrintJobs");
            _maxFileSizeBytes = maxFileSizeMb * 1024 * 1024;

            EnsureDirectoriesExist();
            SeedSampleFiles();
        }

        public string PrintJobsDirectory
        {
            get { return _printJobsDirectory; }
        }

        private void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
            if (!Directory.Exists(_printJobsDirectory))
            {
                Directory.CreateDirectory(_printJobsDirectory);
            }
        }

        private void SeedSampleFiles()
        {
            try
            {
                string sampleReport = Path.Combine(_printJobsDirectory, "sample_report.txt");
                if (!File.Exists(sampleReport))
                {
                    File.WriteAllText(sampleReport, "=== DISTRIBUTED COMPUTING SYSTEMS PROJECT REPORT ===\nAuthor: Student\nTopic: .NET Remoting Print Queue Management\nStatus: Initial Demonstration Sample\n");
                }

                string sampleAssignment = Path.Combine(_printJobsDirectory, "sample_assignment.txt");
                if (!File.Exists(sampleAssignment))
                {
                    File.WriteAllText(sampleAssignment, "=== LAB ASSIGNMENT #4 ===\nCourse: Distributed Computing Systems (CS401)\nTask: Remote Method Invocation & Priority Queue Demonstration\n");
                }
            }
            catch (Exception ex)
            {
                ServerLogger.Warn("Failed to seed sample files: " + ex.Message);
            }
        }

        /// <summary>
        /// Validates file name and parameters against security attacks (traversal, invalid characters).
        /// </summary>
        public void ValidateFile(string rawFileName, byte[] fileData)
        {
            if (string.IsNullOrWhiteSpace(rawFileName))
            {
                throw new ArgumentException("File name cannot be empty.");
            }

            // Path traversal checks
            if (rawFileName.Contains("..") || rawFileName.Contains("/") || rawFileName.Contains("\\") || rawFileName.Contains(":") || rawFileName.Contains("\0"))
            {
                throw new ArgumentException("Invalid file name. Path traversal characters are strictly forbidden.");
            }

            // Strip any path segments just in case
            string safeName = Path.GetFileName(rawFileName);
            if (string.IsNullOrWhiteSpace(safeName) || safeName != rawFileName)
            {
                throw new ArgumentException("Invalid file name format.");
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (safeName.IndexOfAny(invalidChars) >= 0)
            {
                throw new ArgumentException("File name contains illegal filesystem characters.");
            }

            string ext = Path.GetExtension(safeName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
            {
                throw new ArgumentException(string.Format("Unsupported file extension '{0}'. Allowed extensions: {1}",
                    ext, string.Join(", ", AllowedExtensions)));
            }

            if (fileData == null || fileData.Length == 0)
            {
                throw new ArgumentException("Uploaded file data cannot be empty.");
            }

            if (fileData.Length > _maxFileSizeBytes)
            {
                throw new ArgumentException(string.Format("File size ({0:N0} bytes) exceeds maximum allowable limit of {1:N0} bytes.",
                    fileData.Length, _maxFileSizeBytes));
            }
        }

        /// <summary>
        /// Saves uploaded document bytes using an isolated job-specific file name.
        /// </summary>
        public string SaveJobFile(Guid jobId, string rawFileName, byte[] fileData)
        {
            ValidateFile(rawFileName, fileData);

            string safeFileName = Path.GetFileName(rawFileName);
            string storedName = string.Format("{0}_{1}", jobId.ToString("N"), safeFileName);
            string fullPath = Path.Combine(_printJobsDirectory, storedName);

            // Double check canonical path resides within _printJobsDirectory
            string canonical = Path.GetFullPath(fullPath);
            if (!canonical.StartsWith(_printJobsDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Security violation: path traversal attempt detected.");
            }

            File.WriteAllBytes(fullPath, fileData);
            return fullPath;
        }

        /// <summary>
        /// Deletes the physical file associated with a job.
        /// </summary>
        public bool DeleteJobFile(Guid jobId, string rawFileName)
        {
            try
            {
                string safeFileName = Path.GetFileName(rawFileName);
                string storedName = string.Format("{0}_{1}", jobId.ToString("N"), safeFileName);
                string fullPath = Path.Combine(_printJobsDirectory, storedName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ServerLogger.Warn(string.Format("Failed to delete file for job {0}: {1}", jobId, ex.Message));
            }
            return false;
        }
    }
}
