using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Business.Services
{
    public interface ISurveyService
    {
        Task<SurveyModel> GetById(int id);
        Task<IList<SurveyModel>> GetAllAsync();
        Task SaveAsync(Survey survey, IFormFile file, string webUrl, string userName);
        Task DeleteAsync(int surveyId, string userName);
        Task<bool> IsSurveyExist(string title);
        Task<SurveyUserMapping?> GetLastSurvey(string userName);
        Task<SurveyUnionConfig?> GetNextSurveyConfig();
        Task<SurveyUnionConfig?> GetSurveyActiveDepositAmount();
        Task<string> IsNextSurveyActive(string userName);
        Task<IList<SurveyLogModel>> GetSurveyLogsAsync(string userName);

        #region Survey Question

        Task SaveSurveyQuestionAsync(string questionIds, int surveyId);
        #endregion

    }
}
