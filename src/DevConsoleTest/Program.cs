using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;

namespace DevConsoleTest {

    /// <summary>
    /// Test Program for Bilge.
    /// </summary>
    internal class Program {

        // private static MyRoller storedRfsh;
        private static int hitCount = 0;


        private static async Task Main(string[] args) {
            //Bilge.SimplifyRouter();
            Bilge.SetConfigurationResolver("v-**");
            var b = new Bilge("", "", tl: System.Diagnostics.SourceLevels.Verbose);

            var thnd = new TCPHandler("127.0.0.1", 9060);
            //  b.AddHandler(thnd);

            Bilge.Alert.Online("Bob");
            b.Info.Log("Starting Test - Timings");
            //DoBasicTimingTests();
            b.AddHandler(new RollingFileSystemHandler(new RollingFSHandlerOptions() {
                CanCreate = true,
                Directory = "X:\\Code\\ipir\\inputFiles",
                FileName = "Bulk",
                FilenameIsMask = false,
                MaxRollingFileSize = "10gb"
            }));


            //DoBulkMessageTests(300000, false);
            DoCalculatedMessageTests();
            await b.Flush();

            Thread.Sleep(100);
            return;

            Console.WriteLine("Hello World!");
            try {
                Bilge.SetConfigurationResolver("v-**");
                Bilge.SetErrorSuppression(false);


                Bilge.Alert.Online("Bob");



                //Bilge.AddHandler(new ConsoleHandler());
                Bilge.AddHandler(thnd);

                /*                try {
                                    throw b.Error.ReportRecordException<FileNotFoundException>((short)SubSystems.Program, 123, "Test", null);

                                }
                                catch (Exception ax) {
                                    Console.WriteLine(ax.Message);
                                }*/


                /*
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
                for (int i = 0; i < 100; i++) {
                    b.Info.Log($"NET CORE! {i}");
                }
#endif

                await b.Flush();
                bool reportAndRecord = true;

                if (reportAndRecord) {
                    DoReportAndRecord();
                }

                /*th.AddSubsystemMapping((ss) => {
                    switch (ss) {
                        case (short)SubSystems.Unknown: return "Unknown";
                        case (short)SubSystems.Program: return "Program";
                        case (short)SubSystems.TestArea: return "TestArea";

                        default:
                            return string.Empty;
                    }

                });
                th.WriteReport();*/

                bool perfTests = true;
                if (perfTests) {
                    DoPerformanceTests();
                }

                bool standardLogs = true;
                if (standardLogs) {
                    for (int i = 0; i < 100; i++) {
                        b.Info.Log($"Hello World {i}");
                        dynamic lg = new ExpandoObject();
                        lg.Fred = "Fred";
                        lg.Bob = 5;
                        lg.Structure = "none";

                        b.Info.More("context", lg);

                        var f = new Dictionary<string, string>();
                        f.Add("plisky.moreinfo", "This is more info");
                        f.Add("appversion", "1.0.0.0");
                        b.Info.Log("context", f);
                    }

                    b.Info.Log("Hi");
                }

                //await b.Flush();
                for (int i = 0; i < 4; i++) {
                    Thread.Sleep(100);
                }

                bool bulkMessageTests = true;
                if (bulkMessageTests) {
                    DoBulkMessageTests();
                }

                bool sourceTests = true;
                if (sourceTests) {
                    DoSourceTests();
                }

                Thread.Sleep(100);
                // b.Flush();

                Console.WriteLine(b.GetDiagnosticStatus());
            } catch (Exception ex) {
                Console.WriteLine(Bilge.Default.GetDiagnosticStatus());
                Console.WriteLine("OUTER >> CATCH:" + ex.Message);
            }

            Console.WriteLine("all done");
        }

        protected static string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. \r\n Sed do eiusmod tempor incididunt ut \r\n labore et dolore magna aliqua. \r\n Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        protected static string longLoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";

