using UnionSurvey.Model;

namespace UnionSurvey.Common
{
    public static class Lookups
    {
        public static async Task<IList<SelectItem>> Surveys()
        {
            var surveys = await AppHelper.SurveyAsync();

            var selectItems = surveys
                .Select(i => new SelectItem { Id = i.SurveyId, Name = i.Title })
                .OrderByDescending(i => i.Name)
                .ToList();

            return selectItems;
        }
    }
}
