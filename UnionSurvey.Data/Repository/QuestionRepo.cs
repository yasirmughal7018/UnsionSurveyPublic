using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Data.Repository
{
    public class QuestionRepo(SurveyUnionContext context)
                    : Repository<Question>(context), IQuestionRepo
    {

        public async new Task<IList<QuestionModel>> GetAllAsync()
        {

            IList<QuestionModel> questions = await (from q in _context.Questions
                                                    join s in _context.SurveyQuestions on q.QuestionId equals s.QuestionId into g
                                                    from s in g.DefaultIfEmpty()
                                                    where q.IsActive == true
                                                    select new QuestionModel
                                                    {
                                                        QuestionId = q.QuestionId,
                                                        Question = q.Question1,
                                                        Option1 = q.Option1,
                                                        Option2 = q.Option2,
                                                        Option3 = q.Option3,
                                                        Option4 = q.Option4,
                                                        SurveyId = s != null ? s.SurveyId : 0,
                                                    })
                                                    .ToListAsync();


            return questions;
        }

        public async Task SaveQuestionBulk(IList<Question> questions)
        {
            await _context.Questions.AddRangeAsync(questions);
            await _context.SaveChangesAsync();
        }
    }
}
