using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class ScheduledJob
{
    public int Id { get; set; }

    public string JobName { get; set; } = null!;

    public TimeOnly SchedualTime { get; set; }

    public DateOnly LastExecutedDate { get; set; }

    public DateTime? NextJobDate { get; set; }

    public virtual ICollection<ScheduledJobLog> ScheduledJobLogs { get; set; } = new List<ScheduledJobLog>();
}
