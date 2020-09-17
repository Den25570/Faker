using FakerPluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerCollection
{
    [FakerClass]
    public class PropertyCollectionFactory : IUsingBridge
    {
        public IFakerDataBridge dataBridge { get; private set; }

        private int minCollectionSize = 2;
        private int maxCollectionSize = 8;

        public void SetDataBridge(IFakerDataBridge dataBridge)
        {
            this.dataBridge = dataBridge;
        }

        [FakerMethod(typeof(List<>))]
        public object GenerateCollection(Random rand, Type[] genericTypes)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(genericTypes);

            var instance = (List<object>)Activator.CreateInstance(constructedListType);

            int length = rand.Next(minCollectionSize, maxCollectionSize);

            if (dataBridge == null)
            {
                throw new Exception("Data bridge not set");
            }

            for(int i = 0; i< length; i++)
            {
                instance.Add(dataBridge.RequestObject(genericTypes[0], null));
            }

            return instance;
        }
    }
}