        protected static string longSnickett = "Miserable orphans, Baudelaire, Count Olaf, treachery, secret passageways, V.F.D., Incredibly Deadly Viper, sugar bowl, eye tattoo, unfortunate events, library, disguise, fire, spyglass, Esmé Squalor, Quagmire triplets, Volunteer Fire Department, deception, peril, mystery, Beatrice, tragic, villainous schemes, Sunny's teeth, Klaus's glasses, Violet's inventions, dreadful, melancholy, betrayal, bravery, cryptic, misfortune, and hope" +
                                "Miserable orphans, Baudelaire, Count Olaf, treachery, secret passageways, V.F.D., Incredibly Deadly Viper, sugar bowl, eye tattoo, unfortunate events, library, disguise, fire, spyglass, Esmé Squalor, Quagmire triplets, Volunteer Fire Department, deception, peril, mystery, Beatrice, tragic, villainous schemes, Sunny's teeth, Klaus's glasses, Violet's inventions, dreadful, melancholy, betrayal, bravery, cryptic, misfortune, and hope." +
                                "Miserable orphans, Baudelaire, Count Olaf, treachery, secret passageways, V.F.D., Incredibly Deadly Viper, sugar bowl, eye tattoo, unfortunate events, library, disguise, fire, spyglass, Esmé Squalor, Quagmire triplets, Volunteer Fire Department, deception, peril, mystery, Beatrice, tragic, villainous schemes, Sunny's teeth, Klaus's glasses, Violet's inventions, dreadful, melancholy, betrayal, bravery, cryptic, misfortune, and hope." +
                                "Miserable orphans, Baudelaire, Count Olaf, treachery, secret passageways, V.F.D., Incredibly Deadly Viper, sugar bowl, eye tattoo, unfortunate events, library, disguise, fire, spyglass, Esmé Squalor, Quagmire triplets, Volunteer Fire Department, deception, peril, mystery, Beatrice, tragic, villainous schemes, Sunny's teeth, Klaus's glasses, Violet's inventions, dreadful, melancholy, betrayal, bravery, cryptic, misfortune, and hope.";

        private static void DoBasicTimingTests() {
            var b = new Bilge("TimingTests");

            bool paralells = true;

            b.Info.Log("Single Line Write");
            b.Info.TimeStart("updateDatabase", timerCategoryName: "Database");


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
            b.Info.TimeStop("updateDatabase", timerCategoryName: "Database");
        }

        private static void DoCalculatedMessageTests() {
            var b = new Bilge("CalculatedMessageTests");

            b.Info.Flow(">>1>>");
            b.Info.Log(">>2>> STANDARD");
            b.Info.Log(">>3>>" + loremIpsum);
            b.Info.Log(">>4>> STANDARD");
            b.Info.Log(">>5>>" + longLoremIpsum, longSnickett);
            b.Info.Log(">>6>> STANDARD");
            for (int i = 0; i < 10; i++) {
                b.Info.Log($">>{6 + i}>>Monkey  World. {i}");
            }
            b.Info.Log(">>17>> END|");
        }

        private static void DoBulkMessageTests(int outer = 100, bool multiLine = true) {
            var b = new Bilge("BulkMessageTests");
            var r = new Random();

            for (int i = 0; i < outer; i++) {

                for (int j = 0; j < 10; j++) {
                    b.Info.Flow();
                    b.Info.Flow("xX");
                    b.Info.Log("Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;", "Monkey  World");

                    b.Info.Log("Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;");
                    b.Warning.Log("Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;Hello World;");


                    int selector = r.Next(100);
                    if (selector < 10) {
                        b.Verbose.Log("Verby Merby Verbyosey", "Monkey  World");
                    } else if (selector < 20) {
                        b.Info.Dump(new Exception("Arfle Barfle Gloop"), "Exception Context");
                    } else if (selector < 30) {
                        b.Error.Log("Errory Merby Errorosey", "Monkey  World");
                    } else if (selector < 40) {
                        b.Warning.Dump(new Exception("Arfle Barfle Gloop"), "Exception Context");
                    } else if (selector < 50) {
                        b.Info.Log(longLoremIpsum, longSnickett);

                    }

                    if (r.Next(100) < 25) {
                        if (multiLine) {
                            b.Info.Log(loremIpsum);
                        } else {
                            b.Info.Log("Hello World;Hello World;Hello World;Hello World;~~#~~Hello World;Hello World;Hello World;", "Monkey  World");
                        }
                    }
                }


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