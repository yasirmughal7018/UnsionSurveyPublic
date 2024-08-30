using Microsoft.AspNetCore.DataProtection;

namespace UnionSurvey.Services
{
    public class SensitiveDataService(IDataProtectionProvider dataProtectionProvider)
    {
        private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector("SensitiveDataProtection");

        public string Protect(string sensitiveData)
        {
            return _protector.Protect(sensitiveData);
        }

        public string Unprotect(string protectedData)
        {
            return _protector.Unprotect(protectedData);
        }
    }
}
