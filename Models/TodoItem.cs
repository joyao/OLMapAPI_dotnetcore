using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
