using Newtonsoft.Json;
using SMSCentral.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SMSCentral.Services
{
    public class DataService
    {
        public static string OperatorsFilePath => Path.GetFullPath("Data/Operators.json");
        public static string ModemsFilePath = Path.GetFullPath("Data/Modems.json");
        public static string ReceiversFilePath = Path.GetFullPath("Data/Receivers.json");

        public List<Operator> Operators = new List<Operator>();
        public List<Modem> Modems = new List<Modem>();
        public List<Receiver> Receivers = new List<Receiver>();

        public DataService()
        {
            LoadOperators();
            LoadModems();
            LoadReceivers();
        }

        public void LoadOperators()
        {
            try
            {
                var items = JsonConvert.DeserializeObject<List<Operator>>(File.ReadAllText(OperatorsFilePath));

                if (items != null)
                {
                    Operators = items;
                }
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(LoadOperators), ex.Message);
            }
        }

        public void SaveOperators()
        {
            try
            {
                File.WriteAllText(OperatorsFilePath, JsonConvert.SerializeObject(Operators));
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(SaveOperators), ex.Message);
            }
        }

        public void LoadModems()
        {
            try
            {
                var items = JsonConvert.DeserializeObject<List<Modem>>(File.ReadAllText(ModemsFilePath));

                if (items != null)
                {
                    items.ForEach(o =>
                    {
                        o.Operator = Operators.FirstOrDefault(p => p.Id == o.OperatorId);
                    });

                    Modems = items;
                }
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(LoadModems), ex.Message);
            }
        }

        public void SaveModems()
        {
            try
            {
                File.WriteAllText(ModemsFilePath, JsonConvert.SerializeObject(Modems));
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(SaveModems), ex.Message);
            }
        }

        public void LoadReceivers()
        {
            try
            {
                var items = JsonConvert.DeserializeObject<List<Receiver>>(File.ReadAllText(ReceiversFilePath));

                if (items != null)
                {
                    items.ForEach(o =>
                    {
                        o.Operator = Operators.FirstOrDefault(p => p.Id == o.OperatorId);
                    });

                    Receivers = items;
                }
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(LoadReceivers), ex.Message);
            }
        }

        public void SaveReceivers()
        {
            try
            {
                File.WriteAllText(ReceiversFilePath, JsonConvert.SerializeObject(Receivers));
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(SaveReceivers), ex.Message);
            }
        }

        //public async Task<Operator> GetOperatorForPhoneNumberAsync(string phoneNumber)
        //{
        //    string url = "http://www.portabilitate.ro/ro-no-" + phoneNumber;

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        using (var request = new HttpRequestMessage())
        //        {
        //            request.Method = HttpMethod.Get;

        //            request.RequestUri = new Uri(url);

        //            var response = client.SendAsync(request).Result;
        //            var htmlResponse = await response.Content.ReadAsStringAsync();

        //            var operatorName = htmlResponse.Split(new string[] { "<a id=\"ctl00_cphBody_lnkOperator\">" }, StringSplitOptions.None)[1].Split(new string[] { "</a>" }, StringSplitOptions.None)[0];

        //            return Operators.FirstOrDefault(o => o.PortabilitateRoName == operatorName);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(GetOperatorForPhoneNumberAsync), ex.Message);

        //        return null;
        //    }
        //}

        public void DeleteReceivers(List<Receiver> items)
        {
            foreach (var item in items)
            {
                var receiver = Receivers
                    .FirstOrDefault(o => o.PhoneNumber == item.PhoneNumber);

                if (receiver != null)
                {
                    Receivers.Remove(receiver);
                }
            }
        }

        public void RefreshOperatorForReceiver(Receiver item)
        {
            if (item != null)
            {
                var receiver = Receivers
                    .FirstOrDefault(o => o.PhoneNumber == item.PhoneNumber);

                if (receiver != null)
                {
                    string url = "http://www.portabilitate.ro/ro-no-" + item.PhoneNumber;

                    try
                    {
                        using (var client = new HttpClient())
                        using (var request = new HttpRequestMessage())
                        {
                            request.Method = HttpMethod.Get;

                            request.RequestUri = new Uri(url);

                            var response = client.SendAsync(request).Result;
                            var htmlResponse = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

                            var operatorName = htmlResponse.Split(new string[] { "<a id=\"ctl00_cphBody_lnkOperator\">" }, StringSplitOptions.None)[1].Split(new string[] { "</a>" }, StringSplitOptions.None)[0];

                            var operatorRefreshed = Operators
                                .FirstOrDefault(o => o.PortabilitateRoName == operatorName);

                            if (operatorRefreshed != null)
                            {
                                receiver.OperatorId = operatorRefreshed.Id;
                                receiver.Operator = operatorRefreshed;
                                receiver.DateRefreshOperator = DateTime.Now;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(RefreshOperatorForReceiver) + " -> Phone number: " + item.PhoneNumber, ex.Message);
                    }
                }
            }
        }

        public bool SendSMS(Receiver item, string message)
        {
            var result = false;

            if (Modems.Count > 0 && Modems.Any(o => o.IsConnected.HasValue && o.IsConnected.Value == true))
            {
                if (item != null)
                {
                    var receiver = Receivers
                        .FirstOrDefault(o => o.PhoneNumber == item.PhoneNumber);

                    if (receiver != null)
                    {
                        var modem = Modems
                            .FirstOrDefault(o => o.OperatorId == receiver.OperatorId && o.IsConnected.HasValue && o.IsConnected.Value == true);

                        if (modem is null)
                        {
                            modem = Modems
                                .Where(o => o.IsConnected.HasValue && o.IsConnected.Value == true)
                                .OrderBy(o => o.Priority)
                                .FirstOrDefault();
                        }

                        if (modem != null)
                        {
                            if (modem.SendSMS(receiver, message))
                            {
                                receiver.DateSendSMS = DateTime.Now;
                                result = true;
                            }
                            else
                            {
                                modem = Modems
                                    .Where(o => o.IsConnected.HasValue && o.IsConnected.Value == true && o.PhoneNumber != modem.PhoneNumber)
                                    .OrderBy(o => o.Priority)
                                    .FirstOrDefault();

                                if (modem.SendSMS(receiver, message))
                                {
                                    receiver.DateSendSMS = DateTime.Now;
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task RefreshOperatorsForReceiversAsync(List<Receiver> receivers)
        {
            string url = "http://www.portabilitate.ro/ro-no-";

            try
            {
                using (var client = new HttpClient())
                {
                    var phoneNumbers = receivers
                        .Where(o => !string.IsNullOrWhiteSpace(o.PhoneNumber))
                        .Select(o => o.PhoneNumber)
                        .ToList();

                    foreach (var phoneNumber in phoneNumbers)
                    {
                        var receiver = Receivers
                            .FirstOrDefault(o => o.PhoneNumber == phoneNumber);

                        if (receiver != null)
                        {
                            try
                            {
                                using (var request = new HttpRequestMessage())
                                {
                                    request.Method = HttpMethod.Get;

                                    request.RequestUri = new Uri(url + phoneNumber);

                                    var response = client.SendAsync(request).Result;
                                    var htmlResponse = await response.Content.ReadAsStringAsync();


                                    var operatorName = htmlResponse.Split(new string[] { "<a id=\"ctl00_cphBody_lnkOperator\">" }, StringSplitOptions.None)[1].Split(new string[] { "</a>" }, StringSplitOptions.None)[0];

                                    var operatorRefreshed = Operators
                                        .FirstOrDefault(o => o.PortabilitateRoName == operatorName);

                                    if ((operatorRefreshed != null) && ((receiver.Operator == null) || (receiver.Operator.Id != operatorRefreshed.Id)))
                                    {
                                        receiver.Operator = operatorRefreshed;
                                        receiver.DateRefreshOperator = DateTime.Now;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(RefreshOperatorsForReceiversAsync) + " -> Get operator for phone number: " + phoneNumber, ex.Message);
                            }

                            await Task.Delay(200);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(RefreshOperatorsForReceiversAsync), ex.Message);
            }
        }
    }
}
