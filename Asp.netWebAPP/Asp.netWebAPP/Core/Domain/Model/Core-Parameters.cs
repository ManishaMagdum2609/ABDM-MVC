using System.ComponentModel.DataAnnotations;

namespace Asp.netWebAPP.Core.Domain.Model
{
    public class AbdmCoreParameters
    {
        [Key]
        public int ParameterId { get; set; }
        public string ParameterGroupName { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ValueDataType { get; set; }
        public string Description { get; set; }
    }
}
