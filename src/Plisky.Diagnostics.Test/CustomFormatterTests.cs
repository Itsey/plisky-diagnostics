namespace Plisky.Diagnostics.Test {

    using System.Text.Json;
    using Plisky.Diagnostics.Copy;
    using Plisky.Test;
    using Xunit;

    [Collection(nameof(ParalellEnabledTestCollection))]
    public class CustomFormatterTests {


        [Fact(DisplayName = nameof(CustomFormatter_ElementBody_Works))]
        [Build(BuildType.Any)]
        public void CustomFormatter_ElementBody_Works() {
            MessageMetadata mmd = TestHelper.GetMessageMetaData()[0];

            var frm = new CustomOutputFormatter(new MessageFormatterOptions() {
                AppendNewline = false
            });
            
            frm.FormatString = "{1}";
            string str = frm.Convert(mmd);

            Assert.NotNull(str);
            Assert.Equal(str, mmd.Body);
        }

        [Fact(DisplayName = nameof(CustomFormatter_ElementClassname_Works))]
        [Build(BuildType.Any)]
        public void CustomFormatter_ElementClassname_Works() {
            MessageMetadata mmd = TestHelper.GetMessageMetaData()[0];

            var frm = new CustomOutputFormatter(new MessageFormatterOptions() {
                AppendNewline = false
            });

            frm.FormatString = "{2}";
            string str = frm.Convert(mmd);

            Assert.NotNull(str);
            Assert.Equal(str, mmd.ClassName);
        }

        [Fact(DisplayName = nameof(CustomFormatter_ElementLineNumber_Works))]
        [Build(BuildType.Any)]
        public void CustomFormatter_ElementLineNumber_Works() {
            MessageMetadata mmd = TestHelper.GetMessageMetaData()[0];

            var frm = new CustomOutputFormatter(new MessageFormatterOptions() {
                AppendNewline = false
            });

            frm.FormatString = "{3}";
            string str = frm.Convert(mmd);

            Assert.NotNull(str);
            Assert.Equal(str, mmd.LineNumber);
        }
    }
}