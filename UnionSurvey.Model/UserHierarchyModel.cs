using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class UserHierarchyModel
    {
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string UserCode { get; set; } = null!;
        public string ReferalCode { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int Level { get; set; }
        public decimal UserBalance { get; set; }
        public decimal TodayIncome { get; set; }
        public decimal TeamComission { get; set; }
        public decimal AvailableBalance => UserBalance + TeamComission;
        public DateOnly? LastSurvey { get; set; }
        public decimal TeamBalance { get; set; }
        public int DirectPromoters { get; set; }
        public int TotalSize { get; set; }
        public decimal DepositBalance { get; set; }
        public int NewlyIncreased { get; set; }
        public int TodaySurveyCompletedUsers { get; set; }
        public string Name => string.IsNullOrEmpty(LastName)
                            ? FirstName
                            : $"{FirstName} {LastName}";


        public IList<UserHierarchyModel> Users = [];
    }
}
