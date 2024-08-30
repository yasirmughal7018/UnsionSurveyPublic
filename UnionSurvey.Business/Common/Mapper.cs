using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Business.Common
{
    internal static class Mapper
    {
        public static SurveyModel ToSurveyModel(this Survey survey)
        {
            SurveyModel surveyModel = new()
            {
                SurveyId = survey.SurveyId,
                Description = survey.Description,
                Title = survey.Title,
                SurveyAmount = survey.SurveyAmount ?? 0,
                UsersForSurveyActive = survey.UsersForSurveyActive,
                LogoPath = survey.LogoPath,
                LogoName = survey.LogoName
            };

            return surveyModel;
        }

        public static QuestionModel ToQuestionModel(this Question question)
        {
            QuestionModel questionModel = new()
            {
                QuestionId = question.QuestionId,
                Question = question.Question1,
                Option1 = question.Option1,
                Option2 = question.Option2,
                Option3 = question.Option3,
                Option4 = question.Option4,
            };

            return questionModel;
        }

        public static UserHierarchyModel ToUserHierarchyModel(this SP_UserHierarchyWithStats userHierarchy)
        {
            UserHierarchyModel model = new()
            {
                UserName = userHierarchy.UserName,
                FirstName = userHierarchy.FirstName,
                LastName = userHierarchy.LastName,
                UserCode = userHierarchy.UserCode,
                ReferalCode = userHierarchy.ReferalCode,
                Email = userHierarchy.Email,
                PhoneNumber = userHierarchy.PhoneNumber,
                Level = userHierarchy.Level,
                UserBalance = userHierarchy.UserBalance,
                TodayIncome = userHierarchy.TodayIncome,
                TeamComission = userHierarchy.TeamComission,
                LastSurvey = userHierarchy.LastSurveyDate == null 
                                ? null 
                                :DateOnly.FromDateTime(userHierarchy.LastSurveyDate.Value),
                TeamBalance = userHierarchy.TeamBalance,
                DirectPromoters = userHierarchy.DirectPromoters,
                TotalSize = userHierarchy.TotalSize,
                DepositBalance = userHierarchy.DepositBalance,
                NewlyIncreased = userHierarchy.NewlyIncreased,
                TodaySurveyCompletedUsers = userHierarchy.TodaySurveyCompletedUsers,
            };

            return model;
        }
    }
}
