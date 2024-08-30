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
    public interface IQuestionService
    {
        Task<IList<QuestionModel>> GetAllAsync();
        Task<QuestionModel> GetByIdAsync(int id);

        Task SaveAsync(Question question, string userName);
        Task DeleteAsync(int questionId, string userName);

        #region Import Questions
        Task ImportSruveyAndQuestionAsync(IFormFile file, int surveyId, string userName);
        #endregion
    }
}
