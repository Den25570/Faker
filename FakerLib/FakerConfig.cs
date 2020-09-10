using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FakerLib
{
    public class FakerConfig
    {
        private Dictionary<Type, Dictionary<Tuple<Type, string>, Func<Random, object>>> configExpressionDelegate;

        public void Add<ParentType, ChildType>(Func<Random, object> del, Expression<Func<ParentType, ChildType>> specifiedField = null)
        {
            Dictionary<Tuple<Type, string>, Func<Random, object>> targetDictionary;
            if (!configExpressionDelegate.TryGetValue(typeof(ParentType), out targetDictionary))
            {
                targetDictionary = new Dictionary<Tuple<Type, string>, Func<Random, object>>();
                configExpressionDelegate.Add(typeof(ParentType), targetDictionary);
            }

            string filedName = specifiedField != null ? ((MemberExpression)specifiedField.Body).Member.Name : null;
            targetDictionary.Add(new Tuple<Type, string>(typeof(ChildType), filedName), del);
        }

        public Func<Random, object> GetExpressionDelegate(Type ParentType, Type ChildType, string ChildName)
        {
            Dictionary<Tuple<Type, string>, Func<Random, object>> childDictionary;
            Func<Random, object> del = null;

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

        public Func<Random, object> searchForDelegate(Type ChildType, Dictionary<Tuple<Type, string>, Func<Random, object>> childDictionary, string ChildName)
        {
            Func<Random, object> del = null;
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
            configExpressionDelegate = new Dictionary<Type, Dictionary<Tuple<Type, string>, Func<Random, object>>>();

            //Setting up default config
            var defaultConfig = new Dictionary<Tuple<Type, string>, Func<Random, object>>();

            defaultConfig.Add(new Tuple<Type, string>(typeof(int), null), PropertyFactory.GenerateInt);
            defaultConfig.Add(new Tuple<Type, string>(typeof(double), null), PropertyFactory.GenerateDouble);
            defaultConfig.Add(new Tuple<Type, string>(typeof(string), null), PropertyFactory.GenerateString);
            defaultConfig.Add(new Tuple<Type, string>(typeof(char), null), PropertyFactory.GenerateChar);
            defaultConfig.Add(new Tuple<Type, string>(typeof(float), null), PropertyFactory.GenerateFloat);
            defaultConfig.Add(new Tuple<Type, string>(typeof(long), null), PropertyFactory.GenerateLong);
            defaultConfig.Add(new Tuple<Type, string>(typeof(DateTime), null), PropertyFactory.GenerateDate);
            defaultConfig.Add(new Tuple<Type, string>(typeof(TimeSpan), null), PropertyFactory.GenerateTime);
            defaultConfig.Add(new Tuple<Type, string>(typeof(Uri), null), PropertyFactory.GenerateURI);

            configExpressionDelegate.Add(typeof(object), defaultConfig);
        }
    }
}
