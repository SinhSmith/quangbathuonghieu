using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.CMS.Models
{
    public class ResumeContactViewModel
    {
        public int Id { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<bool> Gender { get; set; }
        public Nullable<bool> MaritalStatus { get; set; }
        public Nullable<bool> Nationality { get; set; }
        public string Address { get; set; }
        public Nullable<System.Guid> Country { get; set; }
        public Nullable<System.Guid> City { get; set; }
        public Nullable<System.Guid> District { get; set; }
        public string Phone { get; set; }
    }

    public class ResumeSummaryViewModel
    {
        public int Id { get; set; }
        public Nullable<int> YearOfExperience { get; set; }
        public Nullable<System.Guid> HighestEducation { get; set; }
        public string MostRecentCompany { get; set; }
        public string MostRecentPosition { get; set; }
        public Nullable<System.Guid> CurrentJobLevel { get; set; }
        public string ExpectedPosition { get; set; }
        public Nullable<System.Guid> ExpectedJobLevel { get; set; }
        public Nullable<System.Guid> ExpectedLocation { get; set; }
        public Nullable<System.Guid> ExpectedJobCategory { get; set; }
        public Nullable<decimal> ExpectedSalary { get; set; }
    }
    public class ResumeProfileViewModel
    {
        public int Id { get; set; }
        public string IntroduceYourself { get; set; }
    }

    public class CreateEmploymentViewModel
    {
        public Nullable<int> ResumeId { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string FromYear { get; set; }
        public string ToYear { get; set; }
        public Nullable<bool> CurrentJob { get; set; }
        public string Description { get; set; }
    }

    public class EditEmploymentViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> ResumeId { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string FromYear { get; set; }
        public string ToYear { get; set; }
        public Nullable<bool> CurrentJob { get; set; }
        public string Description { get; set; }
    }

    public class CreateEducationViewModel
    {
        public Nullable<int> ResumeId { get; set; }
        public string School { get; set; }
        public Nullable<System.Guid> Qualification { get; set; }
        public string FromYear { get; set; }
        public string ToYear { get; set; }
        public string Achievements { get; set; }
    }

    public class EditEducationViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ResumeId { get; set; }
        public string School { get; set; }
        public Nullable<System.Guid> Qualification { get; set; }
        public string FromYear { get; set; }
        public string ToYear { get; set; }
        public string Achievements { get; set; }
    }
}