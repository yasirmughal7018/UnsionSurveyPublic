using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using UnionSurvey.Business.Services;
using UnionSurvey.Common;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Controllers
{
    public class HomeController(IAppUserSerice userSerice
                                , UserManager<IdentityUser> userManager
                                , ISurveyService surveyService
                                , IFinancialService financialService
                                , ILogger<AccountController> logger)
                    : BaseController(logger)
    {
        public async Task<IActionResult> Index()
        {
            var user = await userManager.FindByNameAsync(LoginUserName);

            if (user == null) { return RedirectToAction("Login", "Account"); }

            var surveys = await AppHelper.SurveyAsync();
            var userFinance = await userSerice.GetUserFinancial(LoginUserName);
            var message = await IsNextSurveyActive();
            var activeUsers = (await userSerice.SP_GetActiveUserHierarchyAsync(user.Id))
                                                .Count(i => i.Level == 2);

            SurveyHomeModel model = new()
            {
                UserName = userFinance.UserName,
                UserBalance = userFinance.UserBalance.ToSafeDecimalFloor2(),
                TeamComission = userFinance.TeamComission.ToSafeDecimalFloor2(),
                TodayIncome = message.ToHasValue()
                                    ? userFinance.TodayIncome.ToSafeDecimalFloor2()
                                    : 0,
                SurveyHowLongAgo = message
            };


            surveys.ToList().ForEach(i =>
            {
                i.IsActive = i.SurveyAmount <= model.AvailableBalance
                                && i.UsersForSurveyActive <= activeUsers
                                && string.IsNullOrEmpty(model.SurveyHowLongAgo);
            });
            model.Surveys = [.. surveys.OrderByDescending(i => i.IsActive)
                                       .ThenBy(i => i.SurveyAmount)];
            return View(model);
        }

        public async Task<IActionResult> Survey(int surveyId)
        {
            var survey = (await AppHelper.SurveyAsync())
                                         .FirstOrDefault(i => i.SurveyId == surveyId);
            if (survey == null)
            {
                await AppHelper.SetSurveyAsync();
                survey = (await AppHelper.SurveyAsync())
                                        .FirstOrDefault(i => i.SurveyId == surveyId);

                if (survey == null)
                    return View();
            }

            var questions = await AppHelper.QuestionAsync(surveyId, survey.SurveyAmount);
            var userFinance = await userSerice.GetUserFinancial(LoginUserName);
            var message = await IsNextSurveyActive();

            SurveyHomeModel model = new()
            {
                UserName = userFinance.UserName,
                UserBalance = userFinance.UserBalance.ToSafeDecimalFloor2(),
                TeamComission = userFinance.TeamComission.ToSafeDecimalFloor2(),
                TodayIncome = message.ToHasValue()
                                    ? userFinance.TodayIncome.ToSafeDecimalFloor2()
                                    : 0,
                SurveyHowLongAgo = message,
                Survey = survey,
                Questions = questions
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Survey(SurveyHomeModel model)
        {
            string isNextSurveyAvailable = await IsNextSurveyActive();
            if (string.IsNullOrEmpty(isNextSurveyAvailable) == false)
                return RedirectToAction(nameof(Index));


            int surveyId = model.Survey!.SurveyId;

            decimal totalTimeSepnt = model.Questions.Sum(i => i.SpendTime);
            decimal totalTime = model.Questions.Count * 10;

            if (totalTimeSepnt == totalTime)
            {
                var totalAnswer = model.Questions
                                            .Where(i => i.SelectedOption.ToSafeString().Length > 0)
                                            .Select(i => i.SelectedOption)
                                            .ToList();

                if (totalAnswer.Count == model.Questions.Count)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            var survey = (await AppHelper.SurveyAsync())
                                         .FirstOrDefault(i => i.SurveyId == surveyId);
            var questions = await AppHelper.QuestionAsync(surveyId, survey!.SurveyAmount);
            var userFinance = await userSerice.GetUserFinancial(LoginUserName);


            model.UserName = userFinance.UserName;
            model.UserBalance = userFinance.UserBalance.ToSafeDecimalFloor2();
            model.TeamComission = userFinance.TeamComission.ToSafeDecimalFloor2();
            model.TodayIncome = userFinance.TodayIncome.ToSafeDecimalFloor2();

            model.Survey = survey;
            model.Questions = questions;



            return View(model);
        }


        public async Task<IActionResult> SurveyLogs()
        {
            var surveyLogs = await surveyService.GetSurveyLogsAsync(LoginUserName);
            return View(surveyLogs);
        }

        #region Private Methods
        public async Task<string> IsNextSurveyActive()
        {
            string msg = string.Empty;

            //Rule 1: survey will be active if his max deposit amount >= Configuration deposit amount
            var deposityAmoutnConfig = await surveyService.GetSurveyActiveDepositAmount();



            var lastSurvey = await surveyService.GetLastSurvey(LoginUserName);
            if (lastSurvey == null)
                return string.Empty;

            var nextSurveyConfig = await surveyService.GetNextSurveyConfig();

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
        #endregion
    }
}
