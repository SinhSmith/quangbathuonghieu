using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal.Core.Database;
using System.IO;

namespace Portal.Site.Controllers
{
    public class BannerController : Controller
    {
        private PortalEntities db = new PortalEntities();

        // GET: /Banner/
        public ActionResult Index()
        {
            var banners = db.Banners.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active).OrderBy(x => x.SortOrder);
            return View(banners.ToList());
        }

        // GET: /Banner/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // GET: /Banner/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Banner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,SortOrder")] Banner banner, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                Guid? imageId = Guid.Empty;
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string inputFilePath = uploadFile.FileName.ToLower();
                    string[] ImageList = { ".gif", ".jpg", ".png" };
                    if (ImageList.Contains(Path.GetExtension(inputFilePath)))
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var img = new Image()
                        {
                            Id = Guid.NewGuid(),
                            FileName = fileName
                        };

                        string physicalDirectory = Server.MapPath("~/Uploads/Banner/" + img.Id);
                        if (!Directory.Exists(physicalDirectory))
                            Directory.CreateDirectory(physicalDirectory);
                        uploadFile.SaveAs(physicalDirectory + "/" + img.FileName);

                        img.FilePath = "/Uploads/Banner/" + img.Id;
                        db.Images.Add(img);

                        imageId = img.Id;
                    }
                }

                banner.Id = Guid.NewGuid();
                banner.ImageId = imageId;
                banner.Status = (int)Portal.Core.Util.Define.Status.Active;
                db.Banners.Add(banner);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(banner);
        }

        // GET: /Banner/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // POST: /Banner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,SortOrder")] Banner banner, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                Guid? imageCover = Guid.Empty;
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string inputFilePath = uploadFile.FileName.ToLower();
                    string[] ImageList = { ".gif", ".jpg", ".png" };
                    if (ImageList.Contains(Path.GetExtension(inputFilePath)))
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var img = new Image()
                        {
                            Id = Guid.NewGuid(),
                            FileName = fileName
                        };

                        string physicalDirectory = Server.MapPath("~/Uploads/Company/" + img.Id);
                        if (!Directory.Exists(physicalDirectory))
                            Directory.CreateDirectory(physicalDirectory);
                        uploadFile.SaveAs(physicalDirectory + "/" + img.FileName);

                        img.FilePath = "/Uploads/Company/" + img.Id;
                        db.Images.Add(img);

                        imageCover = img.Id;
                    }
                }

                var bannerOld = db.Banners.Find(banner.Id);
                bannerOld.Name = banner.Name;
                bannerOld.SortOrder = banner.SortOrder;
                if (imageCover != Guid.Empty)
                    bannerOld.ImageId = imageCover;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(banner);
        }

        // GET: /Banner/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
        }

        // POST: /Banner/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Banner banner = db.Banners.Find(id);
            banner.Status = (int)Portal.Core.Util.Define.Status.Delete;
            //db.Banners.Remove(banner);
            db.SaveChanges();
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
