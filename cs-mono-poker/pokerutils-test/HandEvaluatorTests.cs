using System;
using System.Collections.Generic;
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
            IEvaluateHand evaluator = new EvaluateHandProceduralSinglePass();

            // test each hand type, collecting the results
            List<EvalHandResult> handResults = TestHandVariants(evaluator);

            // gather hands (yes this is contrived, rapid prototype :))
            List<Hand> allHands = new List<Hand>(handResults.Count);
            foreach (EvalHandResult ehr in handResults)
                allHands.Add(ehr.hand);

            TestPickWinner(evaluator, allHands);
        }

        // one test for each hand variant
        // uses sample hands from: https://en.wikipedia.org/wiki/List_of_poker_hands#Full_house
        private List<EvalHandResult> TestHandVariants(IEvaluateHand evaluateHand)
        {
            if(null == evaluateHand)
                throw new ArgumentNullException("evaluateHand", "**cough** AHEM....null intf passed into test method");

            List<EvalHandResult> retList = new List<EvalHandResult>();
            EvalHandResult ehr;


            // TODO: automate validation of kickers as well

            string[] saStraightFlush = {"JC", "10C", "9C", "8C", "7C"};
            ehr = evaluateHand.Evaluate(new Hand(saStraightFlush ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.STRAIGHT_FLUSH, ehr.category);
            retList.Add(ehr);

            // 4 of kind
            string[] saFourOfKind = {"5C", "5D", "5H", "5S", "2D"};
            ehr = evaluateHand.Evaluate(new Hand(saFourOfKind ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.FOUR_OF_KIND, ehr.category);
            retList.Add(ehr);

            // full house
            string[] saFullHouse = {"6S", "6H", "6D", "KS", "KH"};
            ehr = evaluateHand.Evaluate(new Hand(saFullHouse));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.FULL_HOUSE, ehr.category);
            retList.Add(ehr);

            // flush
            string[] saFlush = {"JD", "9D", "8D", "4D", "3D"};
            ehr = evaluateHand.Evaluate(new Hand(saFlush));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.FLUSH, ehr.category);
            retList.Add(ehr);

            // straight
            string[] saStraight = {"10D", "9S", "8H", "7D", "6C"};
            ehr = evaluateHand.Evaluate(new Hand(saStraight ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.STRAIGHT, ehr.category);
            retList.Add(ehr);

            // 3 of kind
            string[] saThreeOfKind = {"QC", "QS", "QH", "9H", "2S"};
            ehr = evaluateHand.Evaluate(new Hand(saThreeOfKind ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.THREE_OF_KIND, ehr.category);
            retList.Add(ehr);

            // two pairs
            string[] saTwoPair = {"JH", "JS", "3C", "3S", "2H"};
            ehr = evaluateHand.Evaluate(new Hand(saTwoPair ));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.TWO_OF_KIND_2X, ehr.category);
            retList.Add(ehr);

            // 2 of kind
            string[] saPair = {"10S", "10H", "8S", "7H", "4C"};
            ehr = evaluateHand.Evaluate(new Hand(saPair));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.TWO_OF_KIND, ehr.category);
            retList.Add(ehr);

            // sad hand (high card)
            string[] saHighCard = {"KD", "QD", "7S", "4S", "3H"};
            ehr = evaluateHand.Evaluate(new Hand(saHighCard));
            Console.WriteLine(ehr);
            Assert.AreEqual(HandCategories.HIGH_CARD, ehr.category);
            retList.Add(ehr);

            return retList;
        }


        //
        // given a set of hands, pick the winner.
        //
        private void TestPickWinner(IEvaluateHand evaluator, List<Hand> handsToEvaluate)
        {
            List<EvalHandResult> winners = HandEvaluator.PickWinnersBruteForce(evaluator, handsToEvaluate);
        }
    }
}