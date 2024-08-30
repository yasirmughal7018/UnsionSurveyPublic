using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class SurveyQuestion
{
    public int Id { get; set; }

    public int SurveyId { get; set; }

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual Survey Survey { get; set; } = null!;
}
