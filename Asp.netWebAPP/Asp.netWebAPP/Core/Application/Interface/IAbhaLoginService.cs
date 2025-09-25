using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Domain.Model;
namespace Asp.netWebAPP.Core.Application.Interface
{
    public interface IAbhaLoginService

    {
        Task<List<AbhaAccount>> SearchAbhaAsync(string mobile);
        Task<OtpResponse> RequestOtpLoginAsync(int index, string txnId);
        Task<VerifyOtpResponse> VerifyAbhaLoginAsync(string txnId, string otp);
        Task<List<PatientSerachDTO>> SearchPatientByMobile(string mobile);
    }
}
