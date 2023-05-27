using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;

namespace DevConsoleTest {

    /// <summary>
    /// Test Program for Bilge.
    /// </summary>
    internal class Program {
        //private static MyRoller storedRfsh;
        private static int hitCount = 0;

        private static void DoBasicTimingTests() {
            var b = new Bilge("TimingTests");

            bool paralells = false;

            b.Info.Log("Single Line Write");
            b.Info.TimeStart("updateDatabase", timerCategoryName: "Database");
            b.Info.TimeStop("updateDatabase", timerCategoryName: "Database");

            if (paralells) {
                var runners = new List<Task>();
                for (int t = 0; t < 20; t++) {
                    var tsk = Task.Run(() => {
                        for (int i = 0; i < 10000; i++) {
                            b.Info.Log($"Taskey {t} {i}", "arfle barfle gloop");
                        }
                    });
                    runners.Add(tsk);
                }
                Task.WaitAll(runners.ToArray());
            }
        }

        private static void DoBulkMessageTests() {
            var b = new Bilge("BulkMessageTests");
            for (int i = 0; i < 1000000; i++) {
                for (int j = 0; j < 100; j++) {
                    b.Info.Log("Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;");
                    b.Info.Log("Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;");
                    b.Info.Log("Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;");
                }
                bool dateChange = false;
                /*
                if (dateChange && storedRfsh != null) {
                    if (i % 100 == 0) {
                        storedRfsh.ActiveDate = storedRfsh.ActiveDate.AddDays(1);
                    }
                }*/
                Thread.Sleep(100);
            }
        }

        private static string SomeSlowString() {
            hitCount++;
            // EXTRA DEBUG >> Console.WriteLine($"Super Slow Method Hit >> {hitCount}");
            // Thread.Sleep(10);
            return new Random().Next(0, 100).ToString();
        }

