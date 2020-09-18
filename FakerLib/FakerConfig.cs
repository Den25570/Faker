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
        private Dictionary<Type, List<Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>>> configExpressionDelegate;
        private List<IFakerClass> plugins;

        public void Add<ParentType, ChildType>(Func<Type[], object> del, Expression<Func<ParentType, ChildType>> specifiedField = null)
        {
            List<Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>> targetDataList;
            if (!configExpressionDelegate.TryGetValue(typeof(ParentType), out targetDataList))
            {
                targetDataList = new List<Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>>();
                configExpressionDelegate.Add(typeof(ParentType), targetDataList);
            }

            string filedName = specifiedField != null ? ((MemberExpression)specifiedField.Body).Member.Name : null;
            targetDataList.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(ChildType), filedName, del, null));
        }

        public Func<Type[], object> GetExpressionDelegate(Type ParentType, Type ChildType, string ChildName, bool caseSensetive = true)
        {
            List<Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>> childDictionary;
            Func<Type[], object> del = null;

            if (configExpressionDelegate.TryGetValue(ParentType, out childDictionary) && searchForDelegate(ChildType, childDictionary, ChildName, out del, caseSensetive)){}
            else
            {
                if (configExpressionDelegate.TryGetValue(ParentType, out childDictionary) && searchForDelegate(ChildType, childDictionary, null, out del, caseSensetive)){}
                else
                {
                    if (configExpressionDelegate.TryGetValue(typeof(object), out childDictionary) && searchForDelegate(ChildType, childDictionary, null, out del, caseSensetive)) {}
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
                if (plugin.GetType().GetInterfaces().Contains(typeof(IFakerClass)))
                {
                    ((IFakerClass)plugin).SetDataBridge(new PluginDataBridge(fakerInstance));
                }
            }
        }

        private bool searchForDelegate(Type ChildType, List<Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>> childDataList, string ChildName, out Func<Type[], object> del, bool caseSensetive = true)
        {
            del = null;
            foreach (var delData in childDataList)
            {
                string name1 = caseSensetive ? delData.Item2 : delData.Item2 != null ? delData.Item2.ToLower() : null;
                string name2 = caseSensetive ? ChildName : ChildName != null ? ChildName.ToLower() : null;

                if ((delData.Item4 == null ? (delData.Item1.Name == ChildType.Name) : delData.Item4.Invoke(ChildType)) && (name1 == name2))
                {
                    del = delData.Item3;
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

            configExpressionDelegate = new Dictionary<Type, List<Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>>>();

            //Setting up default config
            var defaultConfig = new List<Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>>();
            PropertyFactory propertyFactory = new PropertyFactory();
            defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(int), null, propertyFactory.GenerateInt, null));
            defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(double), null, propertyFactory.GenerateDouble, null));
            defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(string), null, propertyFactory.GenerateString, null));
            defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(char), null, propertyFactory.GenerateChar, null));
            defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(float), null, propertyFactory.GenerateFloat, null));
            defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(long), null, propertyFactory.GenerateLong, null));
            defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(typeof(Uri), null, propertyFactory.GenerateURI, null));

            //add plugin methods
            foreach (var plugin in plugins)
            {
                var delegates = pluginController.GetPropertyGenerators(plugin);
                foreach(var del in delegates) {
                    Func<Type, bool> specComparator = null;
                    plugin.customTypeComparator.TryGetValue(del.Item2.Method.Name, out specComparator);
                    defaultConfig.Add(new Tuple<Type, string, Func<Type[], object>, Func<Type, bool>>(del.Item1, null, del.Item2, specComparator));
                }
            }

            configExpressionDelegate.Add(typeof(object), defaultConfig);
        }
    }
}
