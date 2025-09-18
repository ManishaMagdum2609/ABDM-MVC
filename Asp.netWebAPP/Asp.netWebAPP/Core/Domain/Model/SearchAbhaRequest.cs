namespace Asp.netWebAPP.Core.Domain.Model
{
    public class SearchAbhaRequest
    {
        public string Mobile { get; set; }
    }
    public class RequestOtpRequest 
    { 
        public int Index { get; set; } 
        public string TxnId { get; set; }
    }
    public class VerifyOtpRequest 
    { 
        public string TxnId { get; set; }  
        public string Otp { get; set; } 
        public string Mobile { get; set; } }
}
