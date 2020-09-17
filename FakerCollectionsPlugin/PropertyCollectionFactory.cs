using FakerPluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerCollection
{
    [FakerClass]
    public class PropertyCollectionFactory
    {
        private IFakerDataBridge dataBridge;

        [FakerMethod(typeof(ICollection<>))]
        public object GenerateCollection(Random rand, List<Type> genericTypes)
        {
            throw new NotImplementedException();
        }
    }
}
