namespace Plisky.Diagnostics.Listeners {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handler to write the messages to the file system
    /// </summary>
    public class RollingFileSystemHandler : BaseHandler, IBilgeMessageListener {

        /// <summary>
        /// The currently active filename
        /// </summary>
        protected string activeFilename = null;

        /// <summary>
        /// Determines whether the next write should delete the file.
        /// </summary>
        protected bool nextWriteDeletes = false;

        private long exceptions = 0;
        private Exception exOccured;
        private RollingFSHandlerOptions opt;
        private StringBuilder previousFailedWrite;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingFileSystemHandler"/> class.
        /// </summary>
        /// <param name="sLFHandlerOptions">The options.</param>
        public RollingFileSystemHandler(RollingFSHandlerOptions sLFHandlerOptions) {
            opt = sLFHandlerOptions;
            Formatter = DefaultFormatter(false);
            Name = nameof(RollingFileSystemHandler);
            activeFilename = GetFilenameFromMask(opt.Directory, opt.FileName);
            RefreshActiveFilename();
        }

        /// <summary>
        /// Determines the maximum number of characters stored when a file write failes due to a sharing violation, this limit
        /// decides the amount of memory potentially held onto while waiting for a file to become unlocked.
        /// </summary>
        public int MaxLengthFailedMessageStore { get; set; } = 10240;

        /// <summary>
        /// returns the name of this handler
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the current date time
        /// </summary>
        /// <returns>The current date time</returns>
        public virtual DateTime GetDateTime() {
            return DateTime.Now;
        }

        /// <summary>
        /// retrieves the actual filename from the mask suppied
        /// </summary>
        /// <param name="directory">the directory to use</param>
        /// <param name="fileName">A filename mask</param>
        /// <returns>A filename extracting the mask elements</returns>
        public string GetFilenameFromMask(string directory, string fileName) {
            fileName = fileName.ToLower();

            if (fileName.Contains("%")) {
                var when = GetDateTime();
                fileName = fileName.Replace("%dd", $"{when.Day}").Replace("%mm", $"{when.Month}").Replace("%yy", $"{when.Year}");
                fileName = fileName.Replace("%hh", $"{when.Hour}").Replace("%mi", $"{when.Minute}").Replace("%ss", $"{when.Second}");
                string pid = GetPid();
                fileName = fileName.Replace("%pid", $"{pid}");
            }

            if (string.IsNullOrEmpty(Path.GetExtension(fileName))) {
                fileName += ".log";
            }

            fileName = Path.Combine(directory, fileName);
            string potentialFilename;

            if (fileName.Contains("%nn")) {
                int fileLogNumber = 1;

                potentialFilename = fileName.Replace("%nn", $"{fileLogNumber:00}");
                while (CheckForFilePresence(potentialFilename, opt.FileSizeLimit)) {
                    fileLogNumber++;
                    potentialFilename = fileName.Replace("%nn", $"{fileLogNumber:00}");
                }
                fileName = potentialFilename;
            }

            if (fileName.Contains("%ab")) {
                fileName = GetAbFilename(fileName);
            }

            return fileName;
        }

        /// <summary>
        /// Returns the status of this handler
        /// </summary>
        /// <returns>The status string</returns>
        public string GetStatus() {
            if ((exOccured == null) && (exceptions == 0)) {
                return "OK";
            }
            string lastEx = exOccured == null ? "null" : exOccured.Message;
            return $"Last Exception: {lastEx} total exceptions {exceptions}";
        }

