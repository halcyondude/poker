using System;
using NUnit.Framework;
using pokerutils;

namespace pokerutils_test
{

    /*
    // https://en.wikipedia.org/wiki/List_of_poker_hands
    public enum HandCategories
    {
        FIVE_OF_KIND   = 0, // TODO: not supported, requires wild cards
        STRAIGHT_FLUSH = 1, // 5 sequential cards, same suit
        FOUR_OF_KIND   = 2, // 4x
        FULL_HOUSE     = 3, // 2x + 3x
        FLUSH          = 4, // same suit
        STRAIGHT       = 5, // 5 sequential cards
        THREE_OF_KIND  = 6, // 3x
        TWO_OF_KIND_2X = 7, // 2 * 2x
        TWO_OF_KIND    = 8, // 2x
        HIGH_CARD      = 9  // sad hand.  kickers contains the entire hand sorted. all hands start this way
    };
    */

    [TestFixture]
    public class HandEvaluatorTests
    {

        [Test]
        public void TestEvalHandSinglePass()
        {
            IEvaluateHand evaluateHand = new EvaluateHandProceduralSinglePass();
            TestHandVariants(evaluateHand);
        }

        private void TestHandVariants(IEvaluateHand evaluateHand)
        {
            if(null == evaluateHand)
                throw new ArgumentNullException("evaluateHand", "**cough** AHEM....null intf passed into test method");

            EvalHandResult ehr;

            // TODO: automate validation of kickers as well

            string[] saStraightFlush = {"JC", "10C", "9C", "8C", "7C"};
            ehr = evaluateHand.Evaluate(new Hand(saStraightFlush ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.STRAIGHT_FLUSH, ehr.category);

            // 4 of kind
            string[] saFourOfKind = {"5C", "5D", "5H", "5S", "2D"};
            ehr = evaluateHand.Evaluate(new Hand(saFourOfKind ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.FOUR_OF_KIND, ehr.category);

            // full house
            string[] saFullHouse = {"6S", "6H", "6D", "KS", "KH"};
            ehr = evaluateHand.Evaluate(new Hand(saFullHouse));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.FULL_HOUSE, ehr.category);

            // flush
            string[] saFlush = {"JD", "9D", "8D", "4D", "3D"};
            ehr = evaluateHand.Evaluate(new Hand(saFlush));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.FLUSH, ehr.category);

            // straight
            string[] saStraight = {"10D", "9S", "8H", "7D", "6C"};
            ehr = evaluateHand.Evaluate(new Hand(saStraight ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.STRAIGHT, ehr.category);

            // 3 of kind
            string[] saThreeOfKind = {"QC", "QS", "QH", "9H", "2S"};
            ehr = evaluateHand.Evaluate(new Hand(saThreeOfKind ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.THREE_OF_KIND, ehr.category);

            // two pairs
            string[] saTwoPair = {"JH", "JS", "3C", "3S", "2H"};
            ehr = evaluateHand.Evaluate(new Hand(saTwoPair ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.TWO_OF_KIND_2X, ehr.category);

            // 2 of kind
            string[] saPair = {"10S", "10H", "8S", "7H", "4C"};
            ehr = evaluateHand.Evaluate(new Hand(saPair));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.TWO_OF_KIND, ehr.category);

            // sad hand (high card)
            string[] saHighCard = {"KD", "QD", "7S", "4S", "3H"};
            ehr = evaluateHand.Evaluate(new Hand(saHighCard));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.HIGH_CARD, ehr.category);
        }
    }
}