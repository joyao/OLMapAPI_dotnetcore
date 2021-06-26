using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.Models
{
    public class BasicItem
    {
        public class LayerResourceList
        {
            public string ID { get; set; }
            public string GroupID { get; set; }
            public string GroupName { get; set; }
            public string LayerID { get; set; }
            public string LayerOrder { get; set; }
            public string LayerQueryable { get; set; }
            public string LayerTitle { get; set; }
            public string LayerType { get; set; }
            public string DataType { get; set; }
            public string DataURL { get; set; }
            public string LayerVisibleCode { get; set; }
            public string OpenOpacity { get; set; }
        }
    }


}
