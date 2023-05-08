using System.Collections.Generic;
using System.Threading.Tasks;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Xunit;

namespace Plisky.Handlers.Test.Mocks {
    internal class MockMessageHandler : BaseHandler, IBilgeMessageListener {
        public int AssertionMessageCount = 0;
        public volatile int TotalMessagesRecieved;
        private readonly List<MessageMetadata> allMessagesRecieved = new List<MessageMetadata>();

        private string ContextMustBe = null;
        private int flushRecieved = 0;
        private MessageMetadata lastMessageData;
        private string MethodNameMustBe = null;

        public MockMessageHandler(string nme = nameof(MockMessageHandler)) {
            Name = nme;
            LastMessageBatchSize = 0;
            Priority = 100;
        }

        public string BodyMustContain { get; private set; }

        public int LastMessageBatchSize { get; set; }

        public string ManagedThreadIdMustBe { get; private set; }

        public string Name { get; set; }

        public string ProcessIdMustBe { get; private set; }

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

        public override void Flush() {
            base.Flush();
            flushRecieved++;
        }

        public string GetStatus() {
            return "hello";
        }

        public void HandleMessage(MessageMetadata md) {
        }

        public Task HandleMessageAsync(MessageMetadata[] msg) {
            LastMessageBatchSize = msg.Length;
            TotalMessagesRecieved += msg.Length;

            foreach (var m in msg) {
                if (m.CommandType == TraceCommandTypes.AssertionFailed) {
                    AssertionMessageCount++;
                }
                lock (allMessagesRecieved) {
                    allMessagesRecieved.Add(m);
                }
                lastMessageData = m;
            }

            return Task.CompletedTask;
        }

        public IBilgeMessageListener InitiliaseFrom(string initialisationString) {
            return null;
        }

        internal void AssertContextIs(string v) {
            ContextMustBe = v;
        }

        internal void AssertManagedThreadId(int managedThreadId) {
            ManagedThreadIdMustBe = managedThreadId.ToString();
        }

        internal void AssertProcessId(int testProcId) {
            ProcessIdMustBe = testProcId.ToString();
        }

        internal MessageMetadata GetMostRecentMessage() {
            return lastMessageData;
        }

        internal void SetMethodNameMustContain(string v) {
            MethodNameMustBe = v;
        }

        internal void SetMustContainForBody(string v) {
            BodyMustContain = v;
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
            return true;
        }
    }
}