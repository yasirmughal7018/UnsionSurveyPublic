using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class UserFinancialComission
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string ComissionFrom { get; set; } = null!;

    public decimal Amount { get; set; }

    public decimal Percentage { get; set; }

    public int Grade { get; set; }
}
