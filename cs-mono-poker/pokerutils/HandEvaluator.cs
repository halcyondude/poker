using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

//
//
//
//
//
namespace pokerutils
{


    // enable compare/contrast different eval strategies easily, leveraging the same tests
    public interface IEvaluateHand
    {
        EvalHandResult Evaluate(Hand hand);
    }

    //
    // holding area for static helper funcs that don't have a home yet and/or might never see the light of day
    //
    //
    public class HandEvaluator
    {

        //
        // PickWinningHands - given an input hand of more than 5 cards, pick the winning hand(s) of 5 cards
        //
        public static List<Hand> PickWinningHands(string jsonInputHand)
        {
            Console.WriteLine("------------------");
            Console.WriteLine("PickWinningHands()");
            Console.WriteLine("------------------");

            IEvaluateHand evaluator = new EvaluateHandProceduralSinglePass();

            Console.WriteLine();
            List<Hand> allHands = HandFactory.CreateAllPossibleHands(jsonInputHand);
            List<EvalHandResult> winners = PickWinnersBruteForce(evaluator, allHands);

            // TODO: clean this up...too many silly copies...
            List<Hand> winningHands = new List<Hand>(winners.Count);
            foreach(EvalHandResult ehr in winners)
                winningHands.Add(ehr.hand);

            return winningHands;

        }

        //
        // PickWinners() - pick the winner(s) from a list of hands.  Evaluate all hands and score.
        //
        // note: in poker ties are possible, hence the array return type
        //
        public static List<EvalHandResult> PickWinnersBruteForce(IEvaluateHand evaluator, List<Hand> handsToScore)
        {
            //Console.WriteLine("-----------------------");
            //Console.WriteLine("PickWinnersBruteForce()");
            //Console.WriteLine("-----------------------");

            if (null == evaluator)
                throw new ArgumentNullException(nameof(evaluator));

            if (null == handsToScore)
                throw new ArgumentNullException(nameof(handsToScore));


            Console.WriteLine(Environment.NewLine + "Evaluating the following hands...");
            foreach(Hand h in handsToScore)
                Console.WriteLine("{0}", h);
            Console.WriteLine("");

            //
            // evaluate all hands, building results histogram by category.  [0] is highest.
            //
            // note: SortedDictionary has faster insertion, and the same (log n) retrevial as SortedList, but uses a wee bit more memory.
            //       as these set sizes are modest, this will do.
            SortedDictionary<int, List<EvalHandResult>> resultsByCategory = new SortedDictionary<int, List<EvalHandResult>>();

            foreach (Hand hand in handsToScore)
            {
                EvalHandResult ehr = evaluator.Evaluate(hand);
                Console.WriteLine(ehr);

                if (!resultsByCategory.ContainsKey((int) ehr.category))
                {
                    List<EvalHandResult> resList = new List<EvalHandResult>();
                    resList.Add(ehr);
                    resultsByCategory.Add((int) ehr.category, resList);
                }
                else
                {
                    resultsByCategory[(int) ehr.category].Add(ehr);
                }

                //Console.WriteLine("Eval --> {0}", hand);
            }

            // keys are sorted, so fetching the min (best hand) is O(1)
            int winningCategory = resultsByCategory.Keys.Min();

            if (winningCategory < 0)
                throw new InvalidOperationException(
                    "Internal error on a massive scale.  Seek professional help immediately.");

            List<EvalHandResult> winners = resultsByCategory[winningCategory];
            if (winners.Count > 1)
            {
                // we have multiple hands in the winning category
                // determine for that type of hand which {"one is", "ones are"} the winner(s)
                winners = PickWinnersByCategory(winners, (HandCategories) winningCategory);
            }

            Console.WriteLine("Winning Hand(s)");
            foreach(EvalHandResult ehr in winners)
                Console.WriteLine("\t{0}", ehr);

            return winners;

        }

