using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.Models.auth
{
    public class apiUserModels
    {
    }
    public class apiUserObj
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string unit { get; set; }
        public string note { get; set; }
        public string password { get; set; }
        public string lockYN { get; set; }
    }

    public class insertApiUserObj
    {
        private string _lock = "0";
        public string userId { get; set; }
        public string unit { get; set; }
        public string note { get; set; }
        public string password { get; set; }
        public string lockYN { get => _lock; set => _lock = value; }
    }


    /// <summary>
    /// removeApiUserObj
    /// </summary> 
    public class removeApiUserObj
    {
        public string id { get; set; }

    }

    /// <summary>
    /// lockApiUserObj
    /// </summary> 
    public class lockApiUserObj
    {
        public string id { get; set; }

    }

}
