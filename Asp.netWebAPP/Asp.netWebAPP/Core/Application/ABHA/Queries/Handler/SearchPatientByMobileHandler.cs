
using Microsoft.EntityFrameworkCore;


using Asp.netWebAPP.Core.Application.DTO_s;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Core.Domain.Model;
namespace Asp.netWebAPP.Core.Application.ABHA.Queries.Handler
{
    public class SearchPatientByMobileHandler
    {
        private readonly IAbhaLoginService _service;

        public SearchPatientByMobileHandler(IAbhaLoginService service)
        {
            _service = service;
        }

        public async Task<List<PatientSerachDTO>> Handle(SearchPatientByMobileQuery query)
        {
            return await _service.SearchPatientByMobile(query.Mobile);
        }
    
    }
}
