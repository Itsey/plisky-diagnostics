using System;
using System.Collections.Generic;
using Plisky.Diagnostics.Listeners;

namespace Plisky.Diagnostics.Test {

    internal class MockRollingFileSystemHandler : RollingFileSystemHandler {
        protected List<string> existingFiles = new List<string>();
        protected DateTime returnedDate = DateTime.Now;
        protected bool shouldReplaceDate = false;
        private string actualPid = string.Empty;

        private bool replacePid = false;

        public MockRollingFileSystemHandler(RollingFSHandlerOptions o) : base(o) {
        }

        public override DateTime GetDateTime() {
            if (shouldReplaceDate) {
                return returnedDate;
            }
            return base.GetDateTime();
        }

        public void SetDate(int dd, int mm, int yy) {
            shouldReplaceDate = true;
            returnedDate = new DateTime(yy, mm, dd, returnedDate.Hour, returnedDate.Minute, returnedDate.Second);
        }

        public void SetTime(int hh, int mm, int ss) {
            shouldReplaceDate = true;
            returnedDate = new DateTime(returnedDate.Year, returnedDate.Month, returnedDate.Day, hh, mm, ss);
        }

        internal void AddExistingFile(string filenameToExist) {
            existingFiles.Add(filenameToExist);
        }

        internal void SetPid(string pid) {
            actualPid = pid;
            replacePid = true;
        }

        protected override bool CheckForFilePresence(string fileName, long size) {
            if (existingFiles.Contains(fileName)) {
                return true;
            }
            return false;
        }

        protected override string GetPid() {
            if (replacePid) {
                return actualPid;
            }
            return base.GetPid();
        }
    }
}