using Asp.netWebAPP.Core.Application.Interface; 
using Asp.netWebAPP.Core.Domain.Model;

namespace Asp.netWebAPP.Core.Application.ABHA.Queries.Handler
{
    public class SearchAbhaHandler 
    {
        private readonly IAbhaLoginService _service;

        public SearchAbhaHandler(IAbhaLoginService service)
        {
            _service = service;
        }

        public async Task<List<AbhaAccount>> Handle(SearchAbhaQuery query)
        {
            return await _service.SearchAbhaAsync(query.Mobile);
        }
    }
}