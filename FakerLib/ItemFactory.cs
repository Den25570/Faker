using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FakerLib
{
    public class ItemFactory
    {
        public T CreateItem<T>() where T : new() {
            return new T();
        }

        public object CreateItem(Type objectType)
        {
            object instance = Activator.CreateInstance(objectType);

            return instance;
        }
    }
}
