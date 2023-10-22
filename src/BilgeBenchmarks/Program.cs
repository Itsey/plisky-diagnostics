using BenchmarkDotNet.Running;

namespace BilgeBenchmarks;

internal class Program {
    static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        var s = BenchmarkRunner.Run<BasicBenchmarks>();
    }
}