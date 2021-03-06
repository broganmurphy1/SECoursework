﻿using System;
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
       public string BodyFinished { get; set; } //new property = where word is replaced by expanded textspeak abbreviation   
       public string [] BodyOriginal { get; set; } //returned string where words are not replaced
       private string messageBody;

       public override string MessageBody // getting message body and setting its value as user's input
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

        public SMS (string messageid, string sender, string subject, string messagebody, List<string> abvList, List<string> expList ) : base(messageid, sender, subject, messagebody)
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

        public static bool CheckBodyLength(string messagebody)//checks if body is equal to or less than 140 characters
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

        public static string ReplaceWord(string [] arrayofbody, List<string> ablist, List<string> exlist)//method that replaces each word; if word is not an abbreviation: return, if abbreviation pass to ConvertTxtSpeech
        {
            string answer = "";

            foreach(string word in arrayofbody)
            {
                int indexOfWord = Array.IndexOf(arrayofbody, word); //creating index of each word that will be changed
                arrayofbody[indexOfWord] = ConvertTxtSpeech(word, ablist, exlist);
            }
            answer = string.Join(" ", arrayofbody);
            return answer;
        }

        public static string ConvertTxtSpeech(string sentWord, List<string> abvlist, List<string> explist)//converts txt speech to expanded
        {
                if (abvlist.Contains(sentWord))//if sent word is an abbreviation
                {
                    int abvIndex = abvlist.IndexOf(sentWord);
                    string changedWord = sentWord + "<" + explist[abvIndex] + ">"; // changing word to expanded equivalent
                    return changedWord;
                }
                else
                {
                    return sentWord;//if word is not abv, return it
                }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", MessageID, Sender, Subject, MessageBody);
        }
    }
}
