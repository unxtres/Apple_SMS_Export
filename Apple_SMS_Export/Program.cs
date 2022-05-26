using Apple_SMS_Export;
using System.Data.SQLite;


sqlImport databaseObject = new sqlImport();

databaseObject.Welcome();
databaseObject.ManifestDB();
string filepath = databaseObject.FindSmsDatabaseFile();
Console.WriteLine(filepath);
databaseObject.SmsDBConnect(filepath);
//databaseObject.exportMessages();

//fileop fileop = new fileop();
//fileop.showMe();
//fileop.compareLines();