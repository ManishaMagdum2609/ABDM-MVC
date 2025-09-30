namespace Asp.netWebAPP.Core.Application.ABHA.Commands
{
    public class VerifyRegisterOtpCommand
    {
        public string TxnId { get; }
        public string Otp { get; }
        public string Mobile { get; }

        public VerifyRegisterOtpCommand(string txnId, string otp, string mobile)
        {
            TxnId = txnId;
            Otp = otp;
            Mobile = mobile;
        }
    }
}
