namespace Asp.netWebAPP.Core.Application.ABHA.Commands
{
    public class RequestOtpLoginCommand
    {
        public int Index { get; }
        public string TxnId { get; }

        public RequestOtpLoginCommand(int index, string txnId)
        {
            Index = index;
            TxnId = txnId;
        }
    }
}
