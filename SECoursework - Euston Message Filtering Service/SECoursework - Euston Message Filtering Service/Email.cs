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
    public class Email : Message //inheriting from base message class
    {
        public Email(string messageid, string sender, string subject, string messagebody) : base(messageid, sender, subject, messagebody) //Constructor 
        {
            MessageID = messageid;//properties inherited from message base class
            Sender = sender;
            Subject = subject;
            MessageBody = messagebody;
        }

        public static bool CheckEmailFormat(string eAddress) //checks email format using an email regex and if a match is found, return true
        {
            if (Regex.IsMatch(eAddress, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                return true;
            }
            return false; //if condition is not met
        }

        public static bool CheckSubjectLength(string subjectlength) //Checks if subject length is less than or equal to 20 characters, returns true if condition is met
        {
            if (subjectlength.Length <= 20)
            {
                return true;
            }
            else
            {
                return false; // if condition is not met
            }
        }

        public static bool CheckEmailBodyLength(string bodylength) //checks if email body length is less than or equal to 1028 characters, returns true if condition is met
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

        public static void CheckIfURL(string body, ref List<string> qList) //checks if message body contains a url using a url regex, if there is a match the url is added to a quarantine list
        {
            foreach (string url in body.Split(' '))
            {
                if (Regex.IsMatch(url, @"(http(s) ?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?"))
                {
                    qList.Add(url); //qList is a parameter for quarantineList on mainwindow.xaml.cs
                }
            }
            foreach (string link in qList) //attempt at quarantining the URL
            {
                string replaceUrl = body.Replace(link, "< URL Quarantined >"); //creating a new string which is equal to the instance of url and replace with "<URL Quarantined>"
                body = replaceUrl;
            }
        }
        public override string ToString() //ToString() displays email object in this format
        {
            return string.Format("{0}, {1}, {2}, {3}", MessageID, Sender, Subject, MessageBody);
        }
    }
}
