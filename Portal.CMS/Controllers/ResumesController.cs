using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal.Core.Database;
using Portal.Core.Service;
using Zody.Models;
using Microsoft.AspNet.Identity;
using Zody.App_Start;
using Microsoft.AspNet.Identity.Owin;

namespace Portal.CMS.Controllers
{
    [CustomAuthorizeAttribute]
    public class ResumesController : Controller
    {
        private PortalEntities db = new PortalEntities();

        // GET: Resumes/Details
        public ActionResult Details(string email)
        {
            Guid userId = Guid.Empty;
            if (!string.IsNullOrEmpty(email))
            {
                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = UserManager.FindByEmail(email);
                if (user != null)
                    userId = Guid.Parse(user.Id);
            }
            else
            {
                userId = Guid.Parse(User.Identity.GetUserId());
            }
            if (userId == Guid.Empty)
            {
                return HttpNotFound();
            }
            var resume = ResumeService.GetResumeByUserId(userId, "ResumeLanguages", "ResumeEmployments", "ResumeEducations");
            if (resume == null)
            {
                return HttpNotFound();
            }
            return View(resume);
        }

        // GET: Resumes/EditContact - Thông Tin Cá Nhân
        public ActionResult EditContact()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var resume = ResumeService.GetResumeByUserId(userId);
            if (resume != null)
            {
                BuildDropDownListCountry(resume.Country);
                if (resume.Country != null)
                {
                    BuildDropDownListCity(resume.Country.Value, resume.City);
                    if (resume.City != null)
                    {
                        BuildDropDownListDistrict(resume.City.Value, resume.District);
                    }
                }

                var contact = new ResumeContactViewModel
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
                    Phone = resume.Phone
                };

