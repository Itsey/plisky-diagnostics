﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Plisky.Diagnostics.Test {

    internal class MockRouter : BilgeRouter {
        public int AssertionMessageCount = 0;

        private List<Tuple<string,string>> ContextMustIncludeList = new List<Tuple<string, string>>();

        public volatile int TotalMessagesRecieved;
        private List<MessageMetadata> allMessagesRecieved = new List<MessageMetadata>();
        private string ContextMustBe = null;

        private MessageMetadata lastMessageData;

        private string MethodNameMustBe = null;

        private List<Func<MessageMetadata, bool>> validators = new List<Func<MessageMetadata, bool>>();

        public MockRouter() : base("mock") {
        }

        public MockRouter(string procId) : base(procId) {
        }

        public string BodyMustContain { get; private set; }

        public string ClassNameMustBe { get; private set; }
        public int LastMessageBatchSize { get; set; }

        public string ManagedThreadIdMustBe { get; private set; }

        public string Name { get; set; }

        public string ProcessIdMustBe { get; private set; }

        public override void ActualClearEverything() {
        }

        public override void ActualReInitialise() {
        }

        public override void ActualShutdown() {
        }

        public void AssertAllConditionsMetForAllMessages(bool assertSomeMessagesRecieved = true, bool allowSingleMatch = false) {
            if (assertSomeMessagesRecieved) {
                Assert.True(TotalMessagesRecieved > 0, "No messages written to the listener");
            }
            lock (allMessagesRecieved) {
                bool validMatchRecieved = false;

                foreach (var v in allMessagesRecieved) {
                    bool tmp = ValidateMessageDataForMock(v, !allowSingleMatch);
                    if (!validMatchRecieved) {
                        validMatchRecieved = tmp;
                    }
                }
                if (allowSingleMatch) {
                    Assert.True(validMatchRecieved, "Not one of the messages matched the conditions");
                }
            }
        }

        public bool AssertThisMessageMustExist(string message) {
            Assert.True(TotalMessagesRecieved > 0, "No messages written to the listener");
            foreach (var v in allMessagesRecieved) {
                if (v.Body.Contains(message)) {
                    return true;
                }
            }
            return false;
        }

        public void SetTimestampMustBe(DateTime? mustBeAfter, DateTime? mustBeBefore) {
            validators.Add(new Func<MessageMetadata, bool>((mmd) => {
                Assert.NotNull(mmd);
                Assert.NotNull(mmd.TimeStamp);
                if (mustBeAfter != null) {
                    Assert.True(mmd.TimeStamp >= mustBeAfter);
                }

                if (mustBeBefore != null) {
                    Assert.True(mmd.TimeStamp <= mustBeBefore);
                }
                return true;
            }));
        }

        public void SetTimeStampMustBeNull() {
            validators.Add(new Func<MessageMetadata, bool>((mmd) => {
                Assert.NotNull(mmd);
                Assert.Null(mmd.TimeStamp);
                return true;
            }));
        }

        internal void AssertContextIs(string v) {
            ContextMustBe = v;
        }

        internal void AssertManagedThreadId(int managedThreadId) {
            ManagedThreadIdMustBe = managedThreadId.ToString();
        }

        internal void AssertProcessId(string testProcId) {
            ProcessIdMustBe = testProcId;
        }

        internal MessageMetadata GetMostRecentMessage() {
            return lastMessageData;
        }

        internal void SetClassnameMustBe(string className) {
            ClassNameMustBe = className;
        }

        internal void SetMethodNameMustContain(string v) {
            MethodNameMustBe = v;
        }

        internal void SetMustContainForBody(string v) {
            BodyMustContain = v;
        }

        protected override void ActualAddMessage(MessageMetadata[] mmd) {
            TotalMessagesRecieved += mmd.Length;
            PrepareMessage(mmd);

            foreach (var m in mmd) {
                if (m.CommandType == TraceCommandTypes.AssertionFailed) {
                    AssertionMessageCount++;
                }
                lock (allMessagesRecieved) {
                    allMessagesRecieved.Add(m);
                }
                lastMessageData = m;
            }
        }

        protected override void ActualFlushMessages() {
        }

        protected override StringBuilder ActualGetHandlerStatuses(StringBuilder sb) {
            throw new NotImplementedException();
        }

        protected override bool ActualIsClean() {
            throw new NotImplementedException();
        }

        private bool ValidateMessageDataForMock(MessageMetadata md, bool assertInline) {
            Assert.NotNull(md);

            if (ContextMustBe != null) {
                if (assertInline) {
                    Assert.Equal(ContextMustBe, md.Context);
                } else {
                    if (ContextMustBe != md.Context) {
                        return false;
                    }
                }
            }
            if (MethodNameMustBe != null) {
                if (assertInline) {
                    Assert.Equal(MethodNameMustBe, md.MethodName);
                } else {
                    if (MethodNameMustBe != md.MethodName) {
                        return false;
                    }
                }
            }

            if (BodyMustContain != null) {
                if (assertInline) {
                    Assert.True(md.Body.Contains(BodyMustContain), "The body did not contain the right info");
                } else {
                    if (!md.Body.Contains(BodyMustContain)) {
                        return false;
                    }
                }
            }
            if (ManagedThreadIdMustBe != null) {
                if (assertInline) {
                    Assert.Equal(ManagedThreadIdMustBe, md.NetThreadId);
                } else {
                    if (ManagedThreadIdMustBe != md.NetThreadId) {
                        return false;
                    }
                }
            }
            if (ProcessIdMustBe != null) {
                if (assertInline) {
                    Assert.Equal(ProcessIdMustBe, md.ProcessId);
                } else {
                    if (ProcessIdMustBe != md.ProcessId) {
                        return false;
                    }
                }
            }
            if (ClassNameMustBe != null) {
                if (assertInline) {
                    Assert.Equal(ClassNameMustBe, md.ClassName);
                } else {
                    if (ClassNameMustBe != md.ClassName) {
                        return false;
                    }
                }
            }

            if (ContextMustIncludeList.Count>0) {
                foreach(var x in ContextMustIncludeList) {
                    if (!md.MessageTags.ContainsKey(x.Item1)) {
                        return false;
                    }
                    if (x.Item2 != null) {
                        if (md.MessageTags[x.Item1] != x.Item2) {
                            return false;
                        }
                    }
                }
            }

            foreach (var vl in validators) {
                Assert.True(vl(md));
            }
            return true;
        }

        internal void SetContextMustExistForEveryMessage(string contextName, string withValue=null) {
            ContextMustIncludeList.Add(new Tuple<string, string>(contextName, withValue));
        }
    }
}