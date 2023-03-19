using SMSCentral.Models;
using SMSCentral.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SMSCentral.Infrastructure
{
    public class DataContext
    {
        public OperatorsDataService Operators { get; set; }
        public ModemsDataService Modems { get; set; }
        public ReceiversDataService Receivers { get; set; }

        public DataContext()
        {
            Operators = new OperatorsDataService();
            Modems = new ModemsDataService(Operators);
            Receivers = new ReceiversDataService(Operators);
        }

        public async Task<Receiver> GetReceiverOperatorAsync(Receiver receiver)
        {
            string url = "http://www.portabilitate.ro/ro-no-" + receiver.PhoneNumber;

            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;

                    request.RequestUri = new Uri(url);

                    var response = client.SendAsync(request).Result;
                    var htmlResponse = await response.Content.ReadAsStringAsync();

                    var opratorName = htmlResponse.Split(new string[] { "<a id=\"ctl00_cphBody_lnkOperator\">" }, StringSplitOptions.None)[1].Split(new string[] { "</a>" }, StringSplitOptions.None)[0];

                    receiver.Operator = Operators.Items.FirstOrDefault(o=>o.PortabilitateRoName == opratorName);
                }
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(GetReceiverOperatorAsync), ex.Message);
            }

            return receiver;
        }
    }
}
