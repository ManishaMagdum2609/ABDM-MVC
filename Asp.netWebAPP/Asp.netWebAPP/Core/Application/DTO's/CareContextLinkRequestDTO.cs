namespace Asp.netWebAPP.Core.Application.DTO_s
{
    public class CareContextLinkRequestDTO
    {
        public string AbhaNumber { get; set; }
        public string AbhaAddress { get; set; }
        public List<PatientDTO> Patient { get; set; }
    }

    public class PatientDTO
    {
        public string ReferenceNumber { get; set; }
        public string Display { get; set; }
        public List<CareContextDTO> CareContexts { get; set; }
        public string HiType { get; set; }
        public int Count { get; set; }
    }
    public class CareContextDTO
    {
        public string ReferenceNumber { get; set; }
        public string Display { get; set; }
    }

}
