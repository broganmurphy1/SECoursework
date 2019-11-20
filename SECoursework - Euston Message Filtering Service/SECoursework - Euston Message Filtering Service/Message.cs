using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SECoursework___Euston_Message_Filtering_Service
{
    public class Message
    {
        public string MessageID { get; set; }
        public string MessageBody { get; set; }

        public Message(string messageid, string messagebody)
        {
            MessageID = messageid;
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
            return string.Format("{0} - {1}", "You have entered", MessageID);
        }
    }
}
