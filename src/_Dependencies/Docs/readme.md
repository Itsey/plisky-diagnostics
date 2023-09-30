### Plisky.Diagnostics

Plisky.Diagnostics is the home for Bilge, a .net trace library dating back to the first days of .net being available.   Bilge uses a series of methods to write the trace and one or more handlers to deal with the outputting of the trace.  Bilge is a developer first approach to trace so its most commonly used to stream trace messages to a dedicated viewer - FlimFlam.

<a href="https://itsey.github.io/diags-index.html" target="_blank">See the documentation here -> https://itsey.github.io/diags-index.html</a>    
<a href="https://github.com/Itsey/plisky-diagnostics" target="_blank">Issues and Code --> https://github.com/Itsey/plisky-diagnostics</a>

### Getting Started

<a href="https://itsey.github.io/diags-guide-quickstart.html" target="_blank">See this link for a full quick start guide -> https://itsey.github.io/diags-guide-quickstart.html.</a>

#### Quick Start

The following code sets up Bilge to write locally to a dedicated viewer application:    
```code
// Add Nuget Packages Plisky.Diagnostics and Plisky.Listeners.

// Open FlimFlam up on your local machine and allow network connections before running this
Bilge.AddHandler(new TCPHandler("127.0.0.1",9060));

// Create bilge with verbose logging
Bilge b = new Bilge("some context",tl:SourceLevel.Verbose);

// Log something
b.Info.Log("Hello Cruel World");

// Not normally required but for small console apps this ensures the trace is written before the app closes
b.FLush();
```


The following code adds console and file system logging:
```code

// Write to the console.
Bilge.AddHandler(new ConsoleHandler());

// Write locally to flimflam.
Bilge.AddHandler(new TCPHandler("127.0.0.1",9060));


// Write to a file in d:\temp keeping the filesize at 1mb.
var fhnd = new RollingFileSystemHandler(new RollingFSHandlerOptions() {
    Directory = "d:\\temp\\",
    FileName = "pdev.txt",
    FilenameIsMask = false,
    MaxRollingFileSize = "1mb",
});
Bilge.AddHandler(fhnd);

// Create bilge with verbose logging
Bilge b = new Bilge("some context",tl:SourceLevel.Verbose);

// Log something
b.Info.Log("Hello Cruel World");

// Not normally required but for small console apps this ensures the trace is written before the app closes
b.FLush();

```



