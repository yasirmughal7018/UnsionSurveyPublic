using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }

        [StringLength(2000)]
        public string Question { get; set; } = null!;

        [StringLength(100)]
        [Display(Name ="Option")]
        public string? Option1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Option")]
        public string? Option2 { get; set; }

        [StringLength(100)]
        [Display(Name = "Option")]
        public string? Option3 { get; set; }

        [StringLength(100)]
        [Display(Name = "Option")]
        public string? Option4 { get; set; }

        public string? SelectedOption { get; set; }

        public int SurveyId { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        public int SpendTime { get; set; }

        public string HtmlId => $"q_{QuestionId}";

        public IList<QuestionModel> QuestionList { get; set; } = [];

    }
}
