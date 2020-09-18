using FakerPluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerTimePlugin
{
    [FakerClass]
    public class PropertyCollectionFactory : IFakerClass
    {
        public IFakerDataBridge dataBridge { get; private set; }
        public Dictionary<string, Func<Type, bool>> customTypeComparator { get; private set; }

        private Random rand = new Random();

        public PropertyCollectionFactory()
        {
            customTypeComparator = new Dictionary<string, Func<Type, bool>>();
        }

        public void SetDataBridge(IFakerDataBridge dataBridge)
        {
            this.dataBridge = dataBridge;
        }

        [FakerMethod(typeof(DateTime))]
        public object GenerateDate(Type[] genericTypes)
        {
            DateTime start = new DateTime(1970, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rand.Next(range));
        }

        [FakerMethod(typeof(TimeSpan))]
        public object GenerateTime(Type[] genericTypes)
        {
            TimeSpan start = TimeSpan.FromHours(0);
            TimeSpan end = TimeSpan.FromHours(24);
            int maxMinutes = (int)((end - start).TotalMinutes);
            int minutes = rand.Next(maxMinutes);
            return start.Add(TimeSpan.FromMinutes(minutes));
        }
    }
}
