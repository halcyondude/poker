using System;
using System.Diagnostics;
using System.Linq;

namespace pokerutils
{
    public class TestClass
    {
        public void TestMethod()
        {
            Console.WriteLine("Testing Enum values");

            Ranks rankTen = Ranks.TEN;
            Ranks rankFive = Ranks.FIVE;

            if (rankFive < rankTen)
                Console.WriteLine("sanity");
            else
                Console.WriteLine("insanity");
        }

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
                Console.WriteLine(Environment.NewLine + "Testing Card( " + input + ")");

                Card card = new Card(input);

                Console.WriteLine(" --> " + card.ToString());
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public void TestCardClass()
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
                    Debug.Assert(true == TryCreateCard(testInputString), testInputString);
                }
            }

            Console.WriteLine("-------------------------------");
            Console.WriteLine("TESTING INVALID/BAD CARD INPUTS");
            Console.WriteLine("-------------------------------");

            // TODO: figure out NUNIT on linux

            // empty string
            Debug.Assert(false == TryCreateCard(""), "empty string");
            Debug.Assert(false == TryCreateCard(string.Empty), "String.Empty");

            // suits that don't exist
            Debug.Assert(false == TryCreateCard("7X"), "7 of X");

            // ranks that don't exist
            Debug.Assert(false == TryCreateCard("XH"), "X of Hearts");

            // suits and ranks that don't exist
            Debug.Assert(false == TryCreateCard("99Z"), "99 of Zeldas");

            // long strings
            Debug.Assert(false == TryCreateCard("SomeSillyLongString"), "long string");

            // garbage input
            Debug.Assert(false == TryCreateCard("!@"), "garbage symbols");

            // 2 spaces
            Debug.Assert(false == TryCreateCard("  "), "2 spaces");
            // 3 spaces
            Debug.Assert(false == TryCreateCard("   "), "3 spaces");

        }

        public void TestDealHand()
        {
            Deck deck = new Deck();
            Hand hand = deck.Deal(52);

            Console.WriteLine("SHUFFLE - NO");
            foreach(Card c in hand)
                Console.WriteLine(c.ToString());

            deck.Return(hand);

            Console.WriteLine("SHUFFLE - YES");
            deck.Shuffle();
            hand = deck.Deal(52);
            foreach(Card c in hand)
                Console.WriteLine(c.ToString());

        }

        public void TestPrettyPrint()
        {
            Deck deck = new Deck();
            Hand hand = deck.Deal(52);

            foreach (Card c in hand)
            {
                Console.WriteLine(c.ToStringDebug());
            }

        }

    }
}