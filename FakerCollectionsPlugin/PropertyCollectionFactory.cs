using FakerPluginBase;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerCollection
{
    [FakerClass]
    public class PropertyCollectionFactory : IFakerClass
    {
        public IFakerDataBridge dataBridge { get; private set; }
        public Dictionary<string, Func<Type, bool>> customTypeComparator {get; private set; }

        private int minCollectionSize = 2;
        private int maxCollectionSize = 8;
        private Random rand = new Random();

        public PropertyCollectionFactory()
        {
            customTypeComparator = new Dictionary<string, Func<Type, bool>>();
            customTypeComparator.Add("GenerateArray", type => type.IsArray);
            customTypeComparator.Add("GenerateList", type => type.Name == typeof(List<>).Name);

        }

        public void SetDataBridge(IFakerDataBridge dataBridge)
        {
            this.dataBridge = dataBridge;
        }

        [FakerMethod(typeof(List<>))]
        public object GenerateList(Type[] genericTypes)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(genericTypes);

            IList instance = (IList)Activator.CreateInstance(constructedListType);

            int length = rand.Next(minCollectionSize, maxCollectionSize);

            if (dataBridge == null)
            {
                throw new Exception("Data bridge not set");
            }

            for (int i = 0; i< length; i++)
            {
                object item = dataBridge.RequestObject(genericTypes[0], null);

                instance.Add(item);
            }

            return instance;
        }

        [FakerMethod(typeof(Array))]
        public object GenerateArray(Type[] genericTypes)
        {
            var arrayType = genericTypes[0].MakeArrayType();
            int length = rand.Next(minCollectionSize, maxCollectionSize);

            Array instance = (Array)Activator.CreateInstance(arrayType, new object[] { length });       

            if (dataBridge == null)
            {
                throw new Exception("Data bridge not set");
            }

            for (int i = 0; i < length; i++)
            {
                object item = dataBridge.RequestObject(genericTypes[0], null);
                instance.SetValue(item, i);
            }

            return instance;
        }
    }
}
