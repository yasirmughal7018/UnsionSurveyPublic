using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Data.IRepository
{
    public interface ISurveyRepo : IRepository<Survey>
    {
        Task<IList<SurveyQuestion>> GetSuveyQuestionAsync(int surveyId);
        Task RemoveExistingQuestion(SurveyQuestion[] surveyQuestions);
        Task SaveSurveyQuestionAsync(string questionIds, int surveyId);
        Task<SurveyUserMapping?> GetLastSurvey(string userName);
        Task<IList<SurveyLogModel>> GetSurveyLogsAsync(string userName);
    }
}
