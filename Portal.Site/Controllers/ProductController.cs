using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal.Core.Database;
using PagedList;
using System.IO;

namespace Portal.Site.Controllers
{
    public class ProductController : Controller
    {
        private PortalEntities db = new PortalEntities();

        // GET: List
        public ActionResult List(Guid? trade, Guid? city, string keyword, int page = 1)
        {
            var products = db.Products.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active && ((string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword))
                && (trade == null || x.TradeId == trade.Value)
                && (city == null || x.City == city.Value)))
                .OrderByDescending(x => x.CreatedDate);
            return PartialView(products.ToList().ToPagedList(page, 10));
        }

        // GET: ListProductForHomePage
        public ActionResult ListProductForHomePage(string keyword, int page = 1)
        {
            var products = db.Products.Where(x => string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword)).OrderByDescending(x => x.CreatedDate);
            return PartialView(products.ToList().ToPagedList(page, 10));
        }

        // GET: ListProductSameJob
        public ActionResult ListProductSameJob(Guid id)
        {
            var product = db.Products.Find(id);
            var products = db.Products.Where(x => x.Id != id && x.TradeId == product.TradeId && x.Status == (int)Portal.Core.Util.Define.Status.Active).OrderByDescending(x => x.CreatedDate).Take(5);
            return PartialView(products.ToList());
        }

        // GET: Product
        [Authorize(Roles = "Administrator")]
        public ActionResult Index(string keyword)
        {
            var products = db.Products.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active && (string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword))).OrderByDescending(x => x.CreatedDate);
            return View(products.ToList());
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.FirstOrDefault(x => x.Ranking == id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }

            product.CountView++;
            db.SaveChanges();

            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            var cities = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.City);
            ViewBag.City = new SelectList(cities, "Id", "Name");

            var trades = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.Trades);
            ViewBag.TradeId = new SelectList(trades, "Id", "Name");

            return View();
        }

        // POST: Product/Create
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Price,Address,AddressForMap,City,TradeId,Phone,Description")] Product product, HttpPostedFileBase uploadFile)
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

                        string physicalDirectory = Server.MapPath("~/Uploads/Product/" + img.Id);
                        if (!Directory.Exists(physicalDirectory))
                            Directory.CreateDirectory(physicalDirectory);
                        uploadFile.SaveAs(physicalDirectory + "/" + img.FileName);

                        img.FilePath = "/Uploads/Product/" + img.Id;
                        db.Images.Add(img);

                        imageCover = img.Id;
                    }
                }

                product.Id = Guid.NewGuid();
                if (imageCover != Guid.Empty)
                    product.ImageCover = imageCover;
                product.Status = (int)Portal.Core.Util.Define.Status.Active;
                product.CreatedDate = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var cities = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.City);
            ViewBag.City = new SelectList(cities, "Id", "Name", product.City);

            var trades = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.Trades);
            ViewBag.TradeId = new SelectList(trades, "Id", "Name", product.TradeId);

            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Address,AddressForMap,City,TradeId,Phone,Description")] Product product, HttpPostedFileBase uploadFile)
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

                        string physicalDirectory = Server.MapPath("~/Uploads/Product/" + img.Id);
                        if (!Directory.Exists(physicalDirectory))
                            Directory.CreateDirectory(physicalDirectory);
                        uploadFile.SaveAs(physicalDirectory + "/" + img.FileName);

                        img.FilePath = "/Uploads/Product/" + img.Id;
                        db.Images.Add(img);

                        imageCover = img.Id;
                    }
                }

                var productOld = db.Products.Find(product.Id);
                productOld.Name = product.Name;
                productOld.Price = product.Price;
                productOld.Address = product.Address;
                productOld.AddressForMap = product.AddressForMap;
                productOld.City = product.City;
                productOld.TradeId = product.TradeId;
                productOld.Phone = product.Phone;
                productOld.Description = product.Description;
                if (imageCover != Guid.Empty)
                    productOld.ImageCover = imageCover;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            product.Status = (int)Portal.Core.Util.Define.Status.DeActive;
            //db.Products.Remove(product);
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
