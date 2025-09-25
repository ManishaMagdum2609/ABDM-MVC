namespace Asp.netWebAPP.Core.Application.ABHA.Queries
{
    public class SearchPatientByMobileQuery
    {
        public string Mobile { get; }

        public SearchPatientByMobileQuery(string mobile)
        {
            Mobile = mobile;
        }
    }
}
