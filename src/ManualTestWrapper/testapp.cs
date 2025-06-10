using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ManualTestWrapper {
    internal class TestApp {
        private readonly ILogger<TestApp> logger;

        public TestApp(ILogger<TestApp> logger) {
            this.logger = logger;
        }

        public void Run() {

            for (int i = 0; i < 10; i++) {
                logger.LogInformation("This is an info message");
                logger.LogDebug("This is a debug message.");
                logger.LogWarning("This is a warning message.");
                logger.LogError("This is an error message.");
                logger.LogCritical("This is a critical message.");
                logger.LogTrace("This is a trace message.");

                logger.BeginScope("Test Scope");
                logger.LogInformation("This message is within a scope.");
                Thread.Sleep(100);
            }

        }
    }
}
