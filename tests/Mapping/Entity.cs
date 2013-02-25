using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests.Mapping
{
    public class Entity
    {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public SubEntity SubEntity { get; set; }
    }
}
