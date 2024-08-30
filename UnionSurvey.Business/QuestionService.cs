using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Business.Common;
using UnionSurvey.Business.Services;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Business
{
    public class QuestionService(IQuestionRepo questionRepo) : IQuestionService
    {
        public async Task<IList<QuestionModel>> GetAllAsync()
        {
            return await questionRepo.GetAllAsync();
        }

        public async Task<QuestionModel> GetByIdAsync(int id)
        {
            var dbQuestion = await questionRepo.GetByIdAsync(id);
            var question = Mapper.ToQuestionModel(dbQuestion);
            return question;
        }

        public async Task SaveAsync(Question question, string userName)
        {
            try
            {
                question.IsActive = true;
                question.Option1 = Utilities.FirstNonEmpty([question.Option1!, question.Option2!, question.Option3!, question.Option4!], 1, out short match)!;
                question.Option2 = Utilities.FirstNonEmpty([question.Option2!, question.Option3!, question.Option4!], match, out short match2);
                question.Option3 = Utilities.FirstNonEmpty([question.Option3!, question.Option4!], match2, out short match3);
                question.Option4 = Utilities.FirstNonEmpty([question.Option4!], match3, out short match4);

                if (question.QuestionId == 0)
                {
                    question.CreatedBy = userName ?? "";
                    question.CreatedDate = DateTime.UtcNow;

                    await questionRepo.AddAsync(question);
                }
                else
                {
                    var dbEntity = await questionRepo.GetByIdAsync(question.QuestionId);


                    dbEntity!.Option1 = question.Option1;
                    dbEntity!.Option2 = question.Option2;
                    dbEntity!.Option3 = question.Option3;
                    dbEntity!.Option4 = question.Option4;

                    dbEntity!.ModifiedBy = userName ?? "";
                    dbEntity!.ModifiedDate = DateTime.UtcNow;
                    dbEntity.Question1 = question.Question1;
                    dbEntity.IsActive = true;

                    await questionRepo.UpdateAsync(dbEntity);
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public async Task DeleteAsync(int questionId, string userName)
        {
            try
            {
                var dbEntity = await questionRepo.GetByIdAsync(questionId);

                dbEntity!.ModifiedBy = userName;
                dbEntity!.ModifiedDate = DateTime.UtcNow;
                dbEntity.IsActive = false;

                await questionRepo.UpdateAsync(dbEntity);
            }
            catch (Exception ex) { throw ex; }

        }

        #region Import Questions
        public async Task ImportSruveyAndQuestionAsync(IFormFile file, int surveyId, string userName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            try
            {
                IList<Question> questions = [];
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        string[] data = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                        if (data.Length > 0)
                        {
                            Question question = new()
                            {
                                Question1 = data.ElementAtOrDefault(0).ToSafeString(),
                                Option1 = data.ElementAtOrDefault(1).ToSafeString(),
                                Option2 = data.ElementAtOrDefault(2).ToSafeString(),
                                Option3 = data.ElementAtOrDefault(3).ToSafeString(),
                                Option4 = data.ElementAtOrDefault(4).ToSafeString(),
                                IsActive = true,
                                CreatedBy = userName.ToSafeString(),
                                CreatedDate = DateTime.UtcNow,
                            };

                            question.Option1 = Utilities.FirstNonEmpty([question.Option1!, question.Option2!, question.Option3!, question.Option4!], 1, out short match)!;
                            question.Option2 = Utilities.FirstNonEmpty([question.Option2!, question.Option3!, question.Option4!], match, out short match2);
                            question.Option3 = Utilities.FirstNonEmpty([question.Option3!, question.Option4!], match2, out short match3);
                            question.Option4 = Utilities.FirstNonEmpty([question.Option4!], match3, out short match4);

                            if (question.Option1.ToHasValue() && question.Question1.ToHasValue())
                            {
                                question.SurveyQuestions.Add(new SurveyQuestion { SurveyId = surveyId });
                                questions.Add(question);
                            }
                        }
                    }
                }

                await questionRepo.SaveQuestionBulk(questions);

            }
            catch (Exception ex) { throw ex; }

        }
        #endregion

    }
}
