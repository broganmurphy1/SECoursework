using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;


namespace SECoursework___Euston_Message_Filtering_Service
{
    [DataContract]
    public class Message
    {
        [DataMember(Name ="Message ID")]
        public string MessageID { get; set; }
        [DataMember(Name ="Sender")]
        public string Sender { get; set; }
        [DataMember(Name = "Subject")]
        public string Subject { get; set; }
        [DataMember(Name = "Message Body")]
        public virtual string MessageBody { get; set; }

        public Message(string messageid, string sender, string subject ,string messagebody)
        {
            MessageID = messageid;
            Sender = sender;
            Subject = subject;
            MessageBody = messagebody;
        }

        public static bool CheckID(string id)
        {
            if ((id.Length == 10) && 
                (Regex.IsMatch(id[0].ToString(), @"^[a-zA-Z]+$") 
                && (Regex.IsMatch(id.Substring(1, 9), "^[0-9]*$"))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIfBodyEmpty(string bodytext)
        {
            if (bodytext != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", MessageID, Sender, Subject, MessageBody);
        }
    }
}
