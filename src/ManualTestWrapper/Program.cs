using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;

namespace ManualTestWrapper {
    internal class Program {
        static void Main(string[] args) {
            Bilge.SetConfigurationResolver((x, y) => System.Diagnostics.SourceLevels.Verbose);
            Bilge.AddHandler(new TCPHandler("127.0.0.1", 9060, true));
            Bilge b = new Bilge("ManualTestWrapper");


            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            // Resolve and run the app
            var app = serviceProvider.GetRequiredService<TestApp>();
            app.Run();

            for (int i = 0; i < 10; i++) {
                b.Info.Log($"This is a test message {i}");
            }

            Bilge.ForceFlush().Wait();
        }

        private static void ConfigureServices(IServiceCollection services) {
            // Add logging
            services.AddTransient<ILogger, ILoggerWrapper>();
            services.AddLogging(configure => configure.AddBilge().SetMinimumLevel(LogLevel.Trace));

            // Register your application entry point
            services.AddTransient<TestApp>();
        }
    }
}
