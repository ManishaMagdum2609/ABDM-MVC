namespace Asp.netWebAPP.Core.Domain.Model
{
        public class AbhaAccount
        {
            public string txnId { get; set; }
            public List<AbhaEntry> ABHA { get; set; } = new List<AbhaEntry>();
        }
    }



