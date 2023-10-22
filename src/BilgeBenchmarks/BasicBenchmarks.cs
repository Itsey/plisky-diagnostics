using System.Dynamic;
using BenchmarkDotNet.Attributes;
using Plisky.Diagnostics;

namespace BilgeBenchmarks;

public class BasicBenchmarks {
    Bilge b1 = new Bilge();
    Bilge b2 = new Bilge(tl: System.Diagnostics.SourceLevels.Verbose);
    Bilge b3 = new Bilge(tl: System.Diagnostics.SourceLevels.Error );
    Bilge b4 = new Bilge(tl: System.Diagnostics.SourceLevels.Verbose);
    public BasicBenchmarks() {
        b4.ConfigureTrace(new TraceConfiguration() {
            AddClassDetailToTrace = true,
            AddTimestamps = true
        });
    }

    [Benchmark]
    public void BasicLoggingTraceOff() {
        b1.Info.Log("Hello, World!");
        b1.Warning.Log("Hello World!"); 
        b1.Verbose.Log("Hello World!");
        b1.Error.Log("Hello World!");
    }


    [Benchmark]
    public void BasicLoggingTraceVerbose() {        
        b2.Info.Log("Hello, World!");
        b2.Warning.Log("Hello World!");
        b2.Verbose.Log("Hello World!");
        b2.Error.Log("Hello World!");
    }


    [Benchmark]
    public void BasicLoggingTraceVerbose2() {
        
        b2.Info.Log("Hello, World!");
        b2.Warning.Log("Hello World!");
        b2.Verbose.Log("Hello World!");
        b2.Error.Log("Hello World!");
    }

    [Benchmark]
    public void BasicLoggingTraceError() {
        b3.Info.Log("Hello, World!");
        b3.Warning.Log("Hello World!");
        b3.Verbose.Log("Hello World!");
        b3.Error.Log("Hello World!");
    }

    [Benchmark]
    public void AddClassReferences() {
        b3.Info.Log("Hello, World!");
        b3.Warning.Log("Hello World!");
        b3.Verbose.Log("Hello World!");
        b3.Error.Log("Hello World!");
    }


    [Benchmark]
    public void BasicLoggingTraceVerboseWithSecondaryStrings() {
        b2.Info.Log("Hello, World!","This is a secondary string which contains additional information that is added to the trace being written");
        b2.Warning.Log("Hello World!", "This is a secondary string which contains additional information that is added to the trace being written");
        b2.Verbose.Log("Hello World!", "This is a secondary string which contains additional information that is added to the trace being written");
        b2.Error.Log("Hello World!", "This is a secondary string which contains additional information that is added to the trace being written");
    }


    [Benchmark]
    public void BasicLoggingTraceVerboseWithSecondaryStringsAndDynamic() {
        dynamic dobby = new ExpandoObject();
        dobby.arfle = "Barfle";
        dobby.gloop = "Gloop";
        dobby.woof = 4331;
        b2.Info.Log("Hello, World!", dobby);
        b2.Warning.Log("Hello World!", dobby);
        b2.Verbose.Log("Hello World!", dobby);
        b2.Error.Log("Hello World!", dobby);
    }

}


