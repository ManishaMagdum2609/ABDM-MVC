namespace Asp.netWebAPP.Core.Application.ABHA.Commands
{
    public class VerifyOtpCommand
    {
        public string TxnId { get; }
        public string Otp { get; }

        public VerifyOtpCommand(string txnId, string otp)
        {
            TxnId = txnId;
            Otp = otp;
        }
    }
}
