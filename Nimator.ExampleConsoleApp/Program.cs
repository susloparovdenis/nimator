﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using log4net;
using log4net.Config;

namespace Nimator.ExampleConsoleApp
{
    class Program
    {
        // This would probably be a bit higher (e.g. 60 secs or even more) and in
        // the App.config for production scenarios:
        private const int CheckIntervalInSecs = 15;

        // For ease of demo this is an embedded resource, but it could also be in a
        // seperate file or whatever persistence you'd prefer. It might be good not
        // to persist it in a database system, since your monitoring app should pro-
        // bably have as few dependencies as possible...
        private static readonly string configResource = Assembly.GetExecutingAssembly().GetName().Name + ".config.json";
        private static readonly string log4netConfigResource = Assembly.GetExecutingAssembly().GetName().Name + ".log4netconfig.json";

        // See app.config for logging setup.
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("Nimator","Nimator");

        static void Main()
        {
            var logRepository = LogManager.CreateRepository("Nimator");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(log4netConfigResource))
            {
               
                XmlConfigurator.Configure(logRepository, stream); // Alternatively: http://stackoverflow.com/a/10204514/419956
            }

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionLogger;

            logger.Info("Creating Nimator.");

            var nimator = CreateNimator();

            logger.Info($"Nimator created. Starting timer for cycle every {CheckIntervalInSecs} seconds.");

            using (new Timer(_ => nimator.TickSafe(logger), null, 0, CheckIntervalInSecs * 1000))
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }

            logger.Info("Shutting down.");
        }

        private static INimator CreateNimator()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(configResource))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                return Nimator.FromSettings(logger, json);
            }
        }

        private static void UnhandledExceptionLogger(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            var exc = eventArgs.ExceptionObject as Exception;

            if (exc != null)
            {
                logger.Fatal("Unhandled exception occurred.", exc);
            }
            else
            {
                logger.Fatal("Fatal problem without Excption occurred.");
                logger.Fatal(eventArgs.ExceptionObject);
            }
        }
    }
}
