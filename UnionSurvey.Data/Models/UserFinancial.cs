using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class UserFinancial
{
    public string UserName { get; set; } = null!;

    public decimal UserBalance { get; set; }

    public decimal TodayIncome { get; set; }

    public decimal TeamComission { get; set; }

    public DateTime? LastSurveyDate { get; set; }

    public DateTime? LastDepositDate { get; set; }

    public DateTime? LastWithdrawDate { get; set; }

    public virtual AppUser? AppUser { get; set; }
}
