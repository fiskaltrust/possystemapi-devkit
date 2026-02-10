using System.Collections.Generic;
using System.Text;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class ErrorResponse
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }

        public Dictionary<string, object>? Errors { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in Errors ?? new Dictionary<string, object>())
            {
                sb.AppendLine($"  {error.Key}={error.Value}");
            }
            return $"ErrorResponse: Title='{Title}', Status='{Status}', Errors='{sb}'";
        }
    }
}