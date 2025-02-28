using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI
{
    public class DIRegistration
    {
        public Func<DIContainer, object> Factory { get; set; }
        public bool IsSingleton { get; set; }
        public object Instance { get; set; }
    }
}
