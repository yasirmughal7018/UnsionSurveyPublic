using System.Reflection.Metadata.Ecma335;
using UnionSurvey.Common;

namespace YM.Common
{
    public static class Utilities
    {
        public static string ToSafeEightUniqueCode()
        {
            Guid guid = Guid.NewGuid();
            int hash = guid.GetHashCode();
            string code = Math.Abs(hash).ToString("D8")[..8]; // ensure 8 digits
            return code;
        }
        public static string ToSafeFourUniqueCode()
        {
            Guid guid = Guid.NewGuid();
            int hash = guid.GetHashCode();
            string code = Math.Abs(hash).ToString("D4")[..4]; // ensure 4 digits
            return code;
        }
        private static string ToSafeUniqueCode(this short level) => level == 8
                                                                    ? ToSafeEightUniqueCode()
                                                                    : ToSafeFourUniqueCode();


        public static string ToSafeInitial(this string value)
                        =>
                            (string.IsNullOrEmpty(value) ? "" :
                                value.Length == 1 ? value[..1] : value[..2])
                            .Trim()
                            .ToUpper();
        public static string ToSafeUniqueCode(this string value, short level = 8)
                            => $"{value.ToSafeInitial()}{level.ToSafeUniqueCode()}";

        public static bool IsValidNumber(this string value)
        {
            return string.IsNullOrEmpty(value.ToSafeString()) || value.All(char.IsDigit);
        }
        public static bool IsFromAndToDateValid(this DateTime fromDate, DateTime toDate, bool isFullDateValidation = false)
        {

            if (fromDate.Date >= toDate.Date)
                return false;

            return true;
        }
        public static string ToSafeSubstringEnd(this string value, int length)
        {
            value = value.ToSafeString();
            if (value.Length <= length)
                return value;
            else
                return value[^length..];
        }

        public static string? FirstNonEmpty(string[] strings, short start, out short match)
        {
            match = 0;
            foreach (var str in strings)
            {
                match++;
                if (match >= start && !string.IsNullOrEmpty(str))
                {
                    return str.ToSafeString();
                }
            }
            return null;
        }
      
        public static decimal CalculatePureGoldWeight(decimal rati, decimal grossWeight)
            => (grossWeight / 96 * rati).ToSafeDecimalRound3();

        public static List<decimal> GenerateRandomList(this decimal total, int count, decimal percentageVal)
        {
            decimal profit = total.SurveyProfitAmount(6m);
            Random rand = new Random();
            List<decimal> randomNumbers = new List<decimal>();
            decimal sum = 0m;

            for (int i = 0; i < count - 1; i++)
            {
                // Generate a random decimal between 0 and the remaining total divided by the remaining slots
                decimal max = (profit - sum) / (count - i);
                decimal val = Math.Round((decimal)rand.NextDouble() * max, 2);
                randomNumbers.Add(val);
                sum += val;
            }

            // Add the remaining amount to make sure the total is correct
            randomNumbers.Add(Math.Round(profit - sum, 2));

            return randomNumbers;
        }

    }
}
