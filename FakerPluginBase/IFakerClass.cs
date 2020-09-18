using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerPluginBase
{
    public interface IFakerClass
    {       
        IFakerDataBridge dataBridge { get; }

        Dictionary<string, Func<Type, bool>> customTypeComparator { get; }

        void SetDataBridge(IFakerDataBridge dataBridge);
    }
}
