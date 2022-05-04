using Apple_SMS_Export;
using System.Data.SQLite;

sqlImport databaseObject = new sqlImport();

databaseObject.ManifestDB();
string filepath = databaseObject.FindSmsDatabaseFile();
Console.WriteLine(filepath);
databaseObject.SmsDBConnect(filepath);
//databaseObject.exportMessages();