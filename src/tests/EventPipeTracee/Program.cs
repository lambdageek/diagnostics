﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventPipeTracee
{
    class Program
    {
        private const string AppLoggerCategoryName = "AppLoggerCategory";

        static void Main(string[] args)
        {
            TestBody(args[0]);
        }

        private static void TestBody(string loggerCategory)
        {
            Console.Error.WriteLine("Starting remote test process");
            Console.Error.Flush();

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddEventSourceLogger();
                // Set application defined levels
                builder.AddFilter(null, LogLevel.Error); // Default
                builder.AddFilter(AppLoggerCategoryName, LogLevel.Warning);
            });

            using var loggerFactory = serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
            var customCategoryLogger = loggerFactory.CreateLogger(loggerCategory);
            var appCategoryLogger = loggerFactory.CreateLogger(AppLoggerCategoryName);

            Console.Error.WriteLine($"{DateTime.UtcNow} Awaiting start");
            Console.Error.Flush();
            if (Console.Read() == -1)
            {
                throw new InvalidOperationException("Unable to receive start signal");
            }

            Console.Error.WriteLine($"{DateTime.UtcNow} Starting test body");
            Console.Error.Flush();
            TestBodyCore(customCategoryLogger, appCategoryLogger);

            //Signal end of test data
            Console.WriteLine("1");

            Console.Error.WriteLine($"{DateTime.UtcNow} Awaiting end");
            Console.Error.Flush();
            if (Console.Read() == -1)
            {
                throw new InvalidOperationException("Unable to receive end signal");
            }

            Console.Error.WriteLine($"{DateTime.UtcNow} Ending remote test process");
        }

        //TODO At some point we may want parameters to choose different test bodies.
        private static void TestBodyCore(ILogger customCategoryLogger, ILogger appCategoryLogger)
        {
            //Json data is always converted to strings for ActivityStart events.
            using (var scope = customCategoryLogger.BeginScope(new Dictionary<string, object> {
                    { "IntValue", "5" },
                    { "BoolValue", "true" },
                    { "StringValue", "test" } }.ToList()))
            {
                customCategoryLogger.LogInformation("Some warning message with {arg}", 6);
            }

            customCategoryLogger.LogWarning("Another message");

            appCategoryLogger.LogInformation("Information message.");
            appCategoryLogger.LogWarning("Warning message.");
            appCategoryLogger.LogError("Error message.");
        }
    }
}