                return PartialView(contact);
            }
            return PartialView();
        }

        // POST: Resumes/EditContact - Thông Tin Cá Nhân
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContact(ResumeContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resume = db.Resumes.Find(model.Id);
                resume.DateOfBirth = model.DateOfBirth;
                resume.Gender = model.Gender;
                resume.MaritalStatus = model.MaritalStatus;
                resume.Nationality = model.Nationality;
                resume.Address = model.Address;
                resume.Country = model.Country;
                resume.City = model.City;
                resume.District = model.District;
                resume.Phone = model.Phone;
                resume.FullName = model.FullName;
                resume.Email = model.Email;
                db.SaveChanges();

                return Json(new { success = true });
            }
            return PartialView(model);
        }

        // GET: Resumes/EditSummary - Thông Tin Chung
        public ActionResult EditSummary()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var resume = ResumeService.GetResumeByUserId(userId, "ResumeLanguages");
            if (resume != null)
            {
                var summary = new ResumeSummaryViewModel
                {
                    Id = resume.Id,
                    YearOfExperience = resume.YearOfExperience,
                    HighestEducation = resume.HighestEducation,
                    MostRecentCompany = resume.MostRecentCompany,
                    MostRecentPosition = resume.MostRecentPosition,
                    CurrentJobLevel = resume.CurrentJobLevel,
                    ExpectedPosition = resume.ExpectedPosition,
                    ExpectedJobLevel = resume.ExpectedJobLevel,
                    ExpectedLocation = resume.ExpectedLocation,
                    ExpectedJobCategory = resume.ExpectedJobCategory,
                    ExpectedSalary = resume.ExpectedSalary
                };

                BuildDropDownListEducationLevel(summary.HighestEducation);
                BuildDropDownListJobLevel(summary.CurrentJobLevel, summary.ExpectedJobLevel);
                BuildDropDownListExpectedLocation(summary.ExpectedLocation);
                BuildDropDownListJobCategory(summary.ExpectedJobCategory);
                BuildDropDownListLanguage();
                BuildDropDownListProficiency();

                return PartialView(summary);
            }
            return PartialView();
        }

        // POST: Resumes/EditSummary - Thông Tin Chung
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSummary(ResumeSummaryViewModel model, string[] Language, string[] Proficiency)
        {
            if (ModelState.IsValid)
            {
                var resume = db.Resumes.Include("ResumeLanguages").FirstOrDefault(x => x.Id == model.Id);
                resume.YearOfExperience = model.YearOfExperience;
                resume.HighestEducation = model.HighestEducation;
                resume.MostRecentCompany = model.MostRecentCompany;
                resume.MostRecentPosition = model.MostRecentPosition;
                resume.CurrentJobLevel = model.CurrentJobLevel;
                resume.ExpectedPosition = model.ExpectedPosition;
                resume.ExpectedJobLevel = model.ExpectedJobLevel;
                resume.ExpectedLocation = model.ExpectedLocation;
                resume.ExpectedJobCategory = model.ExpectedJobCategory;
                resume.ExpectedSalary = model.ExpectedSalary;
                db.SaveChanges();

                #region Resume Languages
                if (Language != null)
                {
                    var ResumeLanguagesFromDB = resume.ResumeLanguages.ToList();
                    var ResumeLanguagesIdFromDB = ResumeLanguagesFromDB.Select(x => x.LanguageId).ToList();
                    var ResumeLanguageProficiencyFromDB = ResumeLanguagesFromDB.Select(x => x.LanguageProficiency).ToList();
                    foreach (var item in ResumeLanguagesFromDB)
                    {
                        if (!(Language.Contains(item.LanguageId.ToString()) && Proficiency.Contains(item.LanguageProficiency.ToString())))
                        {
                            db.ResumeLanguages.Remove(item);
                        }
                    }
                    for (int i = 0; i < Language.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(Language[i]) && !string.IsNullOrEmpty(Proficiency[i]))
                            if (!(ResumeLanguagesIdFromDB.Contains(Guid.Parse(Language[i])) && ResumeLanguageProficiencyFromDB.Contains(Int32.Parse(Proficiency[i]))))
                            {
                                var language = db.Categories.Find(Guid.Parse(Language[i]));
                                var resumeLanguageToAdd = new ResumeLanguage()
                                {
                                    ResumeId = resume.Id,
                                    LanguageId = language.Id,
                                    LanguageProficiency = Int32.Parse(Proficiency[i])
                                };
                                resume.ResumeLanguages.Add(resumeLanguageToAdd);
                            }
                    }
                }
                else
                {
                    resume.ResumeLanguages = new List<ResumeLanguage>();
                    db.ResumeLanguages.RemoveRange(db.ResumeLanguages.Where(x => x.ResumeId == resume.Id));
                }
                db.SaveChanges();
                #endregion

                return Json(new { success = true });
            }

            return PartialView(model);
        }

        // GET: Resumes/EditProfile - Hồ Sơ / Mục Tiêu Nghề Nghiệp
        public ActionResult EditProfile()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var resume = ResumeService.GetResumeByUserId(userId);
            if (resume != null)
            {
                var profile = new ResumeProfileViewModel
                {
                    Id = resume.Id,
                    IntroduceYourself = resume.IntroduceYourself
                };

                return PartialView(profile);
            }
            return PartialView();
        }

        // POST: Resumes/EditProfile - Hồ Sơ / Mục Tiêu Nghề Nghiệp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(ResumeProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resume = db.Resumes.Find(model.Id);
                resume.IntroduceYourself = model.IntroduceYourself;
                db.SaveChanges();

                return Json(new { success = true });
            }

            return PartialView(model);
        }

        // GET: Resumes/CreateEmployment - Kinh Nghiệm Làm Việc
        public ActionResult CreateEmployment()
        {
            return PartialView();
        }

        // POST: Resumes/CreateEmployment - Kinh Nghiệm Làm Việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmployment(CreateEmploymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                var resume = ResumeService.GetResumeByUserId(userId);
                if (resume != null)
                {
                    var employment = new ResumeEmployment()
                    {
                        ResumeId = resume.Id,
                        Position = model.Position,
                        Company = model.Company,
                        FromYear = DateTime.ParseExact(model.FromYear, "MM/yyyy", null),
                        ToYear = (model.CurrentJob.HasValue && model.CurrentJob.Value ? DateTime.Now : DateTime.ParseExact(model.ToYear, "MM/yyyy", null)),
                        CurrentJob = model.CurrentJob,
                        Description = model.Description
                    };
                    db.ResumeEmployments.Add(employment);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return PartialView(model);
        }

        // GET: Resumes/EditEmployment - Kinh Nghiệm Làm Việc
        public ActionResult EditEmployment(int? Id)
        {
            if (Id == null)
            {
                return HttpNotFound();
            }
            var employment = ResumeEmploymentService.GetResumeEmploymentById(Id.Value);
            if (employment == null)
            {
                return HttpNotFound();
            }
            var employmentViewModel = new EditEmploymentViewModel()
            {
                Id = employment.Id,
                ResumeId = employment.ResumeId,
                Position = employment.Position,
                Company = employment.Company,
                FromYear = employment.FromYear.Value.ToString("MM/yyyy"),
                ToYear = employment.ToYear.Value.ToString("MM/yyyy"),
                CurrentJob = employment.CurrentJob,
                Description = employment.Description
            };
            return PartialView(employmentViewModel);
        }

        // POST: Resumes/EditEmployment - Kinh Nghiệm Làm Việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployment(EditEmploymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employment = db.ResumeEmployments.Find(model.Id);
                employment.Position = model.Position;
                employment.Company = model.Company;
                employment.FromYear = DateTime.ParseExact(model.FromYear, "MM/yyyy", null);
                employment.ToYear = (model.CurrentJob.HasValue && model.CurrentJob.Value ? DateTime.Now : DateTime.ParseExact(model.ToYear, "MM/yyyy", null));
                employment.CurrentJob = model.CurrentJob;
                employment.Description = model.Description;
                //db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true });
            }

            return PartialView(model);
        }

        // POST: Resumes/DeleteEmployment - Kinh Nghiệm Làm Việc
        [HttpPost]
        public ActionResult DeleteEmployment(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var employment = db.ResumeEmployments.Find(id.Value);
            if (employment == null)
            {
                return HttpNotFound();
            }
            db.ResumeEmployments.Remove(employment);
            db.SaveChanges();

            return Json(new { success = true });
        }

        // GET: Resumes/CreateEducation - Học Vấn Và Bằng Cấp
        public ActionResult CreateEducation()
        {
            var qualifications = CategoryService.GetEducationLevel();
            ViewBag.Qualification = new SelectList(qualifications, "Id", "Name");

            return PartialView();
        }

        // POST: Resumes/CreateEducation - Học Vấn Và Bằng Cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEducation(CreateEducationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                var resume = ResumeService.GetResumeByUserId(userId);
                if (resume != null)
                {
                    var education = new ResumeEducation()
                    {
                        ResumeId = resume.Id,
                        School = model.School,
                        Qualification = model.Qualification,
                        FromYear = DateTime.ParseExact(model.FromYear, "MM/yyyy", null),
                        ToYear = DateTime.ParseExact(model.ToYear, "MM/yyyy", null),
                        Achievements = model.Achievements
                    };
                    db.ResumeEducations.Add(education);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return PartialView(model);
        }

        // GET: Resumes/EditEducation - Học Vấn Và Bằng Cấp
        public ActionResult EditEducation(int? Id)
        {
            if (Id == null)
            {
                return HttpNotFound();
            }
            var education = ResumeEducationService.GetResumeEducationById(Id.Value);
            if (education == null)
            {
                return HttpNotFound();
            }
            var educationViewModel = new EditEducationViewModel()
            {
                Id = education.Id,
                ResumeId = education.ResumeId,
                School = education.School,
                Qualification = education.Qualification,
                FromYear = education.FromYear.Value.ToString("MM/yyyy"),
                ToYear = education.ToYear.Value.ToString("MM/yyyy"),
                Achievements = education.Achievements
            };
            var qualifications = CategoryService.GetEducationLevel();
            ViewBag.Qualification = new SelectList(qualifications, "Id", "Name", education.Qualification);

            return PartialView(educationViewModel);
        }

        // POST: Resumes/EditEducation - Học Vấn Và Bằng Cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEducation(EditEducationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var education = db.ResumeEducations.Find(model.Id);
                education.School = model.School;
                education.Qualification = model.Qualification;
                education.FromYear = DateTime.ParseExact(model.FromYear, "MM/yyyy", null);
                education.ToYear = DateTime.ParseExact(model.ToYear, "MM/yyyy", null);
                education.Achievements = model.Achievements;
                //db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true });
            }

            return PartialView(model);
        }

        // POST: Resumes/DeleteEducation - Học Vấn Và Bằng Cấp
        [HttpPost]
        public ActionResult DeleteEducation(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var education = db.ResumeEducations.Find(id.Value);
            if (education == null)
            {
                return HttpNotFound();
            }
            db.ResumeEducations.Remove(education);
            db.SaveChanges();

            return Json(new { success = true });
        }

        #region Build DropDownList
        private void BuildDropDownListCountry(object selectedCountry = null)
        {
            var country = CategoryService.GetCountry();
            ViewBag.Country = new SelectList(country, "Id", "Name", selectedCountry);
        }

        private void BuildDropDownListCity(Guid countryId, object selectedCity = null)
        {
            var city = CategoryService.GetCity(countryId);
            ViewBag.City = new SelectList(city, "Id", "Name", selectedCity);
        }

        public JsonResult GetCities(string id)
        {
            var cities = CategoryService.GetCity(Guid.Parse(id));
            return Json(new SelectList(cities, "Id", "Name"));
        }

        private void BuildDropDownListDistrict(Guid cityId, object selectedDistrict = null)
        {
            var district = CategoryService.GetDistrict(cityId);
            ViewBag.District = new SelectList(district, "Id", "Name", selectedDistrict);
        }

        public JsonResult GetDistricts(string id)
        {
            var districts = CategoryService.GetDistrict(Guid.Parse(id));
            return Json(new SelectList(districts, "Id", "Name"));
        }

        private void BuildDropDownListExpectedLocation(object selectedExpectedLocation = null)
        {
            var expectedLocation = CategoryService.GetCity(CategoryService.VietNam);
            ViewBag.ExpectedLocation = new SelectList(expectedLocation, "Id", "Name", selectedExpectedLocation);
        }

        private void BuildDropDownListEducationLevel(object selectedEducationLevel = null)
        {
            var highestEducation = CategoryService.GetEducationLevel();
            ViewBag.HighestEducation = new SelectList(highestEducation, "Id", "Name", selectedEducationLevel);
        }

        private void BuildDropDownListJobLevel(object selectedCurrentJobLevel = null, object selectedExpectedJobLevel = null)
        {
            var jobLevel = CategoryService.GetJobLevel();
            ViewBag.CurrentJobLevel = new SelectList(jobLevel, "Id", "Name", selectedCurrentJobLevel);
            ViewBag.ExpectedJobLevel = new SelectList(jobLevel, "Id", "Name", selectedExpectedJobLevel);
        }

        private void BuildDropDownListJobCategory(object selectedExpectedJobCategory = null)
        {
            var jobCategory = CategoryService.GetJobCategory();
            ViewBag.ExpectedJobCategory = new SelectList(jobCategory, "Id", "Name", selectedExpectedJobCategory);
        }

        private void BuildDropDownListLanguage()
        {
            var language = CategoryService.GetLanguage();
            ViewBag.Language = new SelectList(language, "Id", "Name");
        }

        private void BuildDropDownListProficiency()
        {
            var ListProficiency = new List<SelectListItem>();
            foreach (var item in CategoryService.Proficiency)
            {
                ListProficiency.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            ViewBag.Proficiency = ListProficiency;
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
