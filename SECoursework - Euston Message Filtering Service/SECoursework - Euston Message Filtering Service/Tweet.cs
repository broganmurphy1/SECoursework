using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SECoursework___Euston_Message_Filtering_Service
{
    [DataContract]
    class Tweet: Message
    {
        public Tweet(string messageid, string sender, string subject, string messagebody) : base(messageid, sender, subject, messagebody)
        {
            MessageID = messageid;
            Sender = sender;
            Subject = subject;
            MessageBody = messagebody;
        }

        public static bool CheckTwitterId(string sender)
        {
            if (sender.Length == 16 && (Regex.IsMatch(sender[0].ToString(), @"^[@]+$")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckBodyLength(string messagebody)
        {
            if (messagebody.Length <= 140)
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
