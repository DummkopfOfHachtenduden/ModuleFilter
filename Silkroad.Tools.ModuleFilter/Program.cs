using Silkroad.Framework.Common;
using Silkroad.Framework.Utility;
using Silkroad.Tools.ModuleProxy.Config;
using System;
using System.Reflection;

namespace Silkroad.Tools.ModuleProxy
{
    internal class Program
    {
        private static ServiceCollection _serviceCollection;

        private static void Main(string[] args)
        {
            SetupConsole();
            try
            {
                var config = new FilterConfig("Config\\Filter.xml");

                //Logger
                foreach (var logger in config.Logger)
                {
                    StaticLogger.Create(logger.Key);
                    StaticLogger.SetLogLevel(logger.Value.Ordinal, logger.Key);
                }
                StaticLogger.SetInstance();

                //Services
                _serviceCollection = new ServiceCollection();
                foreach (var serviceSettings in config.Services)
                {
                    var service = new Service(serviceSettings.Value);
                    _serviceCollection.Add(service);
                }

                //Plugins
                var pluginManager = new PluginManager(config.Plugins);
                foreach (var service in _serviceCollection)
                {
                    pluginManager.RegisterService(service);
                }
                var pluginCount = pluginManager.Load();
                StaticLogger.Instance.Info($"{pluginCount} plugins registered.");

                //Start services
                foreach (var service in _serviceCollection)
                {
                    var result = service.Start();
                    if (result == false)
                        StaticLogger.Instance.Fatal($"Failed to start {service.Settings.Name}, check Filter.xml and prev. errors");
                }

                StaticLogger.Instance.Info("Successfully initilized.");
                Console.Beep();

                while (true)
                {
                    var line = Console.ReadLine();
                    if (line == "exit" || line == "quit")
                        break;
                }
                foreach (var service in _serviceCollection)
                {
                    service.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.Beep();
                Console.WriteLine("Something fucked up really hard, please check Filter.xml");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.Beep();
                Console.ReadLine();
            }
        }

        private static void SetupConsole()
        {
            #region GetAssemblyInformation

            var asm = Assembly.GetExecutingAssembly();
            var title = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            var version = asm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            var configuration = asm.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
            var informationalVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            //var product = asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            var copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;

            //Display
            Console.WindowWidth = 120;
            Console.BufferHeight = 2500;
            Console.Title = string.Format("{0} {1} ({2}) [{3}] {4}",
                title,
                version,
                System.IO.File.GetLastWriteTime(asm.Location),
                string.IsNullOrEmpty(configuration) ? "Undefined" : string.Format("{0}", configuration),
                string.IsNullOrEmpty(informationalVersion) ? "" : string.Format("<{0}>", informationalVersion));

            Console.WriteLine(copyright);

            #endregion GetAssemblyInformation
        }
    }
}