#pragma warning disable CS1591 // XML Comments
#pragma warning disable SA1600 // XML Comments

namespace DevConsoleTest {
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using System.Threading.Tasks;
    using Plisky.Diagnostics;
    using Plisky.Diagnostics.Listeners;
    
    public enum SubSystems {
        Unknown = 0x0000,
        Program = 0x0001,
        TestArea = 0x0002
    }

    /// <summary>
    /// Dummy implementation of the rfsh
    /// </summary>
    public class TempHandler : BaseHandler, IBilgeMessageListener {

        
        public TempHandler() {

        
        }


        private Dictionary<string, MessageMetadata> MatchedErrors = new(); 

        public string Name => "temp";

        public Task HandleMessageAsync(MessageMetadata[] msg) {

            foreach (var l in msg) {

                if (l.CommandType == TraceCommandTypes.CommandData) {

                    
                    if (l.MessageTags.ContainsKey("plisky.commandtype")) {
                        string f = l.MessageTags["plisky.commandtype"];
                        if (f== "ERROR_RECORD") {

                            if (l.MessageTags.ContainsKey("plisky.hresult")) {
                                MatchedErrors.Add(l.MessageTags["plisky.hresult"], l);
                            }
                        }
                    }
                }
               
            }

            return Task.CompletedTask;
        }

        public string GetStatus() {
            return "OK";
        }

        private Func<short, string> mapper = null;

        public bool UseSubSystems { get; set; } = true;

        public void AddSubsystemMapping( Func<short, string> mapping ) { 
            if (mapping!=null) {
                UseSubSystems  = true;
                mapper = mapping;
            } else {
                UseSubSystems = false;
                mapper = null;
            }
        }

        public void WriteReport() {
            StringBuilder sb = new();
            sb.Append("<html><head><title>Errors</title></head><body>"); sb.Append(Environment.NewLine);

            sb.Append("<h1>Errors</h1>"); sb.Append(Environment.NewLine);
            sb.Append($"<p>Errors list generated at {DateTime.Now}</p>"); sb.Append(Environment.NewLine);

            foreach (var v in MatchedErrors) {
                sb.Append($"<!--ELA-START[{v.Key}]-->");

             
                sb.Append(Environment.NewLine);
                sb.Append($"<h3>{v.Key}</h3>");
                sb.Append(Environment.NewLine);
                if (UseSubSystems) {
                    string valToParse = v.Key;
                    if (v.Key.StartsWith("0x")) {
                        valToParse = v.Key.Substring(2);
                    }
                    int answer = int.Parse(valToParse,NumberStyles.HexNumber);
                    short subsys = (short)(answer >> 16);
                    short errorNo = (short)(answer & 0x0000FFFF);
                    string ss = mapper(subsys);
                    if (!string.IsNullOrEmpty(ss)) {
                        sb.Append($"<small>Subsystem: {ss}, Error: {errorNo}</small>{Environment.NewLine}");
                    }
                }
                sb.Append("<p>");
                sb.Append(v.Value.Body);
                sb.Append("</p>");
                sb.Append(Environment.NewLine);
                sb.Append($"<!--ELA-STOP[{v.Key}]-->");
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine);
            sb.Append("</body></html>");

            File.WriteAllText(@"c:\temp\errors.html", sb.ToString());
        }
    }


    

    
}
#pragma warning restore CS1591 // XML Comments
#pragma warning restore SA1600 // XML Comments