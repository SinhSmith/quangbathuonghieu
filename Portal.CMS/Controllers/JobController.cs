using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal.Core.Database;
using Portal.Core.Service;
using Portal.Core.Util;
using Zody.Models;

namespace Portal.CMS.Controllers
{
    [Authorize]
    public class JobController : Controller
    {
        private PortalEntities db = new PortalEntities();

        // GET: Job/Index
        [AllowAnonymous]
        public ActionResult Index(string Keyword, string City, string JobCategory, string JobLevel, int page = 1)
        {
            // City
            var cities = CategoryService.GetCity(CategoryService.VietNam);
            List<SelectListItem> cityList = new List<SelectListItem>();
            cityList.AddRange(new SelectList(cities, "Name", "Name"));
            cityList.Insert(0, new SelectListItem { Text = "Chọn địa điểm", Value = Guid.Empty.ToString() });
            ViewBag.City = cityList;

            // Job Category
            var jobCategories = CategoryService.GetJobCategory();
            List<SelectListItem> jobCategoriesList = new List<SelectListItem>();
            jobCategoriesList.AddRange(new SelectList(jobCategories, "Name", "Name"));
            jobCategoriesList.Insert(0, new SelectListItem { Text = "Chọn ngành nghề", Value = Guid.Empty.ToString() });
            ViewBag.JobCategory = jobCategoriesList;

            // Job Level
            var jobLevels = CategoryService.GetJobLevel();
            List<SelectListItem> jobLevelsList = new List<SelectListItem>();
            jobLevelsList.AddRange(new SelectList(jobLevels, "Name", "Name"));
            jobLevelsList.Insert(0, new SelectListItem { Text = "Chọn cấp bậc", Value = Guid.Empty.ToString() });
            ViewBag.JobLevel = jobLevelsList;

            var jobs = db.Jobs.Where(x => x.Status == (int)Common.Util.Define.Status.Active && ((string.IsNullOrEmpty(Keyword) || x.JobTitle.Contains(Keyword))
                && (string.IsNullOrEmpty(City) || x.JobWorkPlaceString.Contains(City))
                && (string.IsNullOrEmpty(JobCategory) || x.JobCategorieString.Contains(JobCategory))
                && (string.IsNullOrEmpty(JobLevel) || x.JobLevelString.Contains(JobLevel)))).OrderByDescending(x => x.TimeCreate);

            return View(jobs.ToList().ToPagedList(page, 15));
        }

