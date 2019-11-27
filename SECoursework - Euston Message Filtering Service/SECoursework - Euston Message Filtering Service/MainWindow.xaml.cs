using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace SECoursework___Euston_Message_Filtering_Service
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> sirList = new List<string>(); //SIR list
        public List<string> quarantineList = new List<string>(); //Quarantine List

        string filePath = @"E:\Uni\JSONSave.txt"; //file path for text file for JSON output
        static string abvFilePath = @"E:\Uni\Abv.txt"; //file path for column A of textspeak abbreviations; this stores abbreviations
        static string expandFilePath = @"E:\Uni\Expand.txt"; //file path for column  of textspeak abbreviations; this stores abbreviations expanded

        public List<string> abvList = File.ReadAllLines(abvFilePath).ToList(); //reading lines of abv text file to a list
        public List<string> expandList = File.ReadAllLines(expandFilePath).ToList(); //reading lines of expand text file to a list

        public List<string> hashtagList = new List<string>(); // list of hashtags
        public List<string> mentionsList = new List<string>(); //list of mentions
        
        public MainWindow()
        {
            InitializeComponent();
            PopulateSirList(ref sirList); //populating SIR list using constructor
        }

        private void btn_ProcessEmail_Click(object sender, RoutedEventArgs e)
        {
            btn_View.IsEnabled = true; //enables view messages button so that messages can be viewed 
            try
            {
                if (Email.CheckID(txt_MessageID.Text) && (txt_MessageID.Text != "") && (Regex.IsMatch(txt_MessageID.Text[0].ToString(), @"^[Ee]+$"))) //calling static CheckID method from email class (Inherited from Message class) which checks if ID is valid, if also validates the id textbox to not accept empty and regex.IsMatch checks that [0] of ID is the letter E
                {
                    if (Email.CheckEmailFormat(txt_Sender.Text) && (txt_Sender.Text != "")) //Checking if email address is valid and not empty
                    {
                        if (Email.CheckSubjectLength(txt_Subject.Text) && (txt_Subject.Text != "")) //checking subject length and if its not empty
                        {
                            if (Email.CheckEmailBodyLength(txt_MessageBody.Text) && (txt_MessageBody.Text != "")) //checking body length and if its not empty
                            {
                                lst_Quarantine.Items.Clear(); //clear's URL's from listbox from previous messages
                                quarantineList.Clear(); //clear's previous URL's from previous messages from list

                                checkIfSIR(txt_Subject.Text, txt_MessageBody.Text, ref sirList); //check if email is SIR
                                Email.CheckIfURL(txt_MessageBody.Text, ref quarantineList); //check if URL is within message body

                                List<Email> email = new List<Email>(); // create list of emails
                                email.Add(new Email(txt_MessageID.Text, txt_Sender.Text, txt_Subject.Text, txt_MessageBody.Text)); // new instance of email class which stores textbox inputs into properties in the constructor of email

                                string json = JsonConvert.SerializeObject(email.ToArray()); //JSON format with object value email

                                File.AppendAllText(filePath, json + Environment.NewLine); //adding JSON string to textfile

                                MessageBox.Show("Email added");
                                clearTextBoxes();
                            }
                            else
                            {
                                MessageBox.Show("Body must be under 1028 characters and not empty");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Subject must be under twenty characters and not empty");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email not in correct format");
                    }
                }
                else
                {
                    MessageBox.Show("ID not in correct format, must be 'E' following with nine numbers");
                }
            }
            catch
            {
                MessageBox.Show("Please enter an input for all fields");
            }

            foreach (string link in quarantineList) //displays quarantined URL's in list to quarantine list
            {
                lst_Quarantine.Items.Add(link);
            }
            //foreach (string url in quarantineList)
            //{
            //    string urlreplaced = url.Replace(url, "<URL Quarantined>");
            //}
            //foreach (string url in txt_MessageBody.Text.Split(' '))
            //{
            //    if (Regex.IsMatch(url, @"(http(s) ?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?"))
            //    {
            //        lst_Quarantine.Items.Add(url);
            //        int index = Array.IndexOf(txt_MessageBody.Text.Split(' '), url);
            //        txt_MessageBody.Text.Split(' ')[index] = "<URL Quarantined>";
            //    }
            //}
        }

        private void btn_View_Click(object sender, RoutedEventArgs e) //views all messages in textfile
        {
            List<Message> MessageList = new List<Message>(); //create message list
            List<string> JSONMessageList = File.ReadAllLines(filePath).ToList(); //create list f textfile lines
            foreach (string messages in JSONMessageList) //for each message in text file list
            {
                lst_DisplayMessages.Items.Clear();//clear any previous messages so there is no duplication
                try
                {
                    string[] splitMessages = messages.Split(','); //adds entry to array after every instance of a comma is found in textfile

                    Message message = new Message(splitMessages[0], splitMessages[1], splitMessages[2], splitMessages[3]); //new instance of message class with entries of a line in the textfile

                    MessageList.Add(message); //add it to message list
                    foreach (Message newMessage in MessageList) 
                    {
                        lst_DisplayMessages.Items.Add(newMessage); //display each message in textfile to message listbox
                    }
                }
                catch
                {
                    MessageBox.Show("File empty");
                }
            }
        }

        private void btn_DeleteMessage_Click(object sender, RoutedEventArgs e)// remove message from listbox and selected index from textfile
        {
            btn_View.IsEnabled = false; //disable view button; delete mode
                if (lst_DisplayMessages.SelectedIndex > -1)
                {
                    lst_DisplayMessages.Items.RemoveAt(lst_DisplayMessages.SelectedIndex);
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        using (TextWriter tw = new StreamWriter(fs))
                            foreach (var item in lst_DisplayMessages.Items)
                                tw.WriteLine(item);
                    }
                }

                if (lst_DisplayMessages.SelectedIndex >= 0)
                    lst_DisplayMessages.Items.RemoveAt(lst_DisplayMessages.SelectedIndex);
        }

        private void clearTextBoxes() //clears textboxes
        {
            txt_MessageID.Clear();
            txt_Sender.Clear();
            txt_Subject.Clear();
            txt_MessageBody.Clear();
        }

        public bool checkIfSIR(string subject, string messagebody, ref List<string> sList)
        {
            Regex dateRegex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$"); //regex for valid date
            Regex sportCentreCodeRegex = new Regex(@"[0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9]"); //regex for valid sport centre code

            if (subject.StartsWith("SIR") && dateRegex.IsMatch(subject)) //if subject starts "SIR" and contains valid date
            {
                if (messagebody.StartsWith("sport centre code") && sportCentreCodeRegex.IsMatch(messagebody) && messagebody.Contains("nature of incident"))  //if message body starts with "sports centre code", contains a valid sport centre code and "nature of incident"
                {
                    foreach (string incident in sList) //for each valid incident
                    {
                        if(messagebody.Contains(incident))//if this incident is contained in messagebody, add incident to SIR list and return true
                        {
                            lst_SIR.Items.Add(messagebody);
                            return true;
                        }
                    } 
                }
            }
            return false; //else return false
        }
        
        public void PopulateSirList(ref List<string> sList) //populates SIR list with all types of incidents
        {
            sList.Add("theftofproperties");
            sList.Add("staff attack");
            sList.Add("device damage");
            sList.Add("raid");
            sList.Add("customer attack");
            sList.Add("staff abuse");
            sList.Add("bomb threat");
            sList.Add("terrorism");
            sList.Add("suspicious incident");
            sList.Add("sport injury");
            sList.Add("personal info leak");
        }

        private void btn_ProcessSMS_Click(object sender, RoutedEventArgs e)
        {
            btn_View.IsEnabled = true;
            try
            {
                if (SMS.CheckID(txt_MessageID.Text) && (txt_MessageID.Text != "") && (Regex.IsMatch(txt_MessageID.Text[0].ToString(), @"^[Ss]+$")))// same validation of ID for email except first char must be "S"
                {
                    if (txt_Sender.Text.Length == 11 && (txt_Sender.Text != "") && (CheckSMSNumForDigits(txt_Sender.Text))) //checks if phone num length is = 11, not empty and all numerical
                    {
                        if (SMS.CheckBodyLength(txt_MessageBody.Text) && (txt_MessageBody.Text != "")) //checks if body length less than or equal to 140 characters
                        {
                            if (txt_Subject.Text == "") //checks if subject text box is empty
                            {
                                lst_Quarantine.Items.Clear(); //clears previous quarantined URL's from listbox
                                quarantineList.Clear(); //clears previous messages from quarantine list

                                List<SMS> sms = new List<SMS>(); //new SMS list
                                sms.Add(new SMS(txt_MessageID.Text, txt_Sender.Text, "N/A", txt_MessageBody.Text, abvList, expandList)); //instance of SMS object, subject is "N/A" as sms cannot have subject, also properties have been created for abv list and expand list

                                string json = JsonConvert.SerializeObject(sms.ToArray()); //convert SMS to JSON

                                File.AppendAllText(filePath, json + Environment.NewLine); //add to textfile

                                MessageBox.Show("SMS added");
                                clearTextBoxes();
                            }
                            else
                            {
                                MessageBox.Show("Cannot have subject for this type of message");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Body must be under or equal 140 characters and not empty");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Phone number not in correct format");
                    }
                }
                else
                {
                    MessageBox.Show("ID not in correct format, must be 'S' following with nine numbers");
                }
            }
            catch
            {
                MessageBox.Show("SMS not in correct format");
            }
        }

        private void btn_ProcessTweet_Click(object sender, RoutedEventArgs e)
        {
            btn_View.IsEnabled = true;
            try
            {
                if (Tweet.CheckID(txt_MessageID.Text) && (txt_MessageID.Text != "") && (Regex.IsMatch(txt_MessageID.Text[0].ToString(), @"^[Tt]+$"))) //validates ID same as above excpet first char must be "T"
                {
                    if (Tweet.CheckTwitterId(txt_Sender.Text) && (txt_Sender.Text != "")) //validates twitter ID so it start with "@" and follow with 15 characters and not empty
                    {
                        if (Tweet.CheckBodyLength(txt_MessageBody.Text) && (txt_MessageBody.Text != "")) //Checks if body length is less than 140 characters and not empty
                        {
                            if (txt_Subject.Text == "")//checks if subject is empty
                            {
                                lst_Quarantine.Items.Clear();//clears previous quarantined URL's from listbox
                                quarantineList.Clear();//Clears these URL's from list

                                Tweet.CheckIfHashtag(txt_MessageBody.Text, ref hashtagList);//checks if hashtag is present
                                Tweet.CheckIfMention(txt_MessageBody.Text, ref mentionsList);//checks if mention is present
                                
                                List<Tweet> tweet = new List<Tweet>();//new tweets list
                                tweet.Add(new Tweet(txt_MessageID.Text, txt_Sender.Text, "N/A", txt_MessageBody.Text, abvList, expandList)); //same constructor as SMS

                                string json = JsonConvert.SerializeObject(tweet.ToArray());

                                File.AppendAllText(filePath, json + Environment.NewLine);

                                MessageBox.Show("Tweet added");
                                clearTextBoxes();
                            }
                            else
                            {
                                MessageBox.Show("Cannot have subject for this type of message");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Body must be under or equal 140 characters and not empty");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Twitter ID not in correct format");
                    }
                }
                else
                {
                    MessageBox.Show("ID not in correct format, must be 'T' following with nine numbers");
                }
            }
            catch
            {
                MessageBox.Show("Tweet not in correct format");
            }
            
            foreach (string hashtag in hashtagList) //adding hashtags to listbox
            {
                lst_Hashtags.Items.Add(hashtag);
            }
            foreach (string mention in mentionsList) //adding mentions to listbox
            {
                lst_Mentions.Items.Add(mention);
            }
        }
        public bool CheckSMSNumForDigits(string sender)//checks if phone number is all numerical
        {
            long parsedValue;
            if (Int64.TryParse(txt_Sender.Text, out parsedValue))
            {
                return true;
            }
            else
                return false;
        }
    }
}
