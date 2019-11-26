using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SECoursework___Euston_Message_Filtering_Service
{
    [DataContract]
    public class SIREmail : Email
    {
        [DataMember(Name = "Sport Centre Code")]
        public string SportCentreCode { get; set; }
        [DataMember(Name = "Nature of Incident")]
        public string NatureOfIncident { get; set; }
        
        public SIREmail (string messageid, string sender, string subject, string messagebody, string sportcentrecode, string natureofincident) : base (messageid, sender, subject, messagebody)
        {
            MessageID = messageid;
            Sender = sender;
            Subject = subject;
            MessageBody = messagebody;
            SportCentreCode = sportcentrecode;
            NatureOfIncident = natureofincident;
        }
    }
}
