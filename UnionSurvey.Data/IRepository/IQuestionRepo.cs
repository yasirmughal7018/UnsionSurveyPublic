using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Data.IRepository
{
    public interface IQuestionRepo : IRepository<Question>
    {
        new Task<IList<QuestionModel>> GetAllAsync();
        Task SaveQuestionBulk(IList<Question> questions);
    }
}
