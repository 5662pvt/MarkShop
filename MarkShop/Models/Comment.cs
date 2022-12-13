using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarkShop.Models
{
    public partial class Comment
    {
        public long MaBL { get; set; }
        public string ComentMsg { get; set; }
        public Nullable<System.DateTime> ComentDate { get; set; }
        public long MaSP { get; set; }
        public long MaKH{ get; set; }
        public long ParentID { get; set; }
        public int Rate { get; set; }
    }
}