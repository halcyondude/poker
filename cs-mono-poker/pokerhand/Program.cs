using System;
using System.Collections.Generic;
using pokerutils;

namespace pokerhand
{
    internal class Program
    {
        public static void log(string s)
        {
            Console.WriteLine(s);
            //var v = new System.Globalization.SortVersion();
        }

        public static void Main(string[] args)
        {

            log("--------------------");
            log("welcome to pokerhand");
            log("--------------------");
            string sevenCardHand = "[\"3H\", \"7S\", \"3S\", \"QD\", \"AH\", \"3D\", \"4S\"]";
            string expectedOutput = "[\"3H\", \"3S\", \"3D\", \"AH\", \"QD\"]";

            Console.WriteLine("input : {0}", sevenCardHand);
            Console.WriteLine("output: {0}", expectedOutput);

            Hand inputHand = new Hand(sevenCardHand);
            Hand expectedHand = new Hand(expectedOutput);

            List<Hand> winners = HandEvaluator.PickWinningHands(sevenCardHand);

            Console.WriteLine("Expected Hand:\n{0}", expectedHand);



        }

        public static void Proto()
        {
            TestClass tc = new TestClass();
            tc.TestMethod();


            log("------------------------------------");
            log("testing dealing hands");
            log("------------------------------------");
            tc.TestDealHand();

            log("------------------------------------");
            log("testing pretty print for cards");
            log("------------------------------------");
            tc.TestPrettyPrint();

            tc.PrototypeUnicode();

            //
            tc.TestJsonParse();

        }

    }
}