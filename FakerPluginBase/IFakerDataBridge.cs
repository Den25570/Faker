using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerPluginBase
{
    public interface IFakerDataBridge
    {
        T RequestObject<T>();
    }
}
