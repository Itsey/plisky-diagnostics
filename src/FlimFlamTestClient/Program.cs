
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using System.Diagnostics;


List<string> messages = new List<string>();


Bilge.SetConfigurationResolver( (x,y) => {
    return SourceLevels.Verbose;
});
Bilge b = new Bilge();

string fname = @"D:\temp\_scratch\logs\bilgesample.log";
File.Delete(fname);

// You must enable tracing
b.ActiveTraceLevel = SourceLevels.Verbose;

// To get any output you must use a handler - this is one talking to FlimFlam
var hnd = new TCPHandler("127.0.0.1", 9060);
hnd.SetFormatter(new FlimFlamV4Formatter());
b.AddHandler(hnd);

var h = new RollingFileSystemHandler(new RollingFSHandlerOptions() {
    Directory = @"D:\temp\_scratch\logs",
    FileName = "BilgeSample.log",
    MaxRollingFileSize = "120mb"
});

b.AddHandler(h);





Bilge.Alert.Online("Test App");

if (false) {
    var e = new Exception("hello", new Exception("inner hello"));
    b.Error.Dump(e, "oops");
}

if (true) {
    b.Info.E();
    b.Info.Log("Hello Wonderful World");
    b.Info.E("IN");
    b.Info.Log("Hello Wonderful IN");
    b.Info.X("IN");
    b.Info.X();
}

if (false) {
    var rnm = new ReallyLongMessages();
    rnm.Go();
    // Your Logging Goes Here.

    Console.WriteLine("Hello Wonderful, World");




    b.Info.E();

    b.Info.Flow();
    b.Info.Log("Hello Wonderful World");
    for (int i = 0; i < 10; i++) {
        b.Info.More($"much more info {i}");
        b.Info.E("AAA");
        b.Info.Log("In the loop");
        b.Info.X("AAA");
    }
    b.Info.More("much more info");

    b.Info.X();

}
Thread.Sleep(100);
// Not normally required but for small test apps that close quickly it can help
await b.Flush();

Thread.Sleep(100);