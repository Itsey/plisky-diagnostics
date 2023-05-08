namespace Plisky.Diagnostics.Listeners {

    using System;

    /// <summary>
    /// filestream handler options
    /// </summary>
    public class RollingFSHandlerOptions : HandlerOptions {

        /// <summary>
        /// The maximum file size
        /// </summary>
        protected long MaxFilesize = -1;

        private const int MINIMUM_PARTS_FOR_VALIDCONFIG = 1;
        private const string RFSH_IDENTIFIER = "RFS:";
        private string mrfs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingFSHandlerOptions"/> class.
        /// filestream handler options.
        /// </summary>
        public RollingFSHandlerOptions() : base(string.Empty) {
            FileName = "log_%dd%mm%yy_%hh%mm%ss_%nn.log";
            FilenameIsMask = true;
            MaxRollingFileSize = "10mb";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingFSHandlerOptions"/> class.
        /// filestream handler options.
        /// </summary>
        /// <param name="initString">the initialisation data</param>
        public RollingFSHandlerOptions(string initString) : base(initString) {
        }

        /// <summary>
        /// Gets or Sets the directory that should be used for the rolling file system handler to write logs into.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets the filename to use for logging.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or Sets the mask property.  If this true then the filename will support replacements of well known mask characters to incremement numbers or use A/B logging.
        /// </summary>
        public bool FilenameIsMask { get; set; }

        /// <summary>
        /// Gets the filesize limit on the log files.
        /// </summary>
        public long FileSizeLimit { get; private set; }

        /// <summary>
        /// The file size to roll at
        /// </summary>
        public string MaxRollingFileSize {
            get {
                return mrfs;
            }
            set {
                SetFileSizeLimit(value);
            }
        }

        /// <summary>
        /// Parses the configuration in the options
        /// </summary>
        /// <returns>true if the parse succeeded</returns>
        public override bool Parse() {
            CanCreate = true;

            if (InitialisationString.StartsWith(RFSH_IDENTIFIER)) {
                InitialisationString = InitialisationString.Substring(RFSH_IDENTIFIER.Length);
            } else {
                CanCreate = false;
                return CanCreate;
            }

            string[] parts = InitialisationString.Split(',');
            if (parts[0].Contains(":")) {
                // This is catching any other initialisation strings SFL: FIL: etc. None of those are valid for tcp
                // initialisation therefore return null.
                CanCreate = false;
                return CanCreate;
            }

            if (parts.Length < MINIMUM_PARTS_FOR_VALIDCONFIG) {
                CanCreate = false;
                return false;
            }

            if (string.IsNullOrEmpty(parts[0])) {
                CanCreate = false;
            } else {
                FileName = parts[0];
                FilenameIsMask = FileName.IndexOf('%') >= 0;
            }

            if (parts.Length > 1) {
                if (parts[1].Trim() == "none") {
                    MaxRollingFileSize = string.Empty;
                } else {
                    MaxRollingFileSize = parts[1];
                }
            }

            return CanCreate;
        }

        private void SetFileSizeLimit(string value) {
            if (string.IsNullOrEmpty(value)) { throw new ArgumentOutOfRangeException(); }

            string working = value.ToLower();

            mrfs = value;

            long multiplier = 1;
            if (working.Contains("kb")) {
                multiplier = 1024;
                working = working.Replace("kb", string.Empty);
            } else if (working.Contains("mb")) {
                multiplier = 1024 * 1024;
                working = working.Replace("mb", string.Empty);
            } else if (working.Contains("gb")) {
                multiplier = 1024 * 1024 * 1024;
                working = working.Replace("gb", string.Empty);
            }

            long result = long.Parse(working) * multiplier;
            FileSizeLimit = result;
        }
    }
}