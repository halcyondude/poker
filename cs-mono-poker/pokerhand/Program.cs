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