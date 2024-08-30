using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class SurveyHomeModel
    {
        public string UserName { get; set; } = null!;
        public decimal UserBalance { get; set; }
        public decimal TodayIncome { get; set; }
        public decimal TeamComission { get; set; }
        public decimal AvailableBalance => UserBalance + TeamComission;
        public string SurveyHowLongAgo { get; set; } = null!;
        public IList<SurveyModel> Surveys { get; set; } = [];
        public IList<QuestionModel> Questions { get; set; } = [];
        public SurveyModel? Survey { get; set; }
    }
}
