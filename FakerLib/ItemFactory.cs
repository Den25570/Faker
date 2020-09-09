using System;
using System.Collections.Generic;
using System.Text;

namespace FakerLib
{
    public class ItemFactory
    {
        public T CreateItem<T>() where T : new() {
            return new T();
        }     
    }
}