        /// <summary>
        /// async message hanlder
        /// </summary>
        /// <param name="msg">The message</param>
        /// <returns>task for async</returns>
        public virtual async Task HandleMessageAsync(MessageMetadata[] msg) {
            string fname = activeFilename;

            StringBuilder sb;
            if (previousFailedWrite != null && previousFailedWrite.Length < MaxLengthFailedMessageStore) {
                sb = previousFailedWrite;
            } else {
                sb = new StringBuilder();
            }

            for (int i = 0; i < msg.Length; i++) {
                sb.Append(Formatter.Convert(msg[i]));
            }

            if (nextWriteDeletes) {
                nextWriteDeletes = false;
                File.Delete(fname);
            }
            try {
#if NETCOREAPP
                await File.AppendAllTextAsync(fname, sb.ToString());
#else
                File.AppendAllText(fname, sb.ToString());
#endif
                // If we did not trigger an exception then the failed write has
                // been written and we can clear it down.
                previousFailedWrite = null;
                exOccured = null;
            } catch (IOException ex) {
                exceptions++;
                exOccured = ex;
                if (ex.HResult != -2147024864) {
                    // ERROR_SHARING_VIOLATION 0x80070020 - if its a sharing violation 1
                    throw;
                }
                previousFailedWrite = sb;
            }
            RefreshActiveFilename();
        }

        /// <summary>
        /// Initialise the rollingfilesystemhandler from a string
        /// </summary>
        /// <param name="initialisationString">An initialisation string</param>
        /// <returns>An initialisated RollingFileSystemHandler</returns>
        [HandlerInitialisation("RFS")]
        public RollingFileSystemHandler InitiliaseFrom(string initialisationString) {
            var opt = new RollingFSHandlerOptions(initialisationString);
            opt.Parse();

            if (opt.CanCreate) {
                return new RollingFileSystemHandler(opt);
            }
            return null;
        }

        /// <summary>
        /// Determines if a file is present
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <param name="fileSizeLimit">The file size limit</param>
        /// <returns>True if the file is present and the right size</returns>
        protected virtual bool CheckForFilePresence(string fileName, long fileSizeLimit) {
            if (fileSizeLimit < 0) {
                return File.Exists(fileName);
            }

            var fi = new FileInfo(fileName);
            if (fi.Exists && fi.Length > fileSizeLimit) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the process id of the current process
        /// </summary>
        /// <returns>The process id as a string</returns>
        protected virtual string GetPid() {
            return Process.GetCurrentProcess().Id.ToString();
        }

        /// <summary>
        /// Determines if a file is bigger than a given size
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <param name="size">The size to check against</param>
        /// <returns>True if it is bigger than that size</returns>
        protected virtual bool IsFileBiggerThan(string fileName, long size) {
            var fi = new FileInfo(fileName);
            return fi.Length > size;
        }

        private string GetAbFilename(string fileName) {
            string potentialFilenameA = fileName.Replace("%ab", "A");
            string potentialFilenameB = fileName.Replace("%ab", "B");

            bool aExists = CheckForFilePresence(potentialFilenameA, opt.FileSizeLimit);
            bool bExists = CheckForFilePresence(potentialFilenameB, opt.FileSizeLimit);

            if (!aExists) {
                return potentialFilenameA;
            }

            if (aExists && !bExists) {
                return potentialFilenameB;
            }

            if (aExists && bExists) {
                if (File.GetLastWriteTimeUtc(potentialFilenameA) <= File.GetLastWriteTimeUtc(potentialFilenameB)) {
                    return potentialFilenameA;
                } else {
                    return potentialFilenameB;
                }
            }
            throw new InvalidOperationException("Logic fault");
        }

        private string RefreshActiveFilename() {
            if (!File.Exists(activeFilename)) {
                return activeFilename;
            }

            string newFilename = GetFilenameFromMask(opt.Directory, opt.FileName);

            if (newFilename != activeFilename) {
                // Something has changed, date or something.
                if (newFilename != activeFilename) {
                    // Only really applies to the AB log file appender but if the filename
                    // we are about to new is new and exists, delete it.
                    if (File.Exists(newFilename)) {
                        nextWriteDeletes = true;
                    }
                    activeFilename = newFilename;
                }
            }

            return activeFilename;
        }
    }
}