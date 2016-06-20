using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal.Core.Database;
using Microsoft.AspNet.Identity;
using Portal.Site.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Portal.Site.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ProfileController()
        {
        }

        public ProfileController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private PortalEntities db = new PortalEntities();

        // GET: /Profie/
        public ActionResult Index()
        {
            return View(db.Profiles.Where(x => x.Status == (int)Portal.Core.Util.Define.Status.Active).ToList());
        }

        // GET: /Profie/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: /Profie/Create
        public ActionResult Create()
        {
            var city = Portal.Core.Service.CategoryService.GetCities();
            ViewBag.City = new SelectList(city, "Id", "Name");

            return View();
        }

        // POST: /Profie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Email,Password,Address,City,District,Phone")] Profile model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    // Profile
                    using (var db = new Portal.Core.Database.PortalEntities())
                    {
                        var profile = new Portal.Core.Database.Profile
                        {
                            Id = Guid.Parse(user.Id),
                            Email = model.Email,
                            Password = model.Password,
                            Address = model.Address,
                            City = model.City,
                            Phone = model.Phone,
                            Status = (int)Portal.Core.Util.Define.Status.Active
                        };
                        db.Profiles.Add(profile);

                        var company = new Portal.Core.Database.Company
                        {
                            Id = Guid.NewGuid(),
                            Name = "",
                            Address = model.Address,
                            AddressForMap = model.Address,
                            City = model.City,
                            Phone = model.Phone,
                            Status = (int)Portal.Core.Util.Define.Status.Pending,
                            CreatedBy = profile.Id,
                            CreatedDate = DateTime.Now,
                            CountView = new Random().Next(200, 1000)
                        };
                        db.Companies.Add(company);
                        db.SaveChanges();
                    }

                    return Json(new { success = true });
                }
                else
                {
                    string errors = "";
                    foreach (var error in result.Errors)
                    {
                        errors += error;
                    }
                    return Json(new { success = false, message = errors });
                }
            }

            return View();
        }

        // GET: /Profie/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: /Profie/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password,Address,City,District,Phone")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profile);
        }

        // GET: /Profie/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: /Profie/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            //Lock
            var user = db.AspNetUsers.FirstOrDefault(x => x.Id == id.ToString());
            user.LockoutEndDateUtc = DateTime.Parse("1/1/9999");

            Profile profile = db.Profiles.Find(id);
            //db.Profiles.Remove(profile);
            profile.Status = (int)Portal.Core.Util.Define.Status.Delete;
            db.SaveChanges();

            //return RedirectToAction("Index");
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
