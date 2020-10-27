using System;
using System.IO;

namespace BoatRaceResultReader
{

    class Program
    {
        static void Main(string[] args)
        {
            RaceResults raceResults = new RaceResults();
            var files = Directory.GetFiles("Results","K??????.TXT");
            foreach(var file in files)
            {
                raceResults.Read(file);
            }
            raceResults.PlayerResultPrint();

        }
    }
}
