using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests.Mapping
{
    public class Model
    {
            public Guid id { get; set; }
            public string name { get; set; }
            public string subEntityName { get; set; }
            public Guid subEntityId { get; set; }
    }
}
