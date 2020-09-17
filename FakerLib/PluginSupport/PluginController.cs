using FakerPluginBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FakerLib.Plugin
{
    public class PluginController
    {
        public List<object> LoadPlugins(string pluginsPath)
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

        public List<Tuple<Type, Func<Random, Type[], object>>> GetPropertyGenerators(object plugin)
        {
            IEnumerable<MethodInfo> methodsInfo = plugin.GetType().GetMethods()
                .Where(t => t.IsPublic)
                .Where(t => t.GetCustomAttributes(typeof(FakerMethod), true).Length > 0);

            var input1 = Expression.Parameter(typeof(Random), "input1");
            var input2 = Expression.Parameter(typeof(List<object>), "input2");

            List<Tuple<Type, Func<Random, Type[], object>>> propertyGenerators = new List<Tuple<Type, Func<Random, Type[], object>>>();
            foreach (var method in methodsInfo)
            {
                Type returnType = ((FakerMethod)method.GetCustomAttribute(typeof(FakerMethod), true)).ReturnType;
                Func<Random, Type[], object> result = (Func<Random, Type[], object>)
                    Delegate.CreateDelegate(typeof(Func<Random, Type[], object>), plugin, method);

                propertyGenerators.Add(new Tuple<Type, Func<Random, Type[], object>>(returnType, result));
            }
            return propertyGenerators;
        }
    }
}