        // This will plow thru input, returning the list of all results with highest rank for **rankHigh** (e.g. ties)
        private static List<EvalHandResult> FetchBestRankHigh(List<EvalHandResult> input)
        {
            if(0 == input.Count)
                throw new ArgumentOutOfRangeException("FetchBestRankHigh() - called with an empty input list");

            // this would be silly but covering case anyway.
            if (1 == input.Count)
                return input;

            List<EvalHandResult> ret = new List<EvalHandResult>();
            Ranks highestRankObserved = input[0].rankHigh;
            ret.Add(input[0]);

            for (int i = 1; i < input.Count; i++)
            {
                EvalHandResult ehr = input[i];

                if(ehr.rankHigh < highestRankObserved)
                    continue;

                if (ehr.rankHigh == highestRankObserved)
                {
                    // tie
                    ret.Add(ehr);
                }
                else if (ehr.rankHigh > highestRankObserved)
                {
                    // this feels wrong/dirty.  but works.  done > perfect.
                    highestRankObserved = ehr.rankHigh;
                    ret.Clear();
                    ret.Add(ehr);
                }
            }

            return ret;
        }

        // This will plow thru input, returning the list of all results with highest rank for **rankLow** (e.g. ties)
        // note: if C# had a preprocessor...would have a combined method.  There are some reflection gimmicks that could be used here...but yuck.
        private static List<EvalHandResult> FetchBestRankLow(List<EvalHandResult> input)
        {
            if(0 == input.Count)
                throw new ArgumentOutOfRangeException("FetchBestRankLow() - called with an empty input list");

            // this would be silly but covering case anyway.
            if (1 == input.Count)
                return input;

            List<EvalHandResult> ret = new List<EvalHandResult>();
            Ranks highestRankObserved = input[0].rankLow;
            ret.Add(input[0]);

            for (int i = 1; i < input.Count; i++)
            {
                EvalHandResult ehr = input[i];

                if (ehr.rankLow < highestRankObserved)
                    continue;

                if (ehr.rankLow == highestRankObserved)
                {
                    ret.Add(ehr);
                }
                else if (ehr.rankLow > highestRankObserved)
                {
                    // this feels wrong/dirty.  but works.  done > perfect.
                    highestRankObserved = ehr.rankLow;
                    ret.Clear();
                    ret.Add(ehr);
                }
            }

            return ret;
        }

        private static List<EvalHandResult> FetchBestRankHighThenRankLowIfNeeded(List<EvalHandResult> input)
        {
            List<EvalHandResult> ret = FetchBestRankHigh(input);
            if (ret.Count > 1)
            {
                ret = FetchBestRankLow(ret);
            }

            return ret;
        }

        //
        // note: assumes all items are of the same category
        //
        private static List<EvalHandResult> PickWinnersByCategory(List<EvalHandResult> finalists,
            HandCategories category)
        {
            // TODO: validate params
            // TODO: brute forcing to get this working rapidly.  This is ripe for optimization.  For now utilizing multi-pass


            List<EvalHandResult> ultimateWinners = new List<EvalHandResult>();

            switch (category)
            {
                // high card is all that matters.
                case HandCategories.STRAIGHT_FLUSH:
                case HandCategories.STRAIGHT:
                    ultimateWinners = FetchBestRankHigh(finalists);
                    break;

                //  high (3x), then low (2x)
                case HandCategories.FULL_HOUSE:
                    ultimateWinners = FetchBestRankHighThenRankLowIfNeeded(finalists);
                    break;

                // high pair, low pair, kicker
                case HandCategories.TWO_OF_KIND_2X:
                    ultimateWinners = FetchBestRankHighThenRankLowIfNeeded(finalists);
                    if (ultimateWinners.Count > 1)
                    {
                        // highly unlikely tie.  time for kickers.
                        // TODO:

                    }
                    break;

                // rank of kind, then kickers
                case HandCategories.FOUR_OF_KIND:
                case HandCategories.THREE_OF_KIND:
                case HandCategories.TWO_OF_KIND:
                    ultimateWinners = FetchBestRankHigh(finalists);
                    if (ultimateWinners.Count > 1)
                    {
                        // TODO kickers
                    }
                    break;


                // kickers only
                case HandCategories.FLUSH:
                case HandCategories.HIGH_CARD:
                    // TODO: kickers
                    break;

                // unsupported
                case HandCategories.FIVE_OF_KIND:
                    throw new NotImplementedException("Error: 5 of a kind is not yet implemented");
                    break;

                default:
                    throw new InvalidEnumArgumentException("Error: new type of hand is not yet supported");
                    break;
            }

            return ultimateWinners;

        }
    }
}
