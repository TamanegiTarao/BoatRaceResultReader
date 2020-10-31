using System;
using System.IO;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

namespace BoatRaceScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DateTime dT = new DateTime(2019, 10, 1);
            DateTime eT = new DateTime(2020, 10, 20);
            DateTime addt = dT.AddDays(1);
            Console.WriteLine(dT.ToString("yyyyMMdd"));
            Console.WriteLine(addt.ToString("yyyyMMdd"));


            do
            {
                for (int place = 1; place <= 24; place++)
                {
                    for (int race = 1; race <= 12; race++)
                    {
                        var addr = "https://www.boatrace.jp/owpc/pc/race/beforeinfo" +
                                   "?rno=" + race.ToString() +
                                   "&jcd=" + place.ToString("d2") +
                                   "&hd=" + dT.ToString("yyyyMMdd");
                        Console.WriteLine(addr);
                        var html = Scraping.GetHtmlString(addr);
                        var starts = Scraping.SearchTenjiSt(html);
                        if (starts == null)
                        {
                            break;
                        }
                        var printStr = dT.ToString("yyyy,MM,dd,") + place.ToString() + "," + race.ToString() + ",";
                        foreach (var str in starts)
                        {
                            printStr += str + ",";
                        }
                        Console.WriteLine(printStr);
                        using (var sw = new StreamWriter("Result.txt", true))
                        {
                            sw.WriteLine(printStr);
                        }
                        Thread.Sleep(2500);
                    }
                }
                dT = dT.AddDays(1);
                if (dT.Ticks > eT.Ticks)
                    break;
            } while (true);
        }
    }

    public class Scraping
    {
        static public string GetHtmlString(string url)
        {
            string rtn = "";
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var webResponse = (HttpWebResponse)webReq.GetResponse())
            using (var responseStream = webResponse.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
            {
                rtn = streamReader.ReadToEnd();
                Console.WriteLine(rtn);
            }
            return rtn;
        }

        static public string[] SearchTenjiSt(string html)
        {
            var lines = html.Split('\n');
            string[] rtn = new string[12] { "", "", "", "", "", "", "", "", "", "", "", "" };

            int num = 0;
            foreach (var line in lines)
            {
                if (line.Contains("table1_boatImage1Number"))
                {
                    var temp = line.Replace("<", ">");
                    var temp2 = temp.Split(">");
                    rtn[num++] = temp2[2];
                }
                if (line.Contains("table1_boatImage1Time"))
                {
                    var temp = line.Replace("<", ">");
                    var temp2 = temp.Split(">");
                    rtn[num++] = temp2[2];
                }
            }
            if (num == 0)
            {
                rtn = null;
            }
            return rtn;
        }

    }
}
