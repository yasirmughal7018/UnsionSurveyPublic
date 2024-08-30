using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using UnionSurvey.Business.Services;
using UnionSurvey.Common;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Controllers
{
    public class QuestionController(IQuestionService questionService
                                   , ILogger<AccountController> logger)
                    : AdminController(logger)
    {
        public async Task<IActionResult> Index()
        {
            QuestionModel questionModel = new()
            {
                QuestionList = await AppHelper.QuestionAsync(0)
            };
            return View(questionModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(QuestionModel model)
        {

            if (ModelState.IsValid && (model.Option1.ToHasValue()
                                               || model.Option2.ToHasValue()
                                               || model.Option3.ToHasValue()
                                               || model.Option4.ToHasValue()))
            {
                Question question = new()
                {
                    QuestionId = model.QuestionId,
                    Question1 = model.Question,
                    Option1 = model.Option1 ?? "",
                    Option2 = model.Option2,
                    Option3 = model.Option3,
                    Option4 = model.Option4,
                };

                await questionService.SaveAsync(question, userName: LoginUserName);
                await AppHelper.SetQuestionAsync();

                return RedirectToAction(nameof(Index));
            }

            model.QuestionList = await AppHelper.QuestionAsync(0);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int questionId)
        {
            if (questionId > 0)
            {
                var question = await questionService.GetByIdAsync(questionId);

                return Json(new { success = true, Question = question });
            }

            return Json(new { success = false, message = "Error" });
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int questionId)
        {
            if (questionId > 0)
            {
                await questionService.DeleteAsync(questionId, userName: LoginUserName);
                await AppHelper.SetQuestionAsync();

                return Json(new { success = true, message = "Record deleted successfully." });
            }

            return Json(new { success = false, message = "Error" });
        }

        public async Task<IActionResult> Import()
        {
            SurveyImportModel model = new()
            {
                Surveys = await Lookups.Surveys()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(SurveyImportModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await questionService.ImportSruveyAndQuestionAsync(model.QuestionFile, model.SurveyId, LoginUserName);
                    await AppHelper.SetSurveyAsync();
                    return RedirectToAction(nameof(Import));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

            }

            model.Surveys = await Lookups.Surveys();

            return View(model);
        }


    }
}
