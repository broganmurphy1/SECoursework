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
    public class SMS: Message
    {
       public string BodyFinished { get; set; }     
       public string [] BodyOriginal { get; set; }
       
        public abvArray
        public SMS (string messageid, string sender, string subject, string messagebody) : base(messageid, sender, subject, messagebody)
        {
            MessageID = messageid;
            Sender = sender;
            Subject = subject;
            MessageBody = messagebody;
            BodyOriginal = messagebody.Split(' ');
            BodyFinished = replaceWord(BodyOriginal);
        }

        public static bool CheckNumberFormat(string sender)
        {
            if (Regex.IsMatch(sender, (@"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|
                                        2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|
                                        4[987654310]|3[9643210]|2[70]|7|1)\d{1,14}$")))
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
            if(messagebody.Length <= 140)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ReplaceWord(string [] arrayofbody) 
        {
            string answer = "";

            foreach(string word in arrayofbody)
            {
                int indexOfWord = Array.IndexOf(arrayofbody, word); //creating index of each word that will be changed
                arrayofbody[indexOfWord] = ConvertTxtSpeech(word);
            }
            answer = string.Join(" ", arrayofbody);
            return answer;
        }

        public static string ConvertTxtSpeech(string sentWord)
        {
            if(abvArray.Contains(sentWord))
            {
                int abvIndex = abvArray.IndexOf(sentWord);
                string changedWord = sentWord + "<" + expArray[abvIndex] + ">";
                return changedWord;
            }
            else
            {
                return sentWord;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", MessageID, Sender, Subject, BodyFinished);
        }
    }
}