        private static void DoPerformanceTests() {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("___________ PERF TESTS ______________ ");

            var tests = new List<QuickPerfTest>();

            var t1 = new QuickPerfTest("errorbilge-nocallback", SourceLevels.Error);
            t1.Test = () => { t1.PerfTestInstance.Verbose.Log(SomeSlowString()); };
            tests.Add(t1);

            var t2 = new QuickPerfTest("verbosebilge-callback", SourceLevels.Verbose);
            t2.Test = () => { t2.PerfTestInstance.Verbose.Log(() => { return SomeSlowString(); }); };
            tests.Add(t2);

            var t3 = new QuickPerfTest("errorbilge-nocallback", SourceLevels.Error);
            t3.Test = () => { t3.PerfTestInstance.Verbose.Log(SomeSlowString()); };
            tests.Add(t3);

            var t4 = new QuickPerfTest("errorbilge-callback", SourceLevels.Error);
            t4.Test = () => { t4.PerfTestInstance.Verbose.Log(() => { return SomeSlowString(); }); };
            tests.Add(t4);

            int loopControl = 100;
            foreach (var v in tests) {
                Console.WriteLine($"{v.TestName} starts");
                v.Sw.Start();
                for (int i = 0; i < loopControl; i++) {
                    v.Test();
                }
                v.Sw.Stop();
                Console.WriteLine($"END : {v.TestName} > {v.Sw.ElapsedMilliseconds.ToString()}ms @ {v.PerfTestInstance.ActiveTraceLevel}");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("___________ END PERF TESTS ______________ ");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void DoSourceTests() {
            var ass = new ActivitySource("Blob", "1");
        }

        private static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            try {
                Bilge.SetConfigurationResolver("v-**");
                Bilge.SetErrorSuppression(false);
                var b = new Bilge(tl: System.Diagnostics.SourceLevels.Verbose);
              /*  var thnd = new TCPHandler("127.0.0.1", 9060);
                thnd.SetFormatter(new FlimFlamV4Formatter());
                var fhnd = new RollingFileSystemHandler(new RollingFSHandlerOptions() {
                    Directory = "d:\\temp\\",
                    FileName = "pdev.txt",
                    FilenameIsMask = false,
                    MaxRollingFileSize = "1mb",
                });
                fhnd.SetFormatter(new FlimFlamV4Formatter());
                Bilge.AddHandler(thnd);
                Bilge.AddHandler(fhnd);*/
                Bilge.Alert.Online("testapp");

#if NETCOREAPP
                b.Info.Log("NET CORE!");
#endif

                bool includeInMemoryTests = false;
                bool includeRollingFileSytstemHandlerTests = false;
                bool includeCustomHandler = false;
                bool bulkMessageTests = false;
                bool sourceTests = false;
                bool perfTests = false;
                bool reportAndRecord = true;
                bool standardLogs = false;

                if (reportAndRecord) {
                    DoReportAndRecord();
                }
                if (perfTests) {
                    DoPerformanceTests();
                }
                if (includeInMemoryTests) {
                  //  SetUpInMemoryHandler();
                }

                if (includeCustomHandler) {
                    Bilge.AddHandler(new CustomHandler());
                }

                if (includeRollingFileSytstemHandlerTests) {
                    //storedRfsh = SetUpRollingFileSystemHandlerTests();
                }
                if (standardLogs) {
                    for (int i = 0; i < 100; i++) {
                        b.Info.Log($"Hello World {i}");
                        dynamic lg = new ExpandoObject();
                        lg.Fred = "Fred";
                        lg.Bob = 5;
                        lg.Structure = "none";

                        b.Info.Log("context", lg);

                        var f = new Dictionary<string, string>();
                        f.Add("plisky.moreinfo", "This is more info");
                        f.Add("appversion", "1.0.0.0");
                        b.Info.Log("context", f);
                    }

                    b.Info.Log("Hi");
                }

                b.Flush();
                for (int i = 0; i < 4; i++) {
                    Thread.Sleep(100);
                }

                if (bulkMessageTests) {
                    DoBulkMessageTests();
                }

                if (sourceTests) {
                    DoSourceTests();
                }

                Thread.Sleep(100);
                b.Flush();

                Console.WriteLine(b.GetDiagnosticStatus());
            } catch (Exception ex) {
                Console.WriteLine(Bilge.Default.GetDiagnosticStatus());
                Console.WriteLine("OUTER >> CATCH:" + ex.Message);
            }

            Console.WriteLine("all done");
        }

        private static void DoReportAndRecord() {
            var blr = new Bilge("ReportAndRecord");

            int res = blr.Error.Record(new ErrorDescription(0x0001, 0x0001, "Access Denied"));

            res = blr.Error.ReportRecord(new ErrorDescription(0x0001, 0x0001, "Access Denied"), "Context");
            var ad = new Exception("Access Denied") {
                HResult = res,
            };

            blr.Error.Report(1, "This was an error");
            dynamic d = new ExpandoObject();
            d.Bob = "Fred";
            d.Fred = "Bob";
            d.async = "async";
            blr.Error.Report(1, "Context Alfredo", d);
            blr.Error.Report(0x0001, 0x0001, "this was an error");

            dynamic xo = new ExpandoObject();
            xo.ErrorLevel = 12;
            xo.Message = "This is completely boned";

            res = blr.Error.Report(0x0001, 0x0001, "Access Denied", xo);
            blr.Assert.Recorded(res, "error check");   // No Assertion

            blr.Assert.ConfigureAsserts(AssertionStyle.Throw);

            blr.Assert.Recorded(res, "TestCode");   // Assertion

            var adx = new Exception("Access Denied") {
                HResult = res,
            };
        }

        /*
        private static void SetUpInMemoryHandler() {
            var imh = new InMemoryHandler() {
                MaxQueueDepth = 1000,
                MessageCallback = b => {
                    Console.WriteLine(b.Body);
                    return b;
                }
            };
            Bilge.AddHandler(imh);
        }

        
        private static MyRoller SetUpRollingFileSystemHandlerTests() {
            var mr = new MyRoller(new RollingFSHandlerOptions() {
                Directory = @"C:\Temp\_Deleteme\Lg\",
                FileName = "Log_%dd%mm%yy.log",
                MaxRollingFileSize = "2mb",
                FilenameIsMask = true
            });
            var cf = new CustomOutputFormatter();
            cf.FormatString = "{0} - {1}";
            mr.SetFormatter(cf);
            Bilge.AddHandler(mr);
            return mr;
        }
        */
    }
}