        // GET: Resumes/Details/5
        [AllowAnonymous]
        public ActionResult Details(string title, int? id)
        {
            if (string.IsNullOrEmpty(title) || id == null)
            {
                return RedirectToAction("Index", "Job");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var job = db.Jobs.Include("Company").FirstOrDefault(x => x.Id == id);
            if (job == null)
            {
                //return RedirectToAction("Index", "Job");
                return HttpNotFound();
            }
            if (!job.JobUrl.Equals(title))
            {
                //return RedirectToAction("Index", "Job");
                return HttpNotFound();
            }

            job.ViewCount++;
            db.SaveChanges();


            ViewBag.MetaTitle = "Tuyển dụng: " + job.JobTitle;
            ViewBag.MetaDescription = Common.Util.Common.DescriptionTrip(Common.Util.Common.StripHTML(job.JobDescription));
            ViewBag.url = Define.RootUrl + job.JobUrl + '_' + job.Id;

            ViewBag.Applied = false;
            
            if (User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                JobApply jobApply = JobApplyService.Get(job.Id, userId);
                if (jobApply != null)
                    ViewBag.Applied = true;
                else ViewBag.Applied = false;
            }
            
            return View(job);
        }

        // GET: Job/List - Get all jobs that posted by current user
        public ActionResult List(int pageNumber = 1, int pageSize = 15)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            if (userId != null)
            {
                if (User.IsInRole("Administrator"))
                {
                    var ListJobs = JobService.GetAllJobs();
                    return View(ListJobs.ToList().ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var company = CompanyService.GetCompanyByUserId(userId);
                    if (company != null)
                    {
                        var ListJobs = JobService.GetJobsByCompanyId(company.Id);
                        return View(ListJobs.ToList().ToPagedList(pageNumber, pageSize));
                    }
                }
            }

            //return View();
            return RedirectToAction("Index", "Job");
        }

        // GET: Job/Create
        public ActionResult Create()
        {
            // Job Level
            var jobLevels = CategoryService.GetJobLevel();
            ViewBag.JobLevel = new SelectList(jobLevels, "Id", "Name");

            // Job Category
            var jobCategories = CategoryService.GetJobCategory();
            ViewBag.JobCategory = new SelectList(jobCategories, "Id", "Name");

            // Job WorkPlace
            var jobWorkPlaces = CategoryService.GetCity(CategoryService.VietNam);
            ViewBag.JobWorkPlace = new SelectList(jobWorkPlaces, "Id", "Name");

            // PreferredLanguageForApplications
            var ListLanguagesJobRequire = new List<object>();
            foreach (var item in CategoryService.LanguagesJobRequire)
            {
                ListLanguagesJobRequire.Add(new { Id = item.Key, Name = item.Value });
            }
            ViewBag.PreferredLanguageForApplications = new SelectList(ListLanguagesJobRequire, "Name", "Name");

            if (User.IsInRole("Administrator"))
            {
                // Company By Admin
                var companyByAdministrator = CompanyService.GetAllCompany();
                ViewBag.CompanyByAdministrator = new SelectList(companyByAdministrator, "Id", "Name");

                // Status
                ViewBag.Status = new SelectList(CategoryService.Status, "Id", "Name");
            }

            var userId = Guid.Parse(User.Identity.GetUserId());
            if (userId != null)
            {
                var company = CompanyService.GetCompanyByUserId(userId);
                if (company != null)
                {
                    var job = new JobViewModel { ContactPerson = company.ContactName, EmailForApplications = company.ContactEmail };
                    return View(job);
                }
            }

            return View();
        }

        // POST: Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] JobViewModel model, string[] JobCategory, string[] JobWorkPlace)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                if (userId != null)
                {
                    var job = new Job();

                    if (User.IsInRole("Administrator"))
                    {
                        if (model.CompanyByAdministrator != null)
                        {
                            var company = CompanyService.GetCompany(model.CompanyByAdministrator.Value);
                            job.CompanyId = company.Id;
                            job.CompanyString = company.Name;
                        }
                        job.Status = model.Status == null ? (int)Common.Util.Define.Status.DeActive : model.Status;
                    }
                    else
                    {
                        var company = CompanyService.GetCompanyByUserId(userId);
                        if (company != null)
                        {
                            job.CompanyId = company.Id;
                            job.CompanyString = company.Name;
                            job.Status = (int)Common.Util.Define.Status.DeActive;
                        }
                    }

                    if (job.CompanyId != null)
                    {
                        job.JobTitle = model.JobTitle;
                        job.JobUrl = Common.Util.Common.RemoveUnicode(model.JobTitle);
                        job.JobLevel = model.JobLevel;
                        job.JobLevelString = CategoryService.GetCategoryById(model.JobLevel.Value).Name;
                        job.SalaryRangeFrom = model.SalaryRangeFrom;
                        job.SalaryRangeTo = model.SalaryRangeTo;
                        job.JobDescription = model.JobDescription;
                        job.JobRequirement = model.JobRequirement;
                        job.JobSkill = model.JobSkill;
                        job.JobBenefit = model.JobBenefit;
                        job.ContactPerson = model.ContactPerson;
                        job.EmailForApplications = model.EmailForApplications;
                        job.PreferredLanguageForApplications = model.PreferredLanguageForApplications;
                        job.ViewCount = 0;
                        job.TimeCreate = DateTime.Now;
                        
                        db.Jobs.Add(job);
                        db.SaveChanges();

                        // Job Categories
                        if (JobCategory != null)
                        {
                            var JobCategoriesString = string.Empty;
                            job.JobCategories = new List<JobCategory>();
                            foreach (var item in JobCategory)
                            {
                                var category = CategoryService.GetCategoryById(Guid.Parse(item));
                                var jobCategoryToAdd = new JobCategory()
                                {
                                    JobId = job.Id,
                                    JobCategoryId = category.Id
                                };
                                job.JobCategories.Add(jobCategoryToAdd);
                                JobCategoriesString += category.Name + ", ";
                            }
                            job.JobCategorieString = JobCategoriesString.Substring(0, JobCategoriesString.Length - 2);
                            db.SaveChanges();
                        }

                        // Job Work Places
                        if (JobWorkPlace != null)
                        {
                            var JobWorkPlacesString = string.Empty;
                            job.JobWorkPlaces = new List<JobWorkPlace>();
                            foreach (var item in JobWorkPlace)
                            {
                                var workPlace = CategoryService.GetCategoryById(Guid.Parse(item));
                                var jobWorkPlaceToAdd = new JobWorkPlace()
                                {
                                    JobId = job.Id,
                                    WorkPlaceId = workPlace.Id
                                };
                                job.JobWorkPlaces.Add(jobWorkPlaceToAdd);
                                JobWorkPlacesString += workPlace.Name + ", ";
                            }
                            job.JobWorkPlaceString = JobWorkPlacesString.Substring(0, JobWorkPlacesString.Length - 2);
                            db.SaveChanges();
                        }

                        return RedirectToAction("List");
                    }
                }
            }
            return View(model);
        }

