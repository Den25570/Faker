using FakerPluginBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace FakerLib.Plugin
{
    public class PluginDataBridge : IFakerDataBridge
    {
        private Faker fakerInstance;
        public PluginDataBridge(Faker fakerInstance)
        {
            this.fakerInstance = fakerInstance;
        }

        public T RequestObject<T>(Type[] genericParams)
        {
            return (T)fakerInstance.GetValue(typeof(object), typeof(T), null, new Random(), genericParams);
        }
    }
}
