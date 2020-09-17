using FakerLib.Plugin;
using FakerPluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FakerLib
{
    public class FakerConfig
    {
        private Dictionary<Type, Dictionary<Tuple<Type, string>, Func<Random, Type[], object>>> configExpressionDelegate;
        private List<Object> plugins;

        public void Add<ParentType, ChildType>(Func<Random, Type[], object> del, Expression<Func<ParentType, ChildType>> specifiedField = null)
        {
            Dictionary<Tuple<Type, string>, Func<Random, Type[], object>> targetDictionary;
            if (!configExpressionDelegate.TryGetValue(typeof(ParentType), out targetDictionary))
            {
                targetDictionary = new Dictionary<Tuple<Type, string>, Func<Random, Type[], object>>();
                configExpressionDelegate.Add(typeof(ParentType), targetDictionary);
            }

            string filedName = specifiedField != null ? ((MemberExpression)specifiedField.Body).Member.Name : null;
            targetDictionary.Add(new Tuple<Type, string>(typeof(ChildType), filedName), del);
        }

        public Func<Random, Type[], object> GetExpressionDelegate(Type ParentType, Type ChildType, string ChildName)
        {
            Dictionary<Tuple<Type, string>, Func<Random, Type[], object>> childDictionary;
            Func<Random, Type[], object> del = null;

            if (configExpressionDelegate.TryGetValue(ParentType, out childDictionary) && searchForDelegate(ChildType, childDictionary, ChildName, out del)){}
            else
            {
                if (configExpressionDelegate.TryGetValue(ParentType, out childDictionary) && searchForDelegate(ChildType, childDictionary, null, out del)){}
                else
                {
                    if (configExpressionDelegate.TryGetValue(typeof(object), out childDictionary) && searchForDelegate(ChildType, childDictionary, null, out del)) {}
                    else
                    {
                        del = null; 
                    }
                }
            }
            return del;
        }

        public void Configure(Faker fakerInstance)
        {
            foreach(var plugin in plugins)
            {
                if (plugin.GetType().GetInterfaces().Contains(typeof(IUsingBridge)))
                {
                    ((IUsingBridge)plugin).SetDataBridge(new PluginDataBridge(fakerInstance));
                }
            }
        }

        private bool searchForDelegate(Type ChildType, Dictionary<Tuple<Type, string>, Func<Random, Type[], object>> childDictionary, string ChildName, out Func<Random, Type[], object> del)
        {
            del = null;
            foreach (var keyPair in childDictionary.Keys)
            {
                if (keyPair.Item1 == ChildType && keyPair.Item2 == ChildName)
                {
                    childDictionary.TryGetValue(keyPair, out del);
                    return true;
                }
            }
            return false;
        }

        public FakerConfig()
        {
            //Load Plugins
            PluginController pluginController = new PluginController();
            plugins = pluginController.LoadPlugins("Plugins");

            configExpressionDelegate = new Dictionary<Type, Dictionary<Tuple<Type, string>, Func<Random, Type[], object>>>();

            //Setting up default config
            var defaultConfig = new Dictionary<Tuple<Type, string>, Func<Random, Type[], object>>();
            PropertyFactory propertyFactory = new PropertyFactory();
            defaultConfig.Add(new Tuple<Type, string>(typeof(int), null), propertyFactory.GenerateInt);
            defaultConfig.Add(new Tuple<Type, string>(typeof(double), null), propertyFactory.GenerateDouble);
            defaultConfig.Add(new Tuple<Type, string>(typeof(string), null), propertyFactory.GenerateString);
            defaultConfig.Add(new Tuple<Type, string>(typeof(char), null), propertyFactory.GenerateChar);
            defaultConfig.Add(new Tuple<Type, string>(typeof(float), null), propertyFactory.GenerateFloat);
            defaultConfig.Add(new Tuple<Type, string>(typeof(long), null), propertyFactory.GenerateLong);
            defaultConfig.Add(new Tuple<Type, string>(typeof(DateTime), null), propertyFactory.GenerateDate);
            defaultConfig.Add(new Tuple<Type, string>(typeof(TimeSpan), null), propertyFactory.GenerateTime);
            defaultConfig.Add(new Tuple<Type, string>(typeof(Uri), null), propertyFactory.GenerateURI);

            //add plugin methods
            foreach (var plugin in plugins)
            {
                var delegates = pluginController.GetPropertyGenerators(plugin);
                foreach(var del in delegates) {
                    defaultConfig.Add(new Tuple<Type, string>(del.Item1, null), del.Item2);
                }
            }

            configExpressionDelegate.Add(typeof(object), defaultConfig);
        }
    }
}
