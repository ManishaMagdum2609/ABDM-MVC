using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Asp.netWebAPP.Core.Domain.Model
{
    [Table("PAT_Patient")] 
    public class PatientModel
    {
        [Key]
        public int PatientId { get; set; } 

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EHRNumber { get; set; }
        
    }
}
