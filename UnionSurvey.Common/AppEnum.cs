using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YM.Common;

namespace UnionSurvey.Common
{
    public static class Constant
    {
        public const string US_ADMIN_REFERAL_CODE = "YSADMIN786";
        public const int US_DEFAULT_USER_ID = 1;
        public const string CURRENT_USERID = "UNIONSURVEY_CURRENT_USER_ID";
        public const string WITHDRAW_INSUFFICENT_BALANCE = "Your balance is not sufficient for withdrawal.";


        public static string GenerateTransactionNumber(this int companyId)
            => $"{companyId}{"TX".ToSafeUniqueCode(8)}";

        public  static decimal SurveyProfitAmount(this decimal surveyAmount, decimal profitvalue)
                    => (surveyAmount * profitvalue) == 0
                        ? 0
                        : surveyAmount * profitvalue / 100;

        public enum TransctionType
        {
            IN,
            OUT
        }

        public enum TransactionStatus
        {
            START,
            INPROGRESS,
            COMPLETED,
            FAIL,
            CANCEL
        }



    }
}
