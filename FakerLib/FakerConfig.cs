using System;
using System.Collections.Generic;
using System.Text;

namespace FakerLib
{
    public class FakerConfig
    {
        private Dictionary<Type, Dictionary<Tuple<Type, Delegate>, Func<Object>>> configDelegates;

        public void Add<ParentType, ChildType>(Func<Object> del, Delegate specifiedField = null)
        {
            Dictionary<Tuple<Type, Delegate>, Func<Object>> targetDictionary;
            if (!configDelegates.TryGetValue(typeof(ParentType), out targetDictionary))
            {
                targetDictionary = new Dictionary<Tuple<Type, Delegate>, Func<Object>>();
                configDelegates.Add(typeof(ParentType), targetDictionary);
            }

            targetDictionary.Add(new Tuple<Type, Delegate>(typeof(ChildType), specifiedField), del);
        }

        public Func<Object> GetDelegate(Type ParentType, Type ChildType, Delegate specifiedField = null)
        {
            Dictionary<Tuple<Type, Delegate>, Func<Object>> childDictionary;
            Func<Object> del = null;

            if (configDelegates.TryGetValue(ParentType, out childDictionary))
            {
                if (!childDictionary.TryGetValue(new Tuple<Type, Delegate>(ChildType, specifiedField), out del))
                {
                    childDictionary.TryGetValue(new Tuple<Type, Delegate>(ChildType, null), out del);
                }
                return del;
            }
            else
            {
                if (configDelegates.TryGetValue(typeof(object), out childDictionary))
                {
                    if (!childDictionary.TryGetValue(new Tuple<Type, Delegate>(ChildType, specifiedField), out del))
                    {
                        childDictionary.TryGetValue(new Tuple<Type, Delegate>(ChildType, null), out del);
                    }
                    return del;
                }
                else
                {
                    throw new Exception("Error while trying to receive standart delegate. Config not setup correctly");
                }
            }
        }

        private Func<Object> GetDelegateFromParent<ParentType, ChildType>(Delegate specifiedField, Dictionary<Tuple<Type, Delegate>, Func<Object>> childDictionary)
        {
            Func<Object> del = null;
            if (!childDictionary.TryGetValue(new Tuple<Type, Delegate>(typeof(ChildType), specifiedField), out del))
            {
                childDictionary.TryGetValue(new Tuple<Type, Delegate>(typeof(ChildType), null), out del);
            }
            return del;
        }

        public FakerConfig()
        {
            configDelegates = new Dictionary<Type, Dictionary<Tuple<Type, Delegate>, Func<Object>>>();

            var defaultConfig = new Dictionary<Tuple<Type, Delegate>, Func<Object>>();

            defaultConfig.Add(new Tuple<Type, Delegate>(typeof(int), null), PropertyFactory.GenerateInt);

            configDelegates.Add(typeof(object), defaultConfig);
        }
    }
}
