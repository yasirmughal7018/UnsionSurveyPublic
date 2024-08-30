using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class SurveyLogModel
    {
        public int SurveyId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string LogoPath { get; set; } = null!;
        public string LogoName { get; set; } = null!;
        public decimal SurveyAmount { get; set; }
        public int TotalSurveyQuestions { get; set; }
        public string SurveyCompletedBy { get; set; } = null!;
        public bool IsSurveyCompleted { get; set; }
        public DateTime CompletedDate { get; set; }

        public string LogoFullPath => $"{LogoPath}/{LogoName}";
        public string SurveyDivId => $"survey_{SurveyId}";
    }
}
