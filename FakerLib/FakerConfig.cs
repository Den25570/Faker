using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FakerLib
{
    public class FakerConfig
    {
        private Dictionary<Type, Dictionary<Tuple<Type, string>, Func<Object>>> configExpressionDelegate;

        public void Add<ParentType, ChildType>(Func<Object> del, Expression<Func<ParentType, ChildType>> specifiedField = null)
        {
            Dictionary<Tuple<Type, string>, Func<Object>> targetDictionary;
            if (!configExpressionDelegate.TryGetValue(typeof(ParentType), out targetDictionary))
            {
                targetDictionary = new Dictionary<Tuple<Type, string>, Func<Object>>();
                configExpressionDelegate.Add(typeof(ParentType), targetDictionary);
            }

            string filedName = specifiedField != null ? ((MemberExpression)specifiedField.Body).Member.Name : null;
            targetDictionary.Add(new Tuple<Type, string>(typeof(ChildType), filedName), del);
        }

        public Func<Object> GetExpressionDelegate(Type ParentType, Type ChildType, string ChildName)
        {
            Dictionary<Tuple<Type, string>, Func<Object>> childDictionary;
            Func<Object> del = null;

            if (configExpressionDelegate.TryGetValue(ParentType, out childDictionary))
            {
                del = searchForDelegate(ChildType, childDictionary, ChildName);
            }
            else
            {
                if (configExpressionDelegate.TryGetValue(typeof(object), out childDictionary))
                {
                    del = searchForDelegate(ChildType, childDictionary, ChildName);
                }
                else
                {
                    throw new Exception("Error while trying to receive standart string. Config not setup correctly");
                }
            }
            return del;
        }

        public Func<Object> searchForDelegate(Type ChildType, Dictionary<Tuple<Type, string>, Func<Object>> childDictionary, string ChildName)
        {
            Func<Object> del = null;
            foreach (var keyPair in childDictionary.Keys)
            {
                if (keyPair.Item1 == ChildType)
                {
                    childDictionary.TryGetValue(keyPair, out del);

                    if (keyPair.Item2 != null)
                    {
                        if (keyPair.Item2 == ChildName)
                        {
                            break;
                        }
                    }              
                }
            }
            return del;           
        }

        public FakerConfig()
        {
            configExpressionDelegate = new Dictionary<Type, Dictionary<Tuple<Type, string>, Func<Object>>>();

            //Setting up default config
            var defaultConfig = new Dictionary<Tuple<Type, string>, Func<Object>>();
            defaultConfig.Add(new Tuple<Type, string>(typeof(int), null), PropertyFactory.GenerateInt);

            configExpressionDelegate.Add(typeof(object), defaultConfig);
        }
    }
}
