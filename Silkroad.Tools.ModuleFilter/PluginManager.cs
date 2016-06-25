using Silkroad.Framework.Common;
using Silkroad.Framework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Silkroad.Tools.ModuleProxy
{
    internal class PluginManager
    {
        private Dictionary<string, ServiceType> _plugins;
        private List<Service> _services;

        private AppDomain _domain;

        public PluginManager(Dictionary<string, ServiceType> plugins)
        {
            _plugins = plugins;
            _services = new List<Service>();
            _domain = AppDomain.CreateDomain("FilterDomain");
        }

        internal void RegisterService(Service service)
        {
            _services.Add(service);
        }

        internal int Load()
        {
            var loadedModuleCount = 0;
            foreach (var kvp in _plugins)
            {
                var file = new FileInfo(kvp.Key + ".dll");

                if (!file.Exists)
                {
                    StaticLogger.Instance.Fatal($"{nameof(PluginManager)}:->{Caller.GetMemberName()}: {file.Name} not found.");
                    continue;
                }

                try
                {
                    var asmName = new AssemblyName
                    {
                        CodeBase = file.Name
                    };
                    var asm = _domain.Load(asmName);
                    var type = asm.GetType(kvp.Key + ".Plugin");

                    StaticLogger.Instance.Info("{0} ({1}) successfully loaded", file.Name, asm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.0.0.0");
                    foreach (var service in _services)
                    {
                        if (service.Settings.Type == kvp.Value)
                        {
                            var instance = Activator.CreateInstance(type) as IPlugin;
                            instance.Register(kvp.Key, service);

                            StaticLogger.Instance.Info($"{kvp.Key} [{kvp.Value}] registered for {service.Settings.Name}");

                            loadedModuleCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StaticLogger.Instance.Fatal(ex, $"{kvp.Key} [{kvp.Value}] failed to load");
                }
            }
            return loadedModuleCount;
        }
    }
}