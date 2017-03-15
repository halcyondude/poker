using System;
using NUnit.Framework;
using pokerutils;

namespace pokerutils_test
{
    [TestFixture]
    public class CardTests
    {
        private static string[] testRanks =
        {
            "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "j", "J", "k", "K", "q", "Q", "a", "A"
        };

        private static string[] testSuits =
        {
            "h", "H", "d", "D", "s", "S", "c", "C"
        };

        public bool TryCreateCard(string input)
        {
            try
            {
                Console.Write("Testing Card( " + input + " )");
                Card card = new Card(input);
                Console.Write(" --> " + card.ToString() + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        [Test]
        public void TestCardInputs()
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine("TESTING ALL VALID CARD INPUTS");
            Console.WriteLine("-----------------------------");

            // get all supported rank/suit combos to ensure all positive cards construct/parse correctly
            foreach (string testSuit in testSuits)
            {
                foreach (string testRank in testRanks)
                {
                    string testInputString = testRank + testSuit;
                    Assert.True(TryCreateCard(testInputString), testInputString);
                }
            }

            Console.WriteLine("-------------------------------");
            Console.WriteLine("TESTING INVALID/BAD CARD INPUTS");
            Console.WriteLine("-------------------------------");

            // empty string
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("");
            });

            // also empty string
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card(string.Empty);
            });

            // suits that don't exist
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("7X");
            });

            // ranks that don't exist
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("XH");
            });

            // suits and ranks that don't exist (e.g. 99 of Zeldas)
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("99Z");
            });

            // long strings
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("SomeSillyLongString");
            });

            // garbage input
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("!@");
            });

            // 2 spaces
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("  ");
            });

            // 3 spaces
            Assert.Throws(typeof(ArgumentException), () =>
            {
                Card c = new Card("   ");
            });
        }

    }
}