using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class SurveyUnionConfig
{
    public string ConfigName { get; set; } = null!;

    public string ConfigVal { get; set; } = null!;

    public string? Description { get; set; }
}
