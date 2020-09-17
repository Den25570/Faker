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

        public object RequestObject(Type objectType, Type[] genericParams)
        {
            return fakerInstance.GetValue(typeof(object), objectType, null, new Random());
        }
    }
}
