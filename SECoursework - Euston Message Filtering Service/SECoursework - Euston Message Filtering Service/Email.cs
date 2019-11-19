using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SECoursework___Euston_Message_Filtering_Service
{
    public class Email : Message
    {
        public string Sender { get; set; }
        public string Subject { get; set; }
        
        public Email (string messageid, string messagebody, string sender, string subject) : base (messageid, messagebody)
        {
            Sender = sender;
            Subject = subject;
        }

        public static bool CheckEmailFormat(string eAddress)
        {
            if (Regex.IsMatch(eAddress, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                return true;
            }
            return false;
        }

        public static bool CheckSubjectLength(string subjectlength)
        {
            if (subjectlength.Length <= 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckEmailBodyLength(string bodylength)
        {
            if (bodylength.Length <= 1028)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIfURL (string body)
        {
            if (body.Contains(Regex.IsMatch(body, )))
        }
    }
}
