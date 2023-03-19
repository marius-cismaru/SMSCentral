using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSCentral.Models
{
    public class Modem
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public int OperatorId { get; set; }

        public int Priority { get; set; } = 0;

        [JsonIgnore]
        public string COMPort { get; set; } = string.Empty;

        [JsonIgnore]
        public bool? IsConnected { get; set; } = false;

        [JsonIgnore]
        public string Status { get { return IsConnected.HasValue ? (IsConnected.Value == true ? "OK" : "Failed") : "N/A"; } }

        [JsonIgnore]
        public Operator Operator { get; set; } = new Operator();

        [JsonIgnore]
        public string OperatorDescription { get { return Operator?.Name ?? string.Empty; } }

        [JsonIgnore]
        public GsmCommMain GsmComm { get; set; }

        public bool SendSMS(Receiver receiver, string message)
        {
            var result = false;

            if (GsmComm != null)
            {
                try
                {
                    if(!GsmComm.IsOpen()) 
                        GsmComm.Open();

                    SmsSubmitPdu pdu;
                    byte dcs = (byte)DataCodingScheme.GeneralCoding.Alpha7BitDefault;
                    pdu = new SmsSubmitPdu(message, Convert.ToString(receiver.PhoneNumber), dcs);

                    GsmComm.SendMessage(pdu);

                    if (GsmComm.IsOpen())
                        GsmComm.Close();

                    result = true;
                }
                catch (Exception ex)
                {
                    Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(SendSMS) + "", ex.Message);
                }
            }

            return result;
        }
    }
}
