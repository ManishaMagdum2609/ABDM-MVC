namespace Asp.netWebAPP.Core.Application.ABHA.Commands
{
    public class RequestRegisterOtpCommand
    {
        public string AadhaarNumber { get; }

        public RequestRegisterOtpCommand(string aadhaarNumber)
        {
            AadhaarNumber = aadhaarNumber;
        }
    }
}
