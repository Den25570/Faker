using System;
using System.Collections.Generic;
using System.Text;

namespace FakerLib
{
    public class FakerConfig
    {
        private Dictionary<Type, Dictionary<Tuple<Type, Func<Type, Type>>, Func<Object>>> configDelegates;

        public void Add<ParentType, ChildType>(Func<Object> del, Func<Type, Type> specifiedField = null)
        {
            Dictionary<Tuple<Type, Func<Type, Type>>, Func<Object>> targetDictionary;
            if (!configDelegates.TryGetValue(typeof(ParentType), out targetDictionary))
            {
                targetDictionary = new Dictionary<Tuple<Type, Func<Type, Type>>, Func<Object>>();
                configDelegates.Add(typeof(ParentType), targetDictionary);
            }

            targetDictionary.Add(new Tuple<Type, Func<Type, Type>>(typeof(ChildType), specifiedField), del);
        }

        public bool TryGetDelegate<ParentType, ChildType>(out Func<Object> del, Func<Type, Type> specifiedField = null)
        {
            Dictionary<Tuple<Type, Func<Type, Type>>, Func<Object>> childDictionary;

            if (configDelegates.TryGetValue(typeof(ParentType), out childDictionary))
            {
                return childDictionary.TryGetValue(new Tuple<Type, Func<Type, Type>>(typeof(ChildType), specifiedField), out del);
            }
            else
            {
                if (configDelegates.TryGetValue(typeof(object), out childDictionary))
                {
                    return childDictionary.TryGetValue(new Tuple<Type, Func<Type, Type>>(typeof(ChildType), specifiedField), out del);
                }
                else
                {
                    throw new Exception("Error while trying to receive standart delegate. Config not setup correctly");
                }
            }
        }

        public FakerConfig()
        {
            configDelegates = new Dictionary<Type, Dictionary<Tuple<Type, Func<Type, Type>>, Func<Object>>>();
        }
    }
}
