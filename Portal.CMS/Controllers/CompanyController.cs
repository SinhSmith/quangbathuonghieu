using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal.Core.Database;
using Portal.Core.Service;

namespace Portal.CMS.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private PortalEntities db = new PortalEntities();

        #region Administrator
        // GET: Company/List
        [Authorize(Roles = "Administrator")]
        public ActionResult List(string keyword, int pageNumber = 1, int pageSize = 15)
        {
            if (User.IsInRole("Administrator"))
            {
                var ListCompany = db.Companies.Where(x => (string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword))).OrderBy(x => x.Name);
                return View(ListCompany.ToList().ToPagedList(pageNumber, pageSize));
            }
            return RedirectToAction("Index", "Job");
        }

        // GET: Company/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            if (!User.IsInRole("Administrator"))
            {
                return RedirectToAction("Index", "Job");
            }

            var numberOfEmployees = CategoryService.GetNumberOfEmployees();
            ViewBag.NumberOfEmployees = new SelectList(numberOfEmployees, "Id", "Name");

            return View();
        }

        // POST: Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Exclude = "Id")]Company model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                db.Companies.Add(model);
                db.SaveChanges();

                return RedirectToAction("List");
            }

            return View(model);
        }

        // GET: Company/Edit
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Guid? id)
        {
            if (!User.IsInRole("Administrator"))
            {
                return RedirectToAction("Index", "Job");
            }
            if (id == null)
            {
                return RedirectToAction("Index", "Job");
            }

            var company = CompanyService.GetCompany(id.Value);
            if (company == null)
            {
                return RedirectToAction("Index", "Job");
            }
            var numberOfEmployees = CategoryService.GetNumberOfEmployees();
            ViewBag.NumberOfEmployees = new SelectList(numberOfEmployees, "Id", "Name", company.NumberOfEmployees);

            return View(company);
        }

        // POST: Company/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Company model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("List");
            }
            return View(model);
        }

        // POST: Company/Delete
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Job");
            }
            var company = db.Companies.Find(id.Value);
            if (company == null)
            {
                return RedirectToAction("Index", "Job");
            }
            db.Entry(company).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();

            return Json(new { success = true });
        }
        #endregion

        #region Users
        // GET: Company/Details
        public ActionResult Details()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var company = CompanyService.GetCompanyByUserId(userId);
            if (company == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index", "Job");
            }
            return View(company);
        }

        // GET: Company/EditCompanyInfo
        public ActionResult EditCompanyInfo()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var company = CompanyService.GetCompanyByUserId(userId);
            if (company == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index", "Job");
            }

            var numberOfEmployees = CategoryService.GetNumberOfEmployees();
            ViewBag.NumberOfEmployees = new SelectList(numberOfEmployees, "Id", "Name", company.NumberOfEmployees);

            return PartialView(company);
        }

        // POST: Job/EditCompanyInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompanyInfo([Bind(Include = "Id, Name, NumberOfEmployees, Profile, Address, Website, Phone, Fax")]Company model)
        {
            if (ModelState.IsValid)
            {
                var company = db.Companies.Find(model.Id);
                company.Name = model.Name;
                company.NumberOfEmployees = model.NumberOfEmployees;
                company.Profile = model.Profile;
                company.Address = model.Address;
                company.Website = model.Website;
                company.Phone = model.Phone;
                company.Fax = model.Fax;
                db.SaveChanges();

                return Json(new { success = true });
            }
            return View(model);
        }

        // GET: Company/EditCompanyContact
        public ActionResult EditCompanyContact()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var company = CompanyService.GetCompanyByUserId(userId);
            if (company == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index", "Job");
            }
            return PartialView(company);
        }

        // POST: Job/EditCompanyContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCompanyContact([Bind(Include = "Id, ContactName, ContactLevelJob, ContactPhone, ContactEmail")]Company model)
        {
            if (ModelState.IsValid)
            {
                var company = db.Companies.Find(model.Id);
                company.ContactName = model.ContactName;
                company.ContactLevelJob = model.ContactLevelJob;
                company.ContactPhone = model.ContactPhone;
                company.ContactEmail = model.ContactEmail;
                db.SaveChanges();

                return Json(new { success = true });
            }
            return View(model);
        }
        #endregion

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