        // GET: Job/Edit/5
        public ActionResult Edit(string title, int? id)
        {
            var job = JobService.GetJobByJobId(id.Value, "Company", "JobCategories", "JobWorkPlaces");
            if (job != null)
            {
                if (!job.JobUrl.Equals(title))
                {
                    return RedirectToAction("List", "Job");
                }

                var userId = Guid.Parse(User.Identity.GetUserId());
                if (userId != null)
                {
                    var company = CompanyService.GetCompanyByUserId(userId);
                    if (!((company != null && job.CompanyId == company.Id) || User.IsInRole("Administrator")))
                    {
                        return RedirectToAction("List", "Job");
                    }
                }

                // Job Level
                var jobLevels = CategoryService.GetJobLevel();
                ViewBag.JobLevel = new SelectList(jobLevels, "Id", "Name", job.JobLevel);

                // Job Category
                var jobCategories = CategoryService.GetJobCategory();
                ViewBag.JobCategory = new SelectList(jobCategories, "Id", "Name");

                // Job WorkPlace
                var jobWorkPlaces = CategoryService.GetCity(CategoryService.VietNam);
                ViewBag.JobWorkPlace = new SelectList(jobWorkPlaces, "Id", "Name");

                var jobCategoriesOfJob = job.JobCategories.Select(x => new { CategoryId = x.JobCategoryId }).ToList();
                ViewBag.JobCategoriesOfJob = Newtonsoft.Json.JsonConvert.SerializeObject(jobCategoriesOfJob);

                var jobWorkPlacesOfJob = job.JobWorkPlaces.Select(x => new { WorkPlaceId = x.WorkPlaceId }).ToList();
                ViewBag.JobWorkPlacesOfJob = Newtonsoft.Json.JsonConvert.SerializeObject(jobWorkPlacesOfJob);

                // PreferredLanguageForApplications
                var ListLanguagesJobRequire = new List<object>();
                foreach (var item in CategoryService.LanguagesJobRequire)
                {
                    ListLanguagesJobRequire.Add(new { Id = item.Key, Name = item.Value });
                }
                ViewBag.PreferredLanguageForApplications = new SelectList(ListLanguagesJobRequire, "Name", "Name", job.PreferredLanguageForApplications);

                if (User.IsInRole("Administrator"))
                {
                    // Company By Admin
                    var companyByAdministrator = CompanyService.GetAllCompany();
                    ViewBag.CompanyByAdministrator = new SelectList(companyByAdministrator, "Id", "Name", job.CompanyId);

                    // Status
                    ViewBag.Status = new SelectList(CategoryService.Status, "Id", "Name", job.Status);
                }

                var jobViewModel = new JobViewModel()
                {
                    Id = job.Id,
                    CompanyByAdministrator = job.CompanyId,
                    CompanyId = job.CompanyId,
                    CompanyString = job.CompanyString,
                    JobTitle = job.JobTitle,
                    JobLevel = job.JobLevel,
                    JobLevelString = job.JobLevelString,
                    SalaryRangeFrom = job.SalaryRangeFrom,
                    SalaryRangeTo = job.SalaryRangeTo,
                    JobDescription = job.JobDescription,
                    JobRequirement = job.JobRequirement,
                    JobSkill = job.JobSkill,
                    JobBenefit = job.JobBenefit,
                    JobCategorieString = job.JobCategorieString,
                    JobWorkPlaceString = job.JobWorkPlaceString,
                    ContactPerson = job.ContactPerson,
                    EmailForApplications = job.EmailForApplications,
                    PreferredLanguageForApplications = job.PreferredLanguageForApplications,
                    Status = job.Status
                };
                return View(jobViewModel);
            }

            //return View();
            return RedirectToAction("Index", "Job");
        }

        // POST: Job/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobViewModel model, string[] JobCategory, string[] JobWorkPlace)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                if (userId != null)
                {
                    var company = CompanyService.GetCompanyByUserId(userId);
                    if (company != null || User.IsInRole("Administrator"))
                    {
                        var job = db.Jobs.Include("JobCategories").Include("JobWorkPlaces").FirstOrDefault(x => x.Id == model.Id);

                        if (User.IsInRole("Administrator"))
                        {
                            if (model.CompanyByAdministrator != null)
                            {
                                var companyByAdmin = CompanyService.GetCompany(model.CompanyByAdministrator.Value);
                                job.CompanyId = companyByAdmin.Id;
                                job.CompanyString = companyByAdmin.Name;
                            }
                            job.Status = model.Status == null ? (int)Common.Util.Define.Status.DeActive : model.Status;
                        }

                        job.JobTitle = model.JobTitle;
                        job.JobLevel = model.JobLevel;
                        job.JobLevelString = CategoryService.GetCategoryById(model.JobLevel.Value).Name;
                        job.SalaryRangeFrom = model.SalaryRangeFrom;
                        job.SalaryRangeTo = model.SalaryRangeTo;
                        job.JobDescription = model.JobDescription;
                        job.JobRequirement = model.JobRequirement;
                        job.JobSkill = model.JobSkill;
                        job.JobBenefit = model.JobBenefit;
                        job.ContactPerson = model.ContactPerson;
                        job.EmailForApplications = model.EmailForApplications;
                        job.PreferredLanguageForApplications = model.PreferredLanguageForApplications;
                        db.SaveChanges();

                        // Job Categories
                        if (JobCategory != null)
                        {
                            var JobCategoriesString = string.Empty;
                            job.JobCategorieString = JobCategoriesString;

                            var JobCategoriesFromDB = job.JobCategories.ToList();
                            var JobCategoriesCategoryIdFromDB = JobCategoriesFromDB.Select(x => x.JobCategoryId).ToList();
                            foreach (var item in JobCategoriesFromDB)
                            {
                                if (!JobCategory.Contains(item.JobCategoryId.ToString()))
                                {
                                    db.JobCategories.Remove(item);
                                }
                                else
                                {
                                    JobCategoriesString += CategoryService.GetCategoryById(item.JobCategoryId.Value).Name + ", ";
                                }
                            }
                            foreach (var item in JobCategory)
                            {
                                if (!JobCategoriesCategoryIdFromDB.Contains(Guid.Parse(item)))
                                {
                                    var category = db.Categories.Find(Guid.Parse(item));
                                    var jobCategoryToAdd = new JobCategory()
                                    {
                                        JobId = job.Id,
                                        JobCategoryId = category.Id
                                    };
                                    job.JobCategories.Add(jobCategoryToAdd);
                                    JobCategoriesString += category.Name + ", ";
                                }
                            }
                            if (!string.IsNullOrEmpty(JobCategoriesString))
                                job.JobCategorieString = JobCategoriesString.Substring(0, JobCategoriesString.Length - 2);
                        }
                        else
                        {
                            job.JobCategories = new List<JobCategory>();
                            db.JobCategories.RemoveRange(db.JobCategories.Where(x => x.JobId == model.Id));
                            job.JobCategorieString = string.Empty;
                        }

                        // Job Work Places
                        if (JobWorkPlace != null)
                        {
                            var JobWorkPlacesString = string.Empty;
                            job.JobWorkPlaceString = JobWorkPlacesString;

                            var JobWorkPlacesFromDB = job.JobWorkPlaces.ToList();
                            var JobWorkPlacesWorkPlaceIdFromDB = JobWorkPlacesFromDB.Select(x => x.WorkPlaceId).ToList();
                            foreach (var item in JobWorkPlacesFromDB)
                            {
                                if (!JobWorkPlace.Contains(item.WorkPlaceId.ToString()))
                                {
                                    db.JobWorkPlaces.Remove(item);
                                }
                                else
                                {
                                    JobWorkPlacesString += CategoryService.GetCategoryById(item.WorkPlaceId.Value).Name + ", ";
                                }
                            }
                            foreach (var item in JobWorkPlace)
                            {
                                if (!JobWorkPlacesWorkPlaceIdFromDB.Contains(Guid.Parse(item)))
                                {
                                    var workPlace = db.Categories.Find(Guid.Parse(item));
                                    var jobWorkPlaceToAdd = new JobWorkPlace()
                                    {
                                        JobId = job.Id,
                                        WorkPlaceId = workPlace.Id
                                    };
                                    job.JobWorkPlaces.Add(jobWorkPlaceToAdd);
                                    JobWorkPlacesString += workPlace.Name + ", ";
                                }
                            }
                            if (!string.IsNullOrEmpty(JobWorkPlacesString))
                                job.JobWorkPlaceString = JobWorkPlacesString.Substring(0, JobWorkPlacesString.Length - 2);
                        }
                        else
                        {
                            job.JobWorkPlaces = new List<JobWorkPlace>();
                            db.JobWorkPlaces.RemoveRange(db.JobWorkPlaces.Where(x => x.JobId == model.Id));
                            job.JobWorkPlaceString = string.Empty;
                        }

                        db.SaveChanges();

                        return RedirectToAction("List");
                    }
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        // GET: Job/SubmitApplication/5
        public ActionResult SubmitApplication(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());

                var resume = ResumeService.GetResumeByUserId(userId, "ResumeLanguages", "ResumeEmployments", "ResumeEducations");
                if (resume == null)
                {
                    return HttpNotFound();
                }
                var job = db.Jobs.Include("Company").FirstOrDefault(x => x.Id == id);
                if (job == null)
                {
                    return HttpNotFound();
                }
                var applied = false;
                JobApply jobApply = JobApplyService.Get(job.Id, userId);
                if (jobApply != null)
                    applied = true;

                var submitapplication = new SubmitApplicationViewModel
                {
                    Id = resume.Id,
                    UserId = resume.UserId,
                    FullName = resume.FullName,
                    Email = resume.Email,
                    DateOfBirth = resume.DateOfBirth,
                    Gender = resume.Gender,
                    MaritalStatus = resume.MaritalStatus,
                    Nationality = resume.Nationality,
                    Address = resume.Address,
                    Country = resume.Country,
                    City = resume.City,
                    District = resume.District,
                    Phone = resume.Phone,
                    JobTitle = job.JobTitle,
                    CompanyName = job.CompanyString,
                    Applied = applied
                };
                return PartialView(submitapplication);
            }
            else
            {
                var submitapplication = new SubmitApplicationViewModel
                {
                    Id = 0,
                    UserId = Guid.Empty,
                    FullName = string.Empty,
                    Email = string.Empty,
                    DateOfBirth = DateTime.Now,
                    Gender = false,
                    MaritalStatus = false,
                    Nationality = false,
                    Address = string.Empty,
                    Country = Guid.Empty,
                    City = Guid.Empty,
                    District = Guid.Empty,
                    Phone = string.Empty,
                    JobTitle = string.Empty,
                    CompanyName = string.Empty,
                    Applied = false
                };
                return PartialView(submitapplication);
            }
        }

        // POST: Job/SubmitApplication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApplication(SubmitApplicationViewModel submitapplication)
        {
            JobApplyService.Insert(new JobApply() { JobId = submitapplication.Id, UserId = Guid.Parse(User.Identity.GetUserId()), TimeCreate = DateTime.Now });
            return Redirect(Request.UrlReferrer.ToString());
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