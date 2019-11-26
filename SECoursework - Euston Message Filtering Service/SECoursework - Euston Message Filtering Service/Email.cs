using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace SECoursework___Euston_Message_Filtering_Service
{
    [DataContract]
    public class Email : Message
    {
        public Email(string messageid, string sender, string subject, string messagebody) : base(messageid, sender, subject, messagebody)
        {
            MessageID = messageid;
            Sender = sender;
            Subject = subject;
            MessageBody = messagebody;
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

        public static void CheckIfURL(string body, ref List<string> qList)
        {
            MainWindow mainWindow = new MainWindow();

            foreach (string url in body.Split(' '))
            {
                if (Regex.IsMatch(url, @"(http(s) ?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?"))
                {
                    qList.Add(url);
                }
            }
            foreach (string link in qList)
            {
                string replaceUrl = body.Replace(link, "< URL Quarantined >");
                body = replaceUrl;
            }
        }
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", MessageID, Sender, Subject, MessageBody);
        }
    }
}
