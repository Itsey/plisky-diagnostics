using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;

namespace HandlersTestConsole {

    internal class Program {
        private static async Task Main(string[] args) {
            Console.WriteLine("Hello, World!");
            _ = Bilge.Alert.Online("Bob");

            _ = Bilge.SetConfigurationResolver("v-**");
            _ = Bilge.AddHandler(new ConsoleHandler());
            var b = new Bilge("Test");

            for (int i = 0; i < 10; i++) {
                b.Info.Log("This is a test message");
            }

            Console.WriteLine("pre");
            Thread.Sleep(100);
            await b.Flush();

            Console.WriteLine("out");
            Thread.Sleep(100);

            _ = Console.ReadLine();

        }
    }
}