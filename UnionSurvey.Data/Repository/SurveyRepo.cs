using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Data.Repository
{
    public class SurveyRepo(SurveyUnionContext context)
                    : Repository<Survey>(context), ISurveyRepo
    {

        private static readonly char[] separator = [','];

        public async Task<IList<SurveyQuestion>> GetSuveyQuestionAsync(int surveyId)
                                                    => await _context.SurveyQuestions
                                                                 .Where(i => i.SurveyId == surveyId)
                                                                 .ToListAsync();

        public async Task RemoveExistingQuestion(SurveyQuestion[] surveyQuestions)
        {
            _context.SurveyQuestions.RemoveRange(surveyQuestions);
            await _context.SaveChangesAsync();
        }

        public async Task SaveSurveyQuestionAsync(string questionIds, int surveyId)
        {
            SurveyQuestion[] ids = questionIds.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(i =>
                                        new SurveyQuestion
                                        {
                                            QuestionId = int.Parse(i),
                                            SurveyId = surveyId
                                        })
                                     .ToArray();


            var existQuestions = await GetSuveyQuestionAsync(surveyId);

            _context.SurveyQuestions.RemoveRange(existQuestions.ToArray());
            await _context.SurveyQuestions.AddRangeAsync(ids);
            await _context.SaveChangesAsync();

        }

        public async Task<SurveyUserMapping?> GetLastSurvey(string userName)
                                => await _context.SurveyUserMappings
                                                .OrderByDescending(j => j.CompletedDate)
                                                .FirstOrDefaultAsync(i => i.UserName == userName);

        public async Task<IList<SurveyLogModel>> GetSurveyLogsAsync(string userName)
        {
            IList<SurveyLogModel> surveyLogs = [];


            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SP_SurveyLog";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter p_UserName = new("@userName", userName.ToSafeDbValue());

                command.Parameters.Add(p_UserName);
                try
                {
                    _context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        while (await result.ReadAsync())
                        {
                            var surveyLog = new SurveyLogModel()
                            {
                                SurveyId = result.GetFieldValue<int>(result.GetOrdinal("SurveyId")),
                                Title = result.GetFieldValue<string>(result.GetOrdinal("Title")),
                                Description = result.GetFieldValue<string>(result.GetOrdinal("Description")),
                                LogoPath = result.GetFieldValue<string>(result.GetOrdinal("LogoPath")),
                                LogoName = result.GetFieldValue<string>(result.GetOrdinal("LogoName")),
                                SurveyAmount = result.GetFieldValue<decimal>(result.GetOrdinal("SurveyAmount")),
                                SurveyCompletedBy = result.GetFieldValue<string>(result.GetOrdinal("SurveyCompletedBy")),
                                TotalSurveyQuestions = result.GetFieldValue<int>(result.GetOrdinal("TotalSurveyQuestions")),
                                CompletedDate = result.GetFieldValue<DateTime>(result.GetOrdinal("CompletedDate")),
                                IsSurveyCompleted = result.GetFieldValue<bool>(result.GetOrdinal("IsSurveyCompleted")),
                            };

                            surveyLogs.Add(surveyLog);
                        }
                    }
                }
                finally { await _context.Database.CloseConnectionAsync(); }
            }

            return surveyLogs;
        }
    }
}
