using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class SP_UserFinancialStatus
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string UserCode { get; set; } = null!;
        public string ReferalCode { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal UserBalance { get; set; }
        public decimal TeamComission { get; set; }
        public decimal TodayIncome { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? TransactionId { get; set; }
        public string? TransactionStatus { get; set; }
        public DateOnly? TransactionDate { get; set; }
        public decimal AvailableBlance => UserBalance + TeamComission;
        public DateTime UpdatedReordDate => ModifiedDate ?? CreatedDate;
        public string Name => string.IsNullOrEmpty(LastName)
                                    ? FirstName 
                                    : $"{FirstName} {LastName}";
    }
}
