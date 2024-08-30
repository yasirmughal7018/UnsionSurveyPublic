
using Newtonsoft.Json;
using UnionSurvey.Business.Services;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Common
{
    public static class AppHelper
    {
        private static readonly object _lock = new object();
        public static async Task<IList<QuestionModel>> QuestionAsync(int surveyId)
        {
            IList<QuestionModel> questions = [];
            //var currentContext = HttpContextHelper.Current;

            //lock (_lock)
            //{
            //    if (currentContext != null)
            //        questions = currentContext?.Session.GetObjectFromJson<List<QuestionModel>>("UNIONSURVEY_QUESTIONS") ?? [];
            //}


            //if (questions == null || questions.Count == 0 || !questions.Any(i => i.SurveyId == surveyId))
            //{
            //    var questionService = ServiceAccessor.GetService<IQuestionService>();
            //    questions = await questionService.GetAllAsync();

            //    lock (_lock)
            //        currentContext?.Session.SetObjectAsJson("UNIONSURVEY_QUESTIONS", questions);
            //}

            var questionService = ServiceAccessor.GetService<IQuestionService>();
            questions = await questionService.GetAllAsync();


            var surveyQeustions = questions
                                .Where(i => i.SurveyId == surveyId || i.SurveyId == 0)
                                .ToList();

            var availableQuestion = questions.Select(i => new QuestionModel
            {
                QuestionId = i.QuestionId,
                Question = i.Question,
                Option1 = i.Option1,
                Option2 = i.Option2,
                Option3 = i.Option3,
                Option4 = i.Option4,
            })
                .DistinctBy(i => i.QuestionId)
                .ToList();

            var mappedQuestion = (from q in availableQuestion
                                  let i = 0
                                  join s in surveyQeustions on q.QuestionId equals s.QuestionId into g
                                  from s in g.DefaultIfEmpty()
                                  select new QuestionModel
                                  {
                                      QuestionId = q.QuestionId,
                                      Question = q.Question,
                                      Option1 = q.Option1,
                                      Option2 = q.Option2,
                                      Option3 = q.Option3,
                                      Option4 = q.Option4,
                                      SurveyId = s != null ? s.SurveyId : 0,
                                      Amount = 0,
                                      SpendTime = 0,

                                  })
                                  .OrderByDescending(i => i.SurveyId)
                                  .ToList();

            return mappedQuestion;
        }
        public static async Task<IList<QuestionModel>> QuestionAsync(int surveyId, decimal surveyAmount)
        {
            IList<QuestionModel> questions = [];
            //var currentContext = HttpContextHelper.Current;

            //lock (_lock)
            //{
            //    if (currentContext != null)
            //        questions = currentContext?.Session.GetObjectFromJson<List<QuestionModel>>("UNIONSURVEY_QUESTIONS") ?? [];
            //}


            //if (questions == null || questions.Count == 0 || !questions.Any(i => i.SurveyId == surveyId))
            //{
            //    var questionService = ServiceAccessor.GetService<IQuestionService>();
            //    questions = await questionService.GetAllAsync();

            //    lock (_lock)
            //        currentContext?.Session.SetObjectAsJson("UNIONSURVEY_QUESTIONS", questions);
            //}

            var questionService = ServiceAccessor.GetService<IQuestionService>();
            questions = await questionService.GetAllAsync();

            var surveyQeustions = questions
                                .Where(i => i.SurveyId == surveyId)
                                .Select(i => new QuestionModel
                                {
                                    QuestionId = i.QuestionId,
                                    Question = i.Question,
                                    Option1 = i.Option1,
                                    Option2 = i.Option2,
                                    Option3 = i.Option3,
                                    Option4 = i.Option4,
                                    SurveyId = surveyId,
                                    Amount = 0,
                                    SpendTime = 0,
                                })
                                .DistinctBy(i => i.QuestionId)
                                .ToList();


            IList<decimal> randSurveyAmount = [];
            var randSurveyAmountIndex = 0;
            if (surveyAmount > 0)
                randSurveyAmount = surveyAmount.GenerateRandomList(surveyQeustions.Count, 0.6m);

            foreach (var question in surveyQeustions)
            {
                question.Amount = randSurveyAmount[randSurveyAmountIndex++];
            }

            return surveyQeustions;
        }
        public static async Task SetQuestionAsync()
        {
            var currentContext = HttpContextHelper.Current;

            if (currentContext != null)
            {
                var questionService = ServiceAccessor.GetService<IQuestionService>();
                var questions = await questionService.GetAllAsync();

                lock (_lock)
                {
                    currentContext.Session.Remove("UNIONSURVEY_QUESTIONS");
                    currentContext.Session.SetObjectAsJson("UNIONSURVEY_QUESTIONS", questions);
                }
            }
        }
        public static void RemoveQuestionAsync()
        {
            var currentContext = HttpContextHelper.Current;

            if (currentContext != null)
            {
                lock (_lock)
                {
                    currentContext.Session.Remove("UNIONSURVEY_QUESTIONS");
                }
            }
        }


        public static async Task<IList<SurveyModel>> SurveyAsync()
        {
            //IList<SurveyModel> surveys = [];
            //var currentContext = HttpContextHelper.Current;

            //lock (_lock)
            //{
            //    if (currentContext != null)
            //        surveys = currentContext?.Session.GetObjectFromJson<List<SurveyModel>>("UNIONSURVEY_SURVEYS") ?? [];
            //}


            //if (surveys == null || surveys.Count == 0)
            //{
            //    var surveyService = ServiceAccessor.GetService<ISurveyService>();
            //    surveys = await surveyService.GetAllAsync();

            //    lock (_lock)
            //        currentContext?.Session.SetObjectAsJson("UNIONSURVEY_SURVEYS", surveys);
            //}

            var surveyService = ServiceAccessor.GetService<ISurveyService>();
            var surveys = await surveyService.GetAllAsync();

            return surveys;
        }
        public static async Task SetSurveyAsync()
        {
            var currentContext = HttpContextHelper.Current;

            if (currentContext != null)
            {
                var surveyService = ServiceAccessor.GetService<ISurveyService>();
                var surveys = await surveyService.GetAllAsync();

                lock (_lock)
                {
                    currentContext.Session.Remove("UNIONSURVEY_SURVEYS");
                    currentContext.Session.SetObjectAsJson("UNIONSURVEY_SURVEYS", surveys);
                }
            }
        }
        public static void RemoveSurveyAsync()
        {
            var currentContext = HttpContextHelper.Current;

            if (currentContext != null)
            {
                lock (_lock)
                {
                    currentContext.Session.Remove("UNIONSURVEY_SURVEYS");
                }
            }
        }

    }


    public static class HttpContextHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext? Current => _httpContextAccessor?.HttpContext;

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null
                    ? default
                    : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public static class ServiceAccessor
    {
        private static IServiceScopeFactory? _serviceScopeFactory;

        public static void Configure(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public static T GetService<T>() where T : notnull
        {
            if (_serviceScopeFactory == null)
            {
                throw new InvalidOperationException("Service provider is not configured.");
            }

            //using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scope = _serviceScopeFactory.CreateScope();
                return scope.ServiceProvider.GetRequiredService<T>();
            }
        }
    }
}
