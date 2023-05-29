namespace Plisky.Diagnostics.Test {

    using System.Collections.Generic;
    using System.Diagnostics;

    public class TestHelper {

        public static MessageMetadata[] GetMessageMetaData(int howMany = 1) {
            var result = new List<MessageMetadata>();

            for (int i = 0; i < howMany; i++) {
                var r = new MessageMetadata() {
                    Body = "Body",
                    ClassName = "Class",
                    CommandType = TraceCommandTypes.LogMessage,
                    FileName = @"C:\temp\temp\filename.txt",
                    FurtherDetails = " This is further details",
                    Context = "this is the content",
                    MachineName = "AMACHINENAME123",
                    MethodName = "AMethod_1234",
                    LineNumber = "10",
                    ProcessId = "1234565",
                    NetThreadId = "12abc",
                    OsThreadId = "1234"
                };
                result.Add(r);
            }
            return result.ToArray();
        }

        internal static Bilge GetBilgeAndClearDown(BilgeRouter rt = null, string context = null, SourceLevels sl = SourceLevels.All, bool simplify = true) {
            Bilge.ClearConfigurationResolver();

            if (rt != null) {
                Bilge.SimplifyRouter(rt);
            } else if (simplify) {
                Bilge.SimplifyRouter();
            }

            Bilge result;

            if (sl != SourceLevels.All) {
                if (context != null) {
                    result = new Bilge(context, tl: sl, resetDefaults: true);
                } else {
                    result = new Bilge(tl: sl, resetDefaults: true);
                }
            } else {
                if (context != null) {
                    result = new Bilge(context, resetDefaults: true);
                } else {
                    result = new Bilge(resetDefaults: true);
                }
            }

            return result;
        }
    }
}