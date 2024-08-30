using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class SurveyService(ISurveyRepo surveyRepo
                            , ISurveyUnionConfigRepo surveyUnionConfigRepo) : ISurveyService
    {
        public async Task<SurveyModel> GetById(int id)
        {
            var survey = await surveyRepo.GetByIdAsync(id);
            SurveyModel model = Mapper.ToSurveyModel(survey);
            return model;
        }
        public async Task<IList<SurveyModel>> GetAllAsync() =>
                                await (surveyRepo.GetAllAsync()
                                                 .Where(i => i.IsDelete == false)
                                                 .Select(i => Mapper.ToSurveyModel(i)))
                                                 .ToListAsync();
        public async Task<SurveyUserMapping?> GetLastSurvey(string userName)
                        => await surveyRepo.GetLastSurvey(userName);

        public async Task SaveAsync(Survey survey, IFormFile file, string webUrl, string userName)
        {
            try
            {
                survey.IsDelete = false;
                string uniqueCode = string.Empty;
                var dbEntity = await surveyRepo.GetByIdAsync(survey.SurveyId);

                if (survey.SurveyId == 0 || dbEntity == null)
                    uniqueCode = userName.ToSafeUniqueCode();
                else
                    uniqueCode = dbEntity.PathUniqueCode ?? userName.ToSafeUniqueCode();

                survey.LogoPath ??= "/img/us/survey";

                string fileName = "";
                if (file != null)
                {
                    fileName = $"{uniqueCode}_{file.FileName.ToSafeSubstringEnd(30)}";
                    string copyPath = Path.Combine(webUrl, "img", "us", "survey", $"{fileName}");

                    if (File.Exists(copyPath) == false)
                    {
                        using (var filestream = new FileStream(copyPath, FileMode.Create))
                        {
                            await file.CopyToAsync(filestream);
                        }
                    }
                }


                if (dbEntity == null)
                {
                    survey.CreatedBy = userName ?? "";
                    survey.CreatedDate = DateTime.UtcNow;
                    survey.LogoName = fileName;
                    survey.PathUniqueCode = uniqueCode;

                    await surveyRepo.AddAsync(survey);
                }
                else
                {
                    dbEntity.Title = survey.Title.ToSafeString();
                    dbEntity.Description = survey.Description.ToSafeString();
                    dbEntity.LogoName = fileName;
                    dbEntity.UsersForSurveyActive = survey.UsersForSurveyActive;

                    await surveyRepo.UpdateAsync(survey);
                }
            }
            catch (Exception ex) { throw ex; }

        }
        public async Task DeleteAsync(int surveyId, string userName)
        {
            try
            {
                var dbEntity = await surveyRepo.GetByIdAsync(surveyId);

                dbEntity!.ModifiedBy = userName;
                dbEntity!.ModifiedDate = DateTime.UtcNow;
                dbEntity.IsDelete = true;

                await surveyRepo.UpdateAsync(dbEntity);
            }
            catch (Exception ex) { throw ex; }

        }
        public async Task<bool> IsSurveyExist(string title)
                            => await surveyRepo.GetAllAsync()
                                                .AnyAsync(i => i.Title == title.ToSafeString());

        public async Task<SurveyUnionConfig?> GetNextSurveyConfig()
                        => await surveyUnionConfigRepo.GetAllAsync()
                                        .FirstOrDefaultAsync(i => i.ConfigName == "NextSurveyTime");

        public async Task<SurveyUnionConfig?> GetSurveyActiveDepositAmount()
                => await surveyUnionConfigRepo.GetAllAsync()
                                .FirstOrDefaultAsync(i => i.ConfigName == "SurveyActiveDepositAmount");

        public async Task<string> IsNextSurveyActive(string userName)
        {
            var lastSurvey = await GetLastSurvey(userName);
            if (lastSurvey == null)
                return string.Empty;


            string msg = string.Empty;
            var nextSurveyConfig = await GetNextSurveyConfig();

            if (nextSurveyConfig == null)
                msg = "Please contact the service center to configure your survey.";
            else
            {
                var lastSurveyDateTime = lastSurvey.CompletedDate;
                DateOnly nextSurveyDate = DateOnly.FromDateTime(lastSurveyDateTime).AddDays(1);
                var nextSurveyTime = TimeOnly.FromDateTime(Convert.ToDateTime(nextSurveyConfig.ConfigVal));
                var nextSurveyDateTime = new DateTime(nextSurveyDate, nextSurveyTime);

                if (DateTime.UtcNow <= nextSurveyDateTime)
                {
                    var timeDifference = (nextSurveyDateTime - DateTime.UtcNow);
                    msg = timeDifference.ToSafeWait();
                }
            }


            return msg;
        }

        public async Task<IList<SurveyLogModel>> GetSurveyLogsAsync(string userName)
                                    => await surveyRepo.GetSurveyLogsAsync(userName);

        #region Survey Question

        public async Task SaveSurveyQuestionAsync(string questionIds, int surveyId)
        {
            try
            {
                await surveyRepo.SaveSurveyQuestionAsync(questionIds, surveyId);
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion
    }
}
