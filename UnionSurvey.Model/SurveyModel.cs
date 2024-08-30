using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class SurveyModel
    {
        public SurveyModel()
        {
            
        }

        [Display(Name = "Id")]
        public int SurveyId { get; set; }
        [Display(Name = "Survey")]
        [StringLength(50)]
        public string Title { get; set; } = null!;
        [StringLength(100)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
        [Display(Name = "Logo")]
        public IFormFile? LogoFile { get; set; }
        [Display(Name = "Survey Amount")]
        [Range(10, 99999)]
        public decimal SurveyAmount { get; set; }

        [Display(Name = "Activation Users")]
        [Range(0, 100)]
        public int UsersForSurveyActive { get; set; }

        public bool IsActive { get; set; }
        public IList<SurveyModel> SurveyList { get; set; } = [];
        public IList<QuestionModel> QuestionList { get; set; } = [];



        public string? LogoPath { get; set; }
        public string? LogoName{ get; set; }
        public string LogoFullPath => $"{LogoPath}/{LogoName}";
        public string SurveyDivId => $"survey_{SurveyId}";
        public string SurveyStatus => IsActive ? "Active" : "Inactive";
        public string SurveyStatusBadge => IsActive ? "bg-success" : "bg-danger";


    }
}
