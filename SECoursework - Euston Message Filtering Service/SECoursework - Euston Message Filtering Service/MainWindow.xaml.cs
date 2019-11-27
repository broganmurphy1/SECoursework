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
        public List<string> sirList = new List<string>();
        public List<string> quarantineList = new List<string>();

        string filePath = @"E:\Uni\JSONSave.txt";
        static string abvFilePath = @"E:\Uni\Abv.txt";
        static string expandFilePath = @"E:\Uni\Expand.txt";

        public List<string> abvList = File.ReadAllLines(abvFilePath).ToList();
        public List<string> expandList = File.ReadAllLines(expandFilePath).ToList();

        public List<string> hashtagList = new List<string>();
        public List<string> mentionsList = new List<string>();
        
        public MainWindow()
        {
            InitializeComponent();
            PopulateSirList(ref sirList);
        }

        private void btn_ProcessEmail_Click(object sender, RoutedEventArgs e)
        {
            btn_View.IsEnabled = true;
            try
            {
                if (Email.CheckID(txt_MessageID.Text) && (txt_MessageID.Text != "") && (Regex.IsMatch(txt_MessageID.Text[0].ToString(), @"^[Ee]+$")))
                {
                    if (Email.CheckEmailFormat(txt_Sender.Text) && (txt_Sender.Text != ""))
                    {
                        if (Email.CheckSubjectLength(txt_Subject.Text) && (txt_Subject.Text != ""))
                        {
                            if (Email.CheckEmailBodyLength(txt_MessageBody.Text) && (txt_MessageBody.Text != ""))
                            {
                                lst_Quarantine.Items.Clear();
                                quarantineList.Clear();

                                checkIfSIR(txt_Subject.Text, txt_MessageBody.Text, ref sirList);
                                Email.CheckIfURL(txt_MessageBody.Text, ref quarantineList);

                                List<Email> email = new List<Email>();
                                email.Add(new Email(txt_MessageID.Text, txt_Sender.Text, txt_Subject.Text, txt_MessageBody.Text));

                                string json = JsonConvert.SerializeObject(email.ToArray());

                                File.AppendAllText(filePath, json + Environment.NewLine);

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

            foreach (string link in quarantineList)
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

        private void btn_View_Click(object sender, RoutedEventArgs e)
        {
            List<Message> MessageList = new List<Message>();
            List<string> JSONMessageList = File.ReadAllLines(filePath).ToList();
            foreach (string messages in JSONMessageList)
            {
                lst_DisplayMessages.Items.Clear();
                try
                {
                    string[] splitMessages = messages.Split(',');

                    Message message = new Message(splitMessages[0], splitMessages[1], splitMessages[2], splitMessages[3]);

                    MessageList.Add(message);
                    foreach (Message newMessage in MessageList)
                    {
                        lst_DisplayMessages.Items.Add(newMessage);
                    }
                }
                catch
                {
                    MessageBox.Show("File empty");
                }
            }
        }

        private void btn_DeleteMessage_Click(object sender, RoutedEventArgs e)
        {
            btn_View.IsEnabled = false;
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

        private void clearTextBoxes()
        {
            txt_MessageID.Clear();
            txt_Sender.Clear();
            txt_Subject.Clear();
            txt_MessageBody.Clear();
        }

        public bool checkIfSIR(string subject, string messagebody, ref List<string> sList)
        {
            Regex dateRegex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$");
            Regex sportCentreCodeRegex = new Regex(@"[0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9]");

            if (subject.StartsWith("SIR") && dateRegex.IsMatch(subject))
            {
                if (messagebody.StartsWith("sport centre code") && sportCentreCodeRegex.IsMatch(messagebody) && messagebody.Contains("nature of incident"))  
                {
                    foreach (string incident in sList)
                    {
                        if(messagebody.Contains(incident))
                        {
                            lst_SIR.Items.Add(messagebody);
                            return true;
                        }
                    } 
                }
            }
            return false;
        }
        
        public void PopulateSirList(ref List<string> sList)
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

        private void btn_DeleteSIR_Click(object sender, RoutedEventArgs e)
        {
            lst_SIR.Items.RemoveAt(lst_SIR.SelectedIndex);
        }

        private void btn_ProcessSMS_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (SMS.CheckID(txt_MessageID.Text) && (txt_MessageID.Text != "") && (Regex.IsMatch(txt_MessageID.Text[0].ToString(), @"^[Ss]+$")))
                {
                    if (txt_Sender.Text.Length == 11 && (txt_Sender.Text != "") && (CheckSMSNumForDigits(txt_Sender.Text)))
                    {
                        if (SMS.CheckBodyLength(txt_MessageBody.Text) && (txt_MessageBody.Text != ""))
                        {
                            if (txt_Subject.Text == "")
                            {
                                lst_Quarantine.Items.Clear();
                                quarantineList.Clear();

                                List<SMS> sms = new List<SMS>();
                                sms.Add(new SMS(txt_MessageID.Text, txt_Sender.Text, "N/A", txt_MessageBody.Text, abvList, expandList));

                                string json = JsonConvert.SerializeObject(sms.ToArray());

                                File.AppendAllText(filePath, json + Environment.NewLine);

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
            try
            {
                if (SMS.CheckID(txt_MessageID.Text) && (txt_MessageID.Text != "") && (Regex.IsMatch(txt_MessageID.Text[0].ToString(), @"^[Tt]+$")))
                {
                    if (Tweet.CheckTwitterId(txt_Sender.Text) && (txt_Sender.Text != ""))
                    {
                        if (Tweet.CheckBodyLength(txt_MessageBody.Text) && (txt_MessageBody.Text != ""))
                        {
                            if (txt_Subject.Text == "")
                            {
                                lst_Quarantine.Items.Clear();
                                quarantineList.Clear();

                                Tweet.CheckIfHashtag(txt_MessageBody.Text, ref hashtagList);
                                Tweet.CheckIfMention(txt_MessageBody.Text, ref mentionsList);
                                
                                List<Tweet> tweet = new List<Tweet>();
                                tweet.Add(new Tweet(txt_MessageID.Text, txt_Sender.Text, "N/A", txt_MessageBody.Text, abvList, expandList));

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
            
            foreach (string hashtag in hashtagList)
            {
                lst_Hashtags.Items.Add(hashtag);
            }
            foreach (string mention in mentionsList)
            {
                lst_Mentions.Items.Add(mention);
            }
        }
        public bool CheckSMSNumForDigits(string sender)
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
