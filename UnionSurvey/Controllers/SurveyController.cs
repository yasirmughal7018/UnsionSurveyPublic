using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UnionSurvey.Business.Services;
using UnionSurvey.Common;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Controllers
{
    public class SurveyController(ISurveyService surveyService
                                    , IWebHostEnvironment environment
                                    , ILogger<AccountController> logger)
                    : AdminController(logger)
    {

        public async Task<IActionResult> Index()
        {
           var surveys = await surveyService.GetAllAsync();

            SurveyModel model = new()
            {
                SurveyList = surveys
            };

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(SurveyModel model)
        {
            if (ModelState.IsValid)
            {
                Survey survey = new()
                {
                    SurveyId = model.SurveyId,
                    SurveyAmount = model.SurveyAmount.ToSafeDecimalRound0(),
                    Description = model.Description,
                    Title = model.Title,
                    UsersForSurveyActive = model.UsersForSurveyActive,
                };
                survey.DomainUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                bool isExist = await surveyService.IsSurveyExist(model.Title);
                if (isExist == false)
                {
                    await surveyService.SaveAsync(survey, model.LogoFile!, environment.WebRootPath, "");
                    await AppHelper.SetSurveyAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                    ModelState.AddModelError(string.Empty, $"{model.Title} is can't be duplicate.");
            }

            model.SurveyList = await surveyService.GetAllAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSurvey(int surveyId)
        {
            if (surveyId > 0)
            {

                await surveyService.DeleteAsync(surveyId, userName: LoginUserName);
                await AppHelper.SetSurveyAsync();

                return Json(new { success = true, message = "Record updated successfully." });
            }

            return Json(new { success = false, message = "Error" });
        }

        public async Task<IActionResult> Question(int id)
        {
            IList<QuestionModel> questions = await AppHelper.QuestionAsync(id);
            SurveyModel survey = await surveyService.GetById(id);

            survey.QuestionList = questions;

            return View(survey);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSurveyQuestion(string questionIds, int surveyId)
        {
            if (questionIds.ToSafeString() != "" && surveyId > 0)
            {
                await surveyService.SaveSurveyQuestionAsync(questionIds, surveyId);
                await AppHelper.SetQuestionAsync();

                return Json(new { success = true, message = "Record save sucessfuly." });
            }
            return Json(new { success = true, message = "Error: Please select survey or question for save." });
        }

    }
}
