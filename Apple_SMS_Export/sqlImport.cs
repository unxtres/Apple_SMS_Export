using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Apple_SMS_Export
{
    public class sqlImport
    {
        public SQLiteConnection myConnection;
        public SQLiteConnection smsConnection;
        public string sms_db_filename;
        public string directory;
        public string manifestPath = "Data Source = C:/3uTools/Backup/20220413_114055_Tomek/00008101-001C68591493001E/Manifest.db";
        public void ManifestDB()
        {
            myConnection = new SQLiteConnection(manifestPath);
        }

        public void OpenConnection()
        {
            if (myConnection != null || myConnection.State == System.Data.ConnectionState.Closed)
            {
                myConnection.Open();
                Console.WriteLine("Connected to database");
            }
        }

        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
                Console.WriteLine("Disconnected from database");
            }
        }

        public string FindSmsDatabaseFile()
            {
                //First find filename in Manifest.db
                string query = "SELECT fileID FROM Files WHERE relativePath='Library/SMS/sms.db'";
                SQLiteCommand searchSmsdb = new SQLiteCommand(query, myConnection);
                OpenConnection();
                SQLiteDataReader result = searchSmsdb.ExecuteReader();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        sms_db_filename = (string)result["fileID"];
                    }
                    //return sms_db_filename;
                    //var sms_db_filename = (string)result["fileID"];
                    //return sms_db_filename;

                    //Find file relative path
                    string[] dirs = Directory.GetFiles(@"C:\3uTools\", sms_db_filename, SearchOption.AllDirectories);
                    foreach (string dir in dirs)
                    {
                        //Console.WriteLine(dir);
                        directory = dir;
                    }
                }
                CloseConnection();
                
                directory = directory.Replace("\\", "/");
                directory = "Data Source = "+directory;

                return directory;
            }

        public void SmsDBConnect(string filePath)
        {
            smsConnection = new SQLiteConnection(filePath);
           
            if (smsConnection != null || smsConnection.State == System.Data.ConnectionState.Closed)
            {
                smsConnection.Open();
                Console.WriteLine("Connected to database");
            }
            // Export path and file.
            string exportPath = @"C:\demo\";
            string exportText = "smsexport.txt";

            // Stream writer for text file.
            StreamWriter textFile = null;

            var sqlSelect = smsConnection.CreateCommand();
            sqlSelect.CommandText = @"SELECT message.text AS Message, handle.id AS 'From', datetime(substr(date, 1, 9) + 978307200, 'unixepoch', 'localtime') as Time, message.is_from_me, message.is_delivered FROM handle, message WHERE message.handle_id = handle.ROWID AND message.text is not null";
            
            var reader = sqlSelect.ExecuteReader();

            textFile = new StreamWriter(@exportPath + exportText);

            // Add the headers to the text file.
            textFile.WriteLine(String.Format("{0}|{1}|{2}|{3}|{4}",
                reader.GetName(0), reader.GetName(1), reader.GetName(2),
                reader.GetName(3), reader.GetName(4)));

            // Construct text file data rows.
            while (reader.Read())
            {

                // Add line from reader object to new text file.
                textFile.WriteLine(String.Format("{0}|{1}|{2}|{3}|{4}",
                    reader[0], reader[1], reader[2], reader[3], reader[4]));

            }

            // Close the file.
            textFile.Close();
        }


        //public void exportMessages()
        //{
        //    string messageText;
        //    string getMessagesQuery = "SELECT message.text AS Message, handle.id AS 'From', datetime(substr(date, 1, 9) + 978307200, 'unixepoch', 'localtime') as Time, message.is_from_me, message.is_delivered FROM handle, message WHERE message.handle_id = handle.ROWID AND message.text is not null";
        //    SQLiteCommand getMessages = new SQLiteCommand(getMessagesQuery, smsConnection);
        //    Console.WriteLine("connected again...");
        //    SQLiteDataReader SMSresult = getMessages.ExecuteReader();

        //    if (SMSresult.HasRows)
        //    {
        //        while (SMSresult.Read())
        //        {
        //            messageText = (string)SMSresult["text"];
        //            Console.WriteLine(messageText);
        //        }
        //    }
        //}
        //public void exportMessages()
        //{
        //    string messageText;
        //    string getMessagesQuery = "SELECT message.text AS Message, handle.id AS 'From', datetime(substr(date, 1, 9) + 978307200, 'unixepoch', 'localtime') as Time, message.is_from_me, message.is_delivered FROM handle, message WHERE message.handle_id = handle.ROWID AND message.text is not null";
        //    SQLiteCommand getMessages = new SQLiteCommand(getMessagesQuery, smsConnection);
        //    Console.WriteLine("connected again...");
        //    getMessages.CommandText = getMessagesQuery;
        //    string wyniki = (string)getMessages.ExecuteScalar();

        //    Console.WriteLine(wyniki);


            
            
        //}
    }
    }