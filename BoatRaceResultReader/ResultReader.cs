using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BoatRaceResultReader
{
    public static class Lib
    {
        static public int IntParse(string str)
        {
            int rtn = 0;
            if(!int.TryParse(str,out rtn))
            {
                rtn = 0;
            }
            return rtn;
        }

        static public double DoubleParse(string str)
        {
            double rtn = 0.0;
            if (!double.TryParse(str, out rtn))
            {
                rtn = 0;
            }
            return rtn;
        }
    }

    public enum Internode1
    {
        General = 0,
        G3,
        G2,
        G1,
        SG,
    }

    public enum Internode2
    {
        Masters = 0,
        Wemen,
    }

    public class InternodeTitle
    {
        public string Title;
        public Internode1 Internode1;
        public Internode2 Internode2;
        public DateTime Start;
        public DateTime End;
    }


    public class RaceTitle
    {
        public string Title;
        public bool FixedApproach;
        public bool SemiFinal;
        public bool Final;
        public bool Seed;
    }

    public class Race
    {
        public int Place;
        public DateTime Date;
        public int InDay;
        public int RaceNum;
        public bool Round2;

        public Race() { }

        // クローンの生成
        // https://cyzennt.co.jp/blog/2020/05/01/c%EF%BC%9A%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%81%AE%E4%B8%AD%E8%BA%AB%E3%82%92%E3%82%B3%E3%83%94%E3%83%BC%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95%EF%BC%88memberwiseclone%E3%83%A1/

        public Race Clone() { return (Race)MemberwiseClone(); }

        public string Str { get { return Place.ToString() + "," + Date.ToString("yyyy,MM,dd") + "," + InDay.ToString() + "," + RaceNum.ToString() + ","; } }
    }

    public class ResultOne
    {
        public int Popler;
        public int Yen;
        public int First;

        public virtual string Str { get { return Popler.ToString() + "," + Yen.ToString() + "," + First.ToString() + ","; } }
    }

    public class ResultTwo : ResultOne
    {
        public int Second;

        public new string Str { get { return base.Str + Second.ToString() + ","; } }
    }

    public class ResultThree : ResultTwo
    {
        public int Thired;

        public new string Str { get { return base.Str + Thired.ToString() + ","; } }
    }

    public class RaceResult
    {
        public Race Race;
        public ResultOne Tansho;
        public ResultOne Fukusho1;
        public ResultOne Fukusho2;
        public ResultTwo NirenTan;
        public ResultTwo NirenFuku;
        public ResultThree SanrenTan;
        public ResultThree SanrenFuku;
        public ResultTwo Kakuren1;
        public ResultTwo Kakuren2;
        public ResultTwo Kakuren3;

        public RaceResult()
        {
            Tansho = new ResultOne();
            Fukusho1 = new ResultOne();
            Fukusho2 = new ResultOne();
            NirenTan = new ResultTwo();
            NirenFuku = new ResultTwo();
            SanrenTan = new ResultThree();
            SanrenFuku = new ResultThree();
            Kakuren1 = new ResultTwo();
            Kakuren2 = new ResultTwo();
            Kakuren3 = new ResultTwo();
        }

        public string Str
        {
            get
            {
                return Tansho.Str + Fukusho1.Str + Fukusho2.Str + NirenTan.Str + NirenFuku.Str +
                       Kakuren1.Str + Kakuren2.Str + Kakuren3.Str + SanrenTan.Str + SanrenFuku.Str;
            }
        }

    }

    public enum WindDirection { None, N, NE, E, ES, S, SW, W, WN };
    public enum Kimarite { Nige, Makuri, Sashi, Makurisashi, Megumare, Nuki, Huseiritsu }
    public class RaceInfo
    {
        public InternodeTitle InternodeTitle;
        public RaceTitle RaceTitle;
        public Race Race;
        public RaceResult RaceResult;

        public Kimarite Kimarite;
        public WindDirection WindDirection;
        public int WindMetor;

        public void KimariteSet(string str) { Kimarite = KimariteNumber(str); }
        public void WindDirectionSet(string str) { WindDirection = WindDirectionNumber(str); }

        public static WindDirection WindDirectionNumber(string str)
        {
            switch (str)
            {
                case "無風　": return WindDirection.None;
                case "北　　": return WindDirection.N;
                case "北東　": return WindDirection.NE;
                case "東　　": return WindDirection.E;
                case "南東　": return WindDirection.ES;
                case "南　　": return WindDirection.S;
                case "南西　": return WindDirection.SW;
                case "西　　": return WindDirection.W;
                case "北西　": return WindDirection.WN;
                default: return WindDirection.None;
            }
        }

        public static Kimarite KimariteNumber(string str)
        {
            switch (str)
            {
                case "逃げ　　　": return Kimarite.Nige;
                case "差し　　　": return Kimarite.Sashi;
                case "まくり　　": return Kimarite.Makuri;
                case "まくり差し": return Kimarite.Makurisashi;
                case "抜き　　　": return Kimarite.Nuki;
                case "恵まれ　　": return Kimarite.Megumare;
                case "　　　　　": return Kimarite.Huseiritsu;
                default: return Kimarite.Nige;
            }
        }

        public string Str
        {
            get
            {
                return InternodeTitle.Title + "," + RaceTitle.Title + "," + Race.Str + RaceResult.Str +
                       ((int)Kimarite).ToString() + "," + (((int)WindDirection).ToString()) + "," + WindMetor.ToString() + ",";
            }
        }

    }

    public class PlayerResult
    {
        public RaceInfo RaceInfo;
        public int Number;
        public int Result;
        public int Wakuban;
        public int Cource;
        public int Motor;
        public int Boat;
        public double Tenji;
        public double Start;


        public static int ResultNumber(string str)
        {
            string[] resultList =
            {
                "F ","L ","K0","K1",
                "S0","S1","S2"
            };

            int rtn;
            if (int.TryParse(str, out rtn) == false)
            {
                for (int i = 0; i < resultList.Length; i++)
                {
                    if (str == resultList[i])
                    {
                        return i * (-1);
                    }
                }
                return resultList.Length * (-1);
            }
            return rtn;
        }

        public string Str
        {
            get
            {
                return RaceInfo.Str + Number.ToString() + "," + Result.ToString() + "," +
                       Wakuban.ToString() + "," + Cource.ToString() + "," + Motor.ToString() + "," +
                       Boat.ToString() + "," + Tenji.ToString() + "," + Start.ToString();
            }
        }
    }

    public class RaceResults
    {
        public List<InternodeTitle> InternodeTitles;
        public List<RaceTitle> RaceTitles;
        public List<RaceResult> Results;
        public List<PlayerResult> PlayerResults;

        public RaceResults()
        {
            InternodeTitles = new List<InternodeTitle>();
            RaceTitles = new List<RaceTitle>();
            Results = new List<RaceResult>();
            PlayerResults = new List<PlayerResult>();
        }

        Encoding _encSJIS;
        private string _StringExtraction(string src, int st, int count)
        {
            if (_encSJIS == null)
                _encSJIS = Encoding.GetEncoding("Shift_JIS");
            byte[] rtnBytes = new byte[count];
            var srcbytes = _encSJIS.GetBytes(src);
            if (srcbytes.Length < (st + count)) { return ""; }
            Array.Copy(srcbytes, st, rtnBytes, 0, count);
            return _encSJIS.GetString(rtnBytes);
        }

        private static bool IsStringInDate(string str){ return Regex.IsMatch(str, "..../../.."); }

        private static bool IsStringInInDay(string str){ return Regex.IsMatch(str, "第..日"); }

        private bool IsStringLine7(string str){ return IsStringInDate(str) && IsStringInInDay(str); }

        private bool IsStringInRoundInfo(string str) { return Regex.IsMatch(str, "H1.00m"); }

        private bool IsRound2(string str) { return str.Contains("H1200m"); }

        public void Read(string file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var sr = new StreamReader(file, Encoding.GetEncoding("shift_jis")))
            {
                InternodeTitle internodeTitle = new InternodeTitle();
                RaceTitle raceTitle;
                RaceInfo raceInfo = new RaceInfo();
                RaceResult raceResult = new RaceResult();
                Race raceBase = new Race();
                Race race;
                int inLineInfo = -9999;
                int inLinePlayer = -9999;
                int place = 0;
                bool entryfirst = false;
                bool nextKakurenSecond = false;
                bool nextKakurenThired = false;
                bool start = false;
                while (sr.EndOfStream == false)
                {
                    var line = sr.ReadLine();
                    
                    if (line.Contains("KBGN"))
                    {   // １.開催場コードの取得
                        place = Lib.IntParse(line.Replace("KBGN", ""));
                        inLineInfo = 0;
                        start = true;
                    }
                    else if(line.Contains("KEND"))
                    {
                        inLineInfo = 0;
                        start = false;
                    }
                    else if(start)
                    {
                        if (inLineInfo == 5) // 文字列を含むかどうかで拾うのがむつかしい
                        {   // ２.節間タイトルを取得
                            // https://csharp-ref.com/program_linq-lambda.html
                            string title = _StringExtraction(line, 10, 50);
                            if (InternodeTitles.Exists(x => x.Title == title))
                            {
                                internodeTitle = InternodeTitles.OrderBy(x => x.Start).FirstOrDefault();
                                entryfirst = false;
                            }
                            else
                            {
                                internodeTitle = new InternodeTitle();
                                internodeTitle.Title = title;
                                entryfirst = true;
                                InternodeTitles.Add(internodeTitle);
                            }
                        }
                        else if (inLineInfo == 7)// 文字列を含むかどうかで拾うのがむつかしい
                        {   // ３.節間日、開催年月日の取得
                            if (IsStringLine7(line))
                            {
                                raceBase.Place = place;
                                raceBase.InDay = Lib.IntParse(_StringExtraction(line, 5, 2));

                                DateTime dateTime = new DateTime(Lib.IntParse(_StringExtraction(line, 19, 4)),
                                                                    Lib.IntParse(_StringExtraction(line, 24, 2)),
                                                                    Lib.IntParse(_StringExtraction(line, 27, 2)));
                                raceBase.Date = dateTime;
                                if (entryfirst)
                                    internodeTitle.Start = dateTime;
                                internodeTitle.End = dateTime;
                            }
                            else
                            {
                                Console.WriteLine("Error:" + line + " " + raceBase.Date.ToString("yyyyMMdd"));
                            }

                        }
                        else if (IsStringInRoundInfo(line))
                        {   // ４.レース番号、レースタイトル、風向、風速の取得
                            race = raceBase.Clone();
                            race.RaceNum = Lib.IntParse(_StringExtraction(line, 2, 2));
                            race.Round2 = IsRound2(line);
                            // レースタイトル
                            var title = _StringExtraction(line, 12, 12);
                            if (RaceTitles.Exists(x => x.Title == title))
                            {
                                raceTitle = RaceTitles.Where(x => x.Title == title).FirstOrDefault();
                            }
                            else
                            {
                                raceTitle = new RaceTitle() { Title = title };
                                RaceTitles.Add(raceTitle);
                            }
                            // レース情報
                            raceInfo = new RaceInfo();
                            raceResult = new RaceResult();
                            raceInfo.WindDirectionSet(_StringExtraction(line, 59, 6));
                            raceInfo.WindMetor = Lib.IntParse(_StringExtraction(line, 65, 2));
                            raceInfo.RaceTitle = raceTitle;
                            raceInfo.Race = race;
                            raceInfo.RaceResult = raceResult;
                            raceInfo.InternodeTitle = internodeTitle;
                        }
                        else if (line.Contains(" 着 艇 登番 　選　手　名　　ﾓｰﾀｰ ﾎﾞｰﾄ 展示 進入 ｽﾀｰﾄﾀｲﾐﾝｸ ﾚｰｽﾀｲﾑ"))
                        {   // ５.決まり手の取得
                            raceInfo.KimariteSet(_StringExtraction(line, 66, 10));
                            inLinePlayer = -1; // この次の行が --------------だから
                        }
                        else if (1 <= inLinePlayer && inLinePlayer <= 6)// 文字列を含むかどうかで拾うのがむつかしい
                        {   // ６.選手結果の取得
                            PlayerResult adder = new PlayerResult();
                            adder.RaceInfo = raceInfo;
                            adder.Number = Lib.IntParse(_StringExtraction(line, 8, 4));
                            adder.Result = PlayerResult.ResultNumber(_StringExtraction(line, 2, 3));
                            adder.Wakuban = Lib.IntParse(_StringExtraction(line, 6, 1));
                            adder.Cource = Lib.IntParse(_StringExtraction(line, 46, 1));
                            adder.Motor = Lib.IntParse(_StringExtraction(line, 30, 2));
                            adder.Boat = Lib.IntParse(_StringExtraction(line, 34, 3));
                            adder.Tenji = Lib.DoubleParse(_StringExtraction(line, 39, 4));
                            adder.Start = Lib.DoubleParse(_StringExtraction(line, 51, 4));
                            PlayerResults.Add(adder);
                        }
                        else if (6 < inLinePlayer && !line.Contains("不成立"))
                        {   // ←７.レース結果の取得
                            if (line.Contains("単勝"))
                            {   // ここの特払い、傾向つかめれば統一したい。
                                if (line.Contains("特払い"))
                                {
                                    raceResult.Tansho.First = -1;
                                    raceResult.Tansho.Yen = Lib.IntParse(_StringExtraction(line, 27, 4));
                                }
                                else
                                {
                                    raceResult.Tansho.First = Lib.IntParse(_StringExtraction(line, 17, 1));
                                    raceResult.Tansho.Yen = Lib.IntParse(_StringExtraction(line, 25, 6));
                                }

                            }
                            if (line.Contains("複勝"))
                            {
                                raceResult.Fukusho1.First = Lib.IntParse(_StringExtraction(line, 17, 1));
                                raceResult.Fukusho1.Yen = Lib.IntParse(_StringExtraction(line, 25, 6));
                                raceResult.Fukusho2.First = Lib.IntParse(_StringExtraction(line, 33, 1));
                                raceResult.Fukusho2.Yen = Lib.IntParse(_StringExtraction(line, 41, 6));
                            }
                            if (line.Contains("２連単") || line.Contains("２連複") || line.Contains("各連複") ||
                                nextKakurenSecond || nextKakurenThired)
                            {
                                ResultTwo res = line.Contains("２連単") ? raceResult.NirenTan :
                                                line.Contains("２連複") ? raceResult.NirenFuku :
                                                line.Contains("各連複") ? raceResult.Kakuren1 :
                                                nextKakurenSecond ? raceResult.Kakuren2 : raceResult.Kakuren3;

                                res.First = Lib.IntParse(_StringExtraction(line, 17, 1));
                                res.Second = Lib.IntParse(_StringExtraction(line, 19, 1));
                                res.Yen = Lib.IntParse(_StringExtraction(line, 25, 6));
                                res.Popler = Lib.IntParse(_StringExtraction(line, 40, 3));
                                // 各連複の２．３番目は取れないので
                                nextKakurenSecond = res == raceResult.Kakuren1 ? true : false;
                                nextKakurenThired = res == raceResult.Kakuren2 ? true : false;
                            }
                            if (line.Contains("３連単") || line.Contains("３連複"))
                            {
                                ResultThree res = line.Contains("３連単") ? raceResult.SanrenTan : raceResult.SanrenFuku;

                                res.First = Lib.IntParse(_StringExtraction(line, 17, 1));
                                res.Second = Lib.IntParse(_StringExtraction(line, 19, 1));
                                res.Thired = Lib.IntParse(_StringExtraction(line, 21, 1));
                                res.Yen = Lib.IntParse(_StringExtraction(line, 25, 6));
                                res.Popler = Lib.IntParse(_StringExtraction(line, 40, 3));
                                if (res == raceResult.SanrenFuku)
                                    inLinePlayer = -9999;
                            }
                        }
                    }
                    inLinePlayer++;
                    inLineInfo++;
                }
            }
        }

        public void PlayerResultPrint()
        {
            foreach (var player in PlayerResults)
            {
                Console.WriteLine(player.Str);
            }
        }
    }
}
