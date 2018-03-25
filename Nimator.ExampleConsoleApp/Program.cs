using System;
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
        const int CheckIntervalInSecs = 15;
 
        // For ease of demo this is an embedded resource, but it could also be in a
        // seperate file or whatever persistence you'd prefer. It might be good not
        // to persist it in a database system, since your monitoring app should pro-
        // bably have as few dependencies as possible...
        static readonly string configResource = "config.json";
        static readonly string log4netConfigResource = "log4net.config";
        
        // See app.config for logging setup.
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger("Nimator","Nimator");
 
        static void Main()
        {
            ConfigureLog4net();
 
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionLogger;
 
            logger.Info("Creating Nimator.");
 
            var nimatorSettingsJson = ReadResource(configResource);
            var nimator = Nimator.FromSettings(logger,nimatorSettingsJson);
 
            logger.Info($"Nimator created. Starting timer for cycle every {CheckIntervalInSecs} seconds.");
 
            using (new Timer(_ => nimator.TickSafe(logger), null, 0, CheckIntervalInSecs * 1000))
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            logger.Info("Shutting down.");
        }
 
        static void ConfigureLog4net()
        {
            var logRepository = LogManager.CreateRepository("Nimator");
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{assemblyName}.{log4netConfigResource}"))
            {
                XmlConfigurator.Configure(logRepository, stream); // Alternatively: http://stackoverflow.com/a/10204514/419956
            }
        }
 
        static string ReadResource(string name)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{assemblyName}.{name}"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();            }
        }
 
 
        static void UnhandledExceptionLogger(object sender, UnhandledExceptionEventArgs eventArgs)
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