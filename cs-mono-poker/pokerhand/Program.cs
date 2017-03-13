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

            log("------------------------------------");
            log("pokerhand - for all your poker needs");
            log("------------------------------------");

            TestClass tc = new TestClass();
            tc.TestMethod();
            tc.TestCardClass();

            log("------------------------------------");
            log("testing dealing hands");
            log("------------------------------------");
            tc.TestDealHand();

            log("------------------------------------");
            log("testing pretty print for cards");
            log("------------------------------------");
            tc.TestPrettyPrint();

            tc.PrototypeUnicode();

        }
    }
}