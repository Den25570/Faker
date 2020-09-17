using FakerPluginBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FakerLib.Plugin
{
    public class PluginController
    {
        public string pluginsPath = "Plugins";

        public PluginController(string pluginsPath)
        {
            this.pluginsPath = pluginsPath;
        }

        private List<object> LoadPlugins()
        {
            var plugins = new List<object>();
            string[] pluginFiles;
            try
            {
                DirectoryInfo pluginDirectory = new DirectoryInfo(pluginsPath);
                if (!pluginDirectory.Exists)
                    pluginDirectory.Create();
                pluginFiles = Directory.GetFiles(pluginsPath, "*.dll");
            }
            catch (Exception e)
            {
                return null;
            }

            foreach (var file in pluginFiles)
            {
                IEnumerable<Type> types;
                Assembly asm;
                try
                {
                    asm = Assembly.LoadFrom(file);
                    var targetFrameworkAttribute = Assembly.GetExecutingAssembly()
                               .GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute), false)
                               .SingleOrDefault();
                    types = asm.GetTypes().
                               Where(t => t.GetCustomAttributes(typeof(FakerClass), true).Length > 0);
                }
                catch (Exception e)
                {
                    continue;
                }

                foreach (var type in types)
                {
                    try
                    {
                        plugins.Add(asm.CreateInstance(type.FullName));
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }

            return plugins;
        }
    }
}
