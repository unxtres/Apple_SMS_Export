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
        public string manifestPath;
        public string backupPath;
        public string exportPath;
        public string fname;
        public string path;
        
        public void Welcome()
        {
            Console.WriteLine("Provide path to manifest.db file.");
            Console.WriteLine("Example: C:/3utools/Backup/20220101_131013_iPhone/00008020-000E74482EDA002E/");
            Console.Write("Path = C:/");
            backupPath = "C:/"+Console.ReadLine().Replace("\\", "/")+"/";
            manifestPath = "Data Source = " + backupPath+"Manifest.db";
            Console.WriteLine();
            Console.WriteLine("Where do you want to export sms messages?");
            Console.WriteLine("Example: C:/export/");
            Console.Write("Export Path = C:/");
            exportPath = @"C:/" + Console.ReadLine().Replace("\\", "/")+"/";
            Directory.CreateDirectory(exportPath);
        }
        public void ManifestDB()
        {
            myConnection = new SQLiteConnection(manifestPath);
        }

        public void OpenConnection()
        {
            if (myConnection != null || myConnection.State == System.Data.ConnectionState.Closed)
            {
                myConnection.Open();
                Console.WriteLine("Connected to backup database");
            }
        }

        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
                Console.WriteLine("Disconnected from backup database");
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

                    //Find file relative path
                    string[] dirs = Directory.GetFiles(backupPath, sms_db_filename, SearchOption.AllDirectories);
                    foreach (string dir in dirs)
                    {
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
                Console.WriteLine("Connected to sms database");
            }

            // Stream writer for text file.
            StreamWriter textFile = null;

            var sqlSelect = smsConnection.CreateCommand();
            sqlSelect.CommandText = @"SELECT message.text AS Message, handle.id AS 'From', datetime(substr(date, 1, 9) + 978307200, 'unixepoch', 'localtime') as Time, message.is_from_me, message.is_delivered FROM handle, message WHERE message.handle_id = handle.ROWID AND message.text is not null ORDER BY handle.id, time";
            
            var reader = sqlSelect.ExecuteReader();
            reader.Read();
            var lastLine = reader[1];
            var currentLine = reader[1];
            

            while (reader.Read())
            {
                currentLine = reader[1];
                if (reader[1].ToString() != lastLine.ToString())
                {
                    fname = reader[1].ToString();
                    fname = fname.Replace("?", "");
                    textFile = new StreamWriter(@exportPath + fname+".txt");

                    if (reader[3].ToString() == "0" && reader[4].ToString() == "1")
                    {
                        textFile.WriteLine(String.Format("MESSAGE FROM: {0}", reader[1]));
                        textFile.WriteLine(String.Format("DATE: {0}", reader[2]));
                        textFile.WriteLine(String.Format("IN MESSAGE: \n {0}", reader[0]));
                    }

                    else
                    {
                        textFile.WriteLine(String.Format("MESSAGE SENT TO: {0}", reader[1]));
                        textFile.WriteLine(String.Format("DATE SENT: {0}", reader[2]));
                        textFile.WriteLine(String.Format("OUT MESSAGE: \n {0}", reader[0]));
                    }
                    lastLine = reader[1];
                    textFile.Close();
                }
                else
                {
                    fname = reader[1].ToString();
                    fname = fname.Replace("?", "");
                    fname = fname.Replace("*", "");
                    path = @"C:\demo\" + fname+".txt";

                    TextWriter tw = new StreamWriter(path, true);
                    if (reader[3].ToString() == "0" && reader[4].ToString() == "1")
                    {
                        tw.WriteLine(String.Format("\nDATE : {0}", reader[2]));
                        tw.WriteLine(String.Format("IN MESSAGE: \n {0}", reader[0]));
                    }

                    else
                    {
                        tw.WriteLine(String.Format("\nDATE SENT: {0}", reader[2]));
                        tw.WriteLine(String.Format("OUT MESSAGE: \n {0}", reader[0]));
                    }

                    tw.Close();
                    lastLine = reader[1];
                }
            }
        }
    }
    }