using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarkShop.ViewModels
{
    public class CMVM
    {
        public int MaBL { get; set; }
        public string ComentMsg { get; set; }
        public Nullable<System.DateTime> ComentDate { get; set; }
        public int MaSP { get; set; }
        public int MaKH { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; }
    }
}