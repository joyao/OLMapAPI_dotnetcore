using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.Models.auth
{
    public class loginInfo
    {
        public string userid { get; set; }
        public string password { get; set; }
    }

    /// <summary>
    /// 權限設定input
    /// </summary> 
    public class tokenObj
    {
        public string status { get; set; }
        public string token { get; set; }
    }
}
