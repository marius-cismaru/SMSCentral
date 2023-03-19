using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSCentral.Models
{
    public class Receiver
    {
        public string PhoneNumber { get;set; } = string.Empty;

        public string Name { get;set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public int? OperatorId { get; set; }

        public DateTime? DateRefreshOperator { get; set; }

        public DateTime? DateSendSMS { get; set; }

        [JsonIgnore]
        public Operator Operator { get; set; }

        [JsonIgnore]
        public string OperatorDescription { get { return Operator?.Name ?? string.Empty; } }

        [JsonIgnore]
        public string DateSendSMSDescription { get { return DateSendSMS.HasValue ? DateSendSMS.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; } }

        [JsonIgnore]
        public string DateRefreshOperatorDescription { get { return DateRefreshOperator.HasValue ? DateRefreshOperator.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; } }
    }
}
