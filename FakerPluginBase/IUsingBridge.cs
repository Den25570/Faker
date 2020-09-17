using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerPluginBase
{
    public interface IUsingBridge
    {       
        IFakerDataBridge dataBridge { get; }

        void SetDataBridge(IFakerDataBridge dataBridge);
    }
}
