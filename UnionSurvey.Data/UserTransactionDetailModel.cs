using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Data
{
    public class UserTransactionDetailModel
    {
        public UserFinancial UserFinancial { get; set; } = new();
        public IList<SP_FinancialLog> TranactionLogs { get; set; } = [];

    }
}
