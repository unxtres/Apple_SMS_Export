using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Apple_SMS_Export
{
    public class fileop
    {
        public string[] lines = File.ReadAllLines(@"C:\demo\smsexport.txt");
        public string[] SplitLines;
        public string[] LastLine = null;
        public int i=0;
        public void showMe()
        {
            Console.WriteLine(lines[9]);
        }
        //public void compareLines()
        //{
        //    foreach (var line in lines)
        //    {
        //        SplitLines = line.Split('|');
        //        if (LastLine != null)
        //        {
        //            if (SplitLines[1] == LastLine[1])
        //            {
        //                Console.WriteLine("tak");
        //                LastLine = SplitLines;
        //            }
        //            else
        //            {
        //                Console.WriteLine("nie");
        //                LastLine = SplitLines;
        //            }
        //        }
        //        else LastLine = SplitLines;
        //    }
        //}
    }
}
