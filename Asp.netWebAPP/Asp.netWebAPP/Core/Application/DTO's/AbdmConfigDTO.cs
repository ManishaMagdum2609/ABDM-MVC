namespace Asp.netWebAPP.Core.Application.DTO_s
{
    public class AbdmConfigDTO
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string tokenUrl { get; set; }
        public string publicKeyUrl { get; set; }
        public string abhaOTPrequestUrl { get; set; }
        public string abhaCreationUrl { get; set; }
        public string abhaLoginUrl { get; set; }
        public string abhaLoginOTPRequestUrl { get; set; }
        public string searchAbhaUrl { get; set; }
        //add here in db  ...Snehal
        public string registerBridgeServiceUrl { get; set; }
        public string careContextLinkUrl { get; set; }
        public string hipId { get; set; }
        public string generateLinkTokenUrl { get; set; }
    }
}
