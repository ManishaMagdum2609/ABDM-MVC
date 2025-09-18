using Asp.netWebAPP.Core.Domain.Value_Objects;
namespace Asp.netWebAPP.Core.Application.DTO_s
{
    public class VerifyOtpResponse
    {
        public string TxnId { get; set; }
        public List<AbhaAccountDTO> Accounts { get; set; }
    }
   
}
