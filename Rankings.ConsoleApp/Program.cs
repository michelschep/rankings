using System;
using Rankings.Core.Services;

namespace Rankings.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
//            Console.WriteLine("Hello World!");
//
//            for (int elo = 0; elo < 400; elo += 100)
//            {
//                var oneSetChance = EloCalculator.ExpectationOneSet(1200, 1200 + elo);
//                var resultChance = EloCalculator.ChanceOfHavingThisResult(1, 0, oneSetChance);
//                Console.WriteLine($"1200 - {1200 + elo} => {oneSetChance}/{resultChance}");
//            }
//            Console.WriteLine("================================");
            
            for (int elo = 0; elo < 400; elo += 100)
            {
                Console.WriteLine($"========== 1200 - { 1200 + elo}  ======================");
                var oneSetChance = NewEloCalculator.ExpectationOneSet(1200, 1200 + elo);
                var resultChance = NewEloCalculator.ChanceOfHavingThisResult(2, 1, oneSetChance);
                Console.WriteLine($"2-1 => {oneSetChance}/{resultChance}");
                resultChance = NewEloCalculator.ChanceOfHavingThisResult(1, 2, oneSetChance);
                Console.WriteLine($"1-2 => {oneSetChance}/{resultChance}");
                resultChance = NewEloCalculator.ChanceOfHavingThisResult(3, 0, oneSetChance);
                Console.WriteLine($"3-0 => {oneSetChance}/{resultChance}");
                resultChance = NewEloCalculator.ChanceOfHavingThisResult(0, 3, oneSetChance);
                Console.WriteLine($"0-3 => {oneSetChance}/{resultChance}");
            }
//            Console.WriteLine("================================");
//
//            for (int elo = 0; elo < 400; elo += 100)
//            {
//                var expectation = EloCalculator.CalculateExpectation(1200, 1200 + elo, 1);
//                Console.WriteLine($"1200 - {1200+elo} => {expectation}");
//            }
//            Console.WriteLine("================================");
//            for (int elo = 0; elo < 400; elo += 100)
//            {
//                var expectation = EloCalculator.CalculateExpectation(1200, 1200 + elo, 2);
//                Console.WriteLine($"1200 - {1200 + elo} => {expectation}");
//            }
//            Console.WriteLine("================================");
//            for (int elo = 0; elo < 400; elo += 100)
//            {
//                var expectation = EloCalculator.CalculateExpectation(1200, 1200 + elo, 3);
//                Console.WriteLine($"1200 - {1200 + elo} => {expectation}");
//            }
            Console.ReadLine();
        }
    }
}
