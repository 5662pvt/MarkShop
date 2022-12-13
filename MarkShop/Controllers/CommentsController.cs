using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MarkShop.ViewModels;


namespace MarkShop.Controllers
{
    public class CommentsController : Controller
    {
        QLBanQuanAoDataContext db = new QLBanQuanAoDataContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }
      
        //comment
        public bool Insert(tbComMent entity)
        {
            db.tbComMents.InsertOnSubmit(entity);
            db.SubmitChanges();
            return true;
        }

        public List<tbComMent> ListComment(int parentId, int productId)
        {
            return db.tbComMents.Where(x => x.ParentID == parentId && x.MaSP == productId).ToList();
        }

        public List<CMVM> ListCommentViewModel(int parentId, int productId)
        {
            var model = (from a in db.tbComMents
                         join b in db.KhachHangs
                             on a.MaKH equals b.MaKH
                         where a.ParentID == parentId && a.MaSP == productId
                         select new
                         {
                             ID = a.MaBL,
                             CommentMsg = a.ComentMsg,
                             CommentDate = a.ComentDate,
                             ProductID = a.MaSP,
                             UserID = a.MaKH,
                             FullName = b.TenKH,
                             ParentID = a.ParentID,
                             Rate = a.Rate
                         }).AsEnumerable().Select(x => new CMVM()
                         {
                             MaBL = x.ID,
                             ComentDate = x.CommentDate,
                             ComentMsg = x.CommentMsg,
                             MaKH = (int)x.UserID,
                             MaSP = (int)x.ProductID,
                             Name = x.FullName,
                             Rate = (int)x.Rate
                         });
            return model.OrderByDescending(y => y.MaBL).ToList();
        }
    }
}