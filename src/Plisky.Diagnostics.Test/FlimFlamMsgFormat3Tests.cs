namespace Plisky.Diagnostics.Test {

    using System;
    using System.IO;
    using System.Text.Json;
    using Plisky.Diagnostics.Copy;
    using Xunit;

    [Collection(nameof(ParalellEnabledTestCollection))]
    public class FlimFlamMsgFormat3Tests {
#if NETCOREAPP

        // Todo : Fix pathing.
        [Fact(Skip ="Rediculous hard coded path!")]
        public void TestIncoming() {
            string[] l = File.ReadAllLines(@"XXX FIX ME XXX \_Dependencies\TestData\deserialisedmessages.txt");
            foreach (string x in l) {
                var res = JsonSerializer.Deserialize<MessageMetaDataTransport>(x);
                Assert.NotNull(res);
            }
        }

        [Fact(DisplayName = nameof(V2Formatter_CanDeserialize))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void V2Formatter_CanDeserialize() {
            // Message MetaData is the internal bilge structure, this formatter is a custom JSON formatter, therefore likely to have issues - but means that there is no dependency
            // on either newtonsoft or ms for Diags, meaning we can still support all frameworks.  Its therefore a nasty hardcoded json format  so this validates that we can
            // restore that format using the standard microsoft jsond deserialiser.

            var mmd = TestHelper.GetMessageMetaData()[0];
            string whenD = DateTime.Now.ToString("dd-MM-yyyy");
            string whenT = DateTime.Now.ToString("hh:mm:");

            var frm = new FlimFlamV2Formatter();
            string str = frm.Convert(mmd);
            Assert.NotNull(str);

            var res = JsonSerializer.Deserialize<MessageMetaDataTransport>(str);
            Assert.NotNull(res);

            Assert.Equal(mmd.Body, res.m);
            Assert.Equal(mmd.FurtherDetails, res.s);
            Assert.Equal(mmd.ClassName, res.c);
            Assert.Equal(mmd.LineNumber, res.l);
            Assert.Equal(mmd.MachineName, res.man);
            Assert.Equal(mmd.MethodName, res.mn);
            Assert.Equal(mmd.NetThreadId, res.nt);
            Assert.Equal(mmd.OsThreadId, res.t);
            Assert.Equal(mmd.CommandType, res.GetCommandType());
            Assert.Contains(whenT, res.dt);
            Assert.Contains(whenD, res.dt);
        }
    }

#endif
}