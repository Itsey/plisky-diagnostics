namespace Plisky.Diagnostics.Test {
#if false
    internal class MockRollingFileSystemHandler : RollingFileSystemHandler {
        protected bool shouldReplaceDate = false;
        protected DateTime returnedDate = DateTime.Now;

        public MockRollingFileSystemHandler(RollingFSHandlerOptions o): base(o) {
        }

        public void SetDate(int dd, int mm, int yy) {
            shouldReplaceDate = true;
            returnedDate = new DateTime(yy, mm, dd, returnedDate.Hour, returnedDate.Minute, returnedDate.Second);
        }

        public void SetTime(int hh, int mm, int ss) {
            shouldReplaceDate = true;
            returnedDate = new DateTime(returnedDate.Year, returnedDate.Month, returnedDate.Day,hh,mm,ss);
        }

        public override DateTime GetDateTime() {
            if (shouldReplaceDate) {
                return returnedDate;
            }
            return base.GetDateTime();
        }

        protected List<string> existingFiles = new List<string>();

        internal void AddExistingFile(string filenameToExist) {
            existingFiles.Add(filenameToExist);
        }

        protected override bool CheckForFilePresence(string fileName, long size) {
            if (existingFiles.Contains(fileName)) {
                return true;
            }
            return false;
        }

        private string actualPid = string.Empty;
        private bool replacePid = false;

        internal void SetPid(string pid) {
            actualPid = pid;
            replacePid = true;
        }

        protected override string GetPid() {
            if(replacePid) {
                return actualPid;
            }
            return base.GetPid();
        }
    }
#endif
}