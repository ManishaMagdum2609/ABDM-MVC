using Asp.netWebAPP.Core.Application.DTO_s;
using System.Text.Json;

namespace Asp.netWebAPP.Infrastructure.Security
{
    public class AbdmConfigMapper
    {
        public static AbdmConfigDTO Map(string parameterValueJson)
        {
            if (string.IsNullOrWhiteSpace(parameterValueJson))
                throw new Exception("ABDM Config value is empty.");

            return JsonSerializer.Deserialize<AbdmConfigDTO>(parameterValueJson)
                   ?? throw new Exception("Failed to deserialize ABDM config.");
        }
    }
}
