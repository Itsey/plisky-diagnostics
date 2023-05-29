# plisky-diagnostics
This is the repository for the Plisky.Diagnostics nuget package which houses Bilge.


# Introduction 
Plisky.Diagnostics is a trace library containing Plisky.Diagnostics.Bilge for application trace and logging.  Bilge is able to write out using listeners from Plisky.Diagnostics.Listeners Nuget package and is designed to work in tandem with FlimFlam which is a log parser and viewer.

This trace library is for detailed developer level trace to support diagnosing complex faults.


# Getting Started
Bilge should be added from Nuget.  [Nuget Package](https://www.nuget.org/packages/Plisky.Diagnostics/)    

Documentation is available at this link [https://itsey.github.io/diags-index.html](https://itsey.github.io/diags-index.html)    
See the quick start at this link [https://itsey.github.io/diags-guide-quickstart.html](https://itsey.github.io/diags-guide-quickstart.html)

The simplest (and not very useful) start is this:    
Bilge b = new Bilge(tl: System.Diagnostics.SourceLevels.Verbose);    
b.AddHandler(new SimpleTraceFileHandler());    
b.Info.Log("Hello Cruel World");    