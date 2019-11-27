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
    public class Tweet: Message
    {
        public string BodyFinished { get; set; }
        public string[] BodyOriginal { get; set; }
        private string messageBody;

        public override string MessageBody
        {
            get
            {
                return BodyFinished;
            }
            set
            {
                messageBody = value;
            }
        }

        public Tweet(string messageid, string sender, string subject, string messagebody, List<string> abvList, List<string> expList) : base(messageid, sender, subject, messagebody)
        {
            MessageID = messageid;
            Sender = sender;
            Subject = subject;
            MessageBody = messagebody;
            BodyOriginal = messagebody.Split(' ');
            List<string> aList = abvList;
            List<string> eList = expList;
            BodyFinished = ReplaceWord(BodyOriginal, aList, eList);
        }

        public static bool CheckTwitterId(string sender) //checks if ID is less than or equal to 16 characters and first character is "@"
        {
            if (sender.Length <= 16 && (Regex.IsMatch(sender[0].ToString(), @"^[@]+$")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckBodyLength(string messagebody) //checks message body so that it must be less than or equal to 140 characters
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

        public static void CheckIfHashtag(string body, ref List<string> hashList) //checks each word in message body, if word is hashtag add it to list
        {
            foreach (string word in body.Split(' '))
            {
                if (word.Contains("#"))
                {
                    hashList.Add(word);
                }
            }
        }

        public static void CheckIfMention(string body, ref List<string> mentionList)//checks each word in message body, if word is mention add it to list
        {
            foreach (string word in body.Split(' '))
            {
                if (word.Contains("@"))
                {
                    mentionList.Add(word);
                }
            }
        }

        public static string ReplaceWord(string[] arrayofbody, List<string> ablist, List<string> exlist)
        {
            string answer = "";

            foreach (string word in arrayofbody)
            {
                int indexOfWord = Array.IndexOf(arrayofbody, word); //creating index of each word that will be changed
                arrayofbody[indexOfWord] = ConvertTxtSpeech(word, ablist, exlist);
            }
            answer = string.Join(" ", arrayofbody);
            return answer;
        }

        public static string ConvertTxtSpeech(string sentWord, List<string> abvlist, List<string> explist)
        {
            if (abvlist.Contains(sentWord))
            {
                int abvIndex = abvlist.IndexOf(sentWord);
                string changedWord = sentWord + "<" + explist[abvIndex] + ">";
                return changedWord;
            }
            else
            {
                return sentWord;
            }
        }
        
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", MessageID, Sender, Subject, MessageBody);
        }
    }
}
