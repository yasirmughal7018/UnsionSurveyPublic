using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Data
{
    public class SP_AdminDashboardStat
    {
        public int Id { get; set; }
        public decimal TodayLoad { get; set; }
        public decimal TodayOutRequest { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalOut { get; set; }
        public decimal BalanceInWallet { get; set; }
        public int TotalActiveUsers { get; set; }
        public decimal TodayEarning { get; set; }
        public decimal TotalEarning { get; set; }

    }
}
