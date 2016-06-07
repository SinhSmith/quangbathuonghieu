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

namespace Portal.Site.Controllers
{
    public class CompanyController : Controller
    {
        private PortalEntities db = new PortalEntities();

        // GET: List
        public ActionResult List(Guid? trade, Guid? city, string keyword, int page = 1)
        {
            var companies = db.Companies.Where(x => (string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword))
                && (trade == null || x.TradeId == trade.Value)
                && (city == null || x.City == city.Value))
                .OrderByDescending(x => x.CreatedDate);
            return PartialView(companies.ToList().ToPagedList(page, 10));
        }

        // GET: ListCompanyForHomePage
        public ActionResult ListCompanyForHomePage(int page = 1)
        {
            var companies = db.Companies.OrderByDescending(x => x.CreatedDate);
            return PartialView(companies.ToList().ToPagedList(page, 10));
        }

        // GET: ListCompanySameJob
        public ActionResult ListCompanySameJob(Guid id)
        {
            var company = db.Companies.Find(id);
            var companies = db.Companies.Where(x => x.Id != id && x.TradeId == company.TradeId).OrderByDescending(x => x.CreatedDate).Take(5);
            return PartialView(companies.ToList());
        }

        // GET: Company
        public ActionResult Index(string keyword, int page = 1)
        {
            var companies = db.Companies.Where(x => string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword)).OrderByDescending(x => x.CreatedDate);
            return View(companies.ToList().ToPagedList(page, 10));
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
            ViewBag.ImageCover = new SelectList(db.Images, "Id", "FileName");
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Ranking,Name,ImageCover,Email,Address,City,Phone,Director,Website,Description,CountView,Status,CreatedBy,CreatedDate,UpdatedBy,UpdateDate")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.Id = Guid.NewGuid();
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ImageCover = new SelectList(db.Images, "Id", "FileName", company.ImageCover);
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
            ViewBag.ImageCover = new SelectList(db.Images, "Id", "FileName", company.ImageCover);
            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ranking,Name,ImageCover,Email,Address,City,Phone,Director,Website,Description,CountView,Status,CreatedBy,CreatedDate,UpdatedBy,UpdateDate")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ImageCover = new SelectList(db.Images, "Id", "FileName", company.ImageCover);
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
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Index");
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
