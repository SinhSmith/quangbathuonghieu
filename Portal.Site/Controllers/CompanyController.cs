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
    public class CompanyController : Controller
    {
        private PortalEntities db = new PortalEntities();

        // GET: List
        public ActionResult List(Guid? trade, Guid? city, string keyword, int page = 1)
        {
            var companies = db.Companies.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active && ((string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword))
                && (trade == null || x.TradeId == trade.Value)
                && (city == null || x.City == city.Value)))
                .OrderByDescending(x => x.CreatedDate);
            return PartialView(companies.ToList().ToPagedList(page, 10));
        }

        // GET: ListCompanyForHomePage
        public ActionResult ListCompanyForHomePage(int page = 1)
        {
            var companies = db.Companies.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active).OrderByDescending(x => x.CreatedDate);
            return PartialView(companies.ToList().ToPagedList(page, 50));
        }

        // GET: ListCompanySameJob
        public ActionResult ListCompanySameJob(Guid id)
        {
            var company = db.Companies.Find(id);
            var companies = db.Companies.Where(x => x.Id != id && x.TradeId == company.TradeId && x.Status == (int)Portal.Core.Util.Define.Status.Active).OrderByDescending(x => x.CreatedDate).Take(5);
            return PartialView(companies.ToList());
        }

        // GET: Company
        [Authorize(Roles = "Administrator")]
        public ActionResult Index(string keyword)
        {
            var companies = db.Companies.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active && (string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword))).OrderByDescending(x => x.CreatedDate);
            return View(companies.ToList());
        }

        // GET: Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.FirstOrDefault(x => x.Ranking == id.Value);
            if (company == null)
            {
                return HttpNotFound();
            }

            company.CountView++;
            db.SaveChanges();

            return View(company);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            var cities = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.City);
            ViewBag.City = new SelectList(cities, "Id", "Name");

            var trades = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.Trades);
            ViewBag.TradeId = new SelectList(trades, "Id", "Name");

            return View();
        }

        // POST: Company/Create
        [HttpPost, ValidateInput(false)] // Hoac [AllowHtml] Annotation
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Email,Address,AddressForMap,City,TradeId,Phone,Director,Website,Description")] Company company, HttpPostedFileBase uploadFile)
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

                company.Id = Guid.NewGuid();
                if (imageCover != Guid.Empty)
                    company.ImageCover = imageCover;
                company.Status = (int)Portal.Core.Util.Define.Status.Active;
                company.CreatedDate = DateTime.Now;
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Company/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }

            var cities = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.City);
            ViewBag.City = new SelectList(cities, "Id", "Name", company.City);

            var trades = Core.Service.CategoryService.GetCategoryByType((int)Core.Service.BaseService.CategoryType.Trades);
            ViewBag.TradeId = new SelectList(trades, "Id", "Name", company.TradeId);

            return View(company);
        }

        // POST: Company/Edit/5
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Address,AddressForMap,City,TradeId,Phone,Director,Website,Description")] Company company, HttpPostedFileBase uploadFile)
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

                var companyOld = db.Companies.Find(company.Id);
                companyOld.Name = company.Name;
                companyOld.Email = company.Email;
                companyOld.Address = company.Address;
                companyOld.AddressForMap = company.AddressForMap;
                companyOld.City = company.City;
                companyOld.TradeId = company.TradeId;
                companyOld.Phone = company.Phone;
                companyOld.Director = company.Director;
                companyOld.Website = company.Website;
                companyOld.Description = company.Description;
                if (imageCover != Guid.Empty)
                    companyOld.ImageCover = imageCover;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Company/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Company company = db.Companies.Find(id);
            //db.Companies.Remove(company);
            company.Status = (int)Portal.Core.Util.Define.Status.DeActive;
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
