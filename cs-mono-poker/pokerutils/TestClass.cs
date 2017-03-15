using System;
using System.Text;
using Newtonsoft.Json;


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

        public void PrototypeUnicode()
        {
            // TODO: this works, everything below does not!
            Console.Write("Testing Unicode Card: {0}\n", "\U0001F0A1");

            uint card_base = 0x1F0A0;
            uint card_suffix = 0x1;

            uint card_combined = card_base | card_suffix;

            uint hrm = 0x1F0A1;
            char c = (char)hrm ;
            Console.WriteLine("simple cast: {0}", c);

            byte[] byteArray = BitConverter.GetBytes(card_combined);

            var unicodeString = Encoding.Unicode.GetString(byteArray);
            Console.WriteLine("BitConverter to the rescue! {0}", unicodeString);

            Decoder d = Encoding.Unicode.GetDecoder();
            char[] resultCharArray = new char[2];

            var takeTwo = d.GetChars(byteArray, 0, byteArray.Length, resultCharArray, 0);
            Console.WriteLine("Take2: {0}", resultCharArray.ToString());
        }

        public void TestJsonParse()
        {
            string sampleInput = "[\"JH\", \"4C\", \"4S\", \"JC\", \"9H\"]";
            Console.WriteLine(String.Format("Testing String: {0}", sampleInput));

            var ret = JsonConvert.DeserializeObject<string[]>(sampleInput);

        }

    }
}