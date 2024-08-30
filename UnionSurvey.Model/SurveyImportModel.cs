using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class SurveyImportModel
    {
        [Display(Name = "Survey")]
        public int SurveyId { get; set; }
        [Display(Name = "Survey")]
        public string? Survey { get; set; } = null;
        [Display(Name = "Select question file to import")]
        public IFormFile QuestionFile { get; set; } = null!;

        public string? TemplateUrl { get; set; } = null;

        public IList<SelectItem> Surveys { get; set; } = [];
    }
}
