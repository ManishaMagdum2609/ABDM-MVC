using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Domain.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Asp.netWebAPP.Core.Application.Interface
{
    public interface IAbhaRegistrationService
    {
       
        Task<OtpResponse> RequestRegisterOtpAsync(string AadhaarNumber);
       
        Task<VerifyRegisterOtpResponse> VerifyAbhaRegistrationAsync(string txnId, 
            string otp, string mobile);

    }
}
