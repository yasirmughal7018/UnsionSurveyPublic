using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class SurveyUserMapping
{
    public int Id { get; set; }

    public int SurveyId { get; set; }

    public string UserName { get; set; } = null!;

    public bool IsSurveyCompleted { get; set; }

    public DateTime CompletedDate { get; set; }

    public bool? IsCommisionCalculated { get; set; }

    public virtual Survey Survey { get; set; } = null!;
}
