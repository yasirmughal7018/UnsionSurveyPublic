using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class ScheduledJobLog
{
    public int Id { get; set; }

    public int ScheduledJobId { get; set; }

    public DateTime ScheduledJobDate { get; set; }

    public string Status { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ScheduledJob ScheduledJob { get; set; } = null!;
}
