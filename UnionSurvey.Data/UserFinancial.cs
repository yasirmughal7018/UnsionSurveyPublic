using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Data.Models;

public partial class UserFinancial
{
    public decimal AvailableBalance => UserBalance + TeamComission;
    [NotMapped]
    public int ActiveUsers { get; set; }
}
