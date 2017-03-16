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

    //
    // a few notes about how the this struct works.
    //
    // - flushes (both)  --> rankHigh is highest card in the straight
    // - "of kind" hands --> rankHigh is the rank of the pair/triple/quad/quintuple
    // - two_of_kind_2x  --> rankHigh, rankLow both used
    // - full house      --> rankHigh is 3x, rankLow is 2x
    // - kickers         --> "leftover" cards for 'of kind' hands that will break ties.
    //
    public struct EvalHandResult
    {
        public Hand           hand;     // really only for printing / debugging.  not needed

        public HandCategories category; // highest category that applies
        public Ranks          rankHigh; // see above
        public Ranks          rankLow;  // see above
        public Ranks[]        kickers;  // sorted array of kickers, [0] is highest.  could be null

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // first dump the hand
            sb.AppendFormat("{0}:= ", hand);

            switch (category)
            {
                case HandCategories.FIVE_OF_KIND:
                    throw new NotImplementedException("Error: 5 of a kind is not yet implemented");
                    break;

                case HandCategories.STRAIGHT_FLUSH:
                    sb.AppendFormat("Straight Flush, High: {0}", pretty(rankHigh));
                    break;

                case HandCategories.FOUR_OF_KIND:
                    sb.AppendFormat("4 of a kind ({0}'s), Kicker: {1}", pretty(rankHigh), pretty(kickers[0]));
                    break;

                case HandCategories.FULL_HOUSE:
                    sb.AppendFormat("Full House, (3*{0}'s, 2*{1}'s)", pretty(rankHigh), pretty(rankLow));
                    break;

                case HandCategories.FLUSH:
                    sb.Append("Flush, Ordered Ranks: ");
                    foreach (Ranks r in kickers)
                        sb.AppendFormat("{0} ", pretty(r));
                    break;

                case HandCategories.STRAIGHT:
                    sb.AppendFormat("Straight, High: {0}", pretty(rankHigh));
                    break;

                case HandCategories.THREE_OF_KIND:
                    sb.AppendFormat("3 of a kind ({0}'s), Kickers: ", pretty(rankHigh));
                    foreach (Ranks r in kickers)
                        sb.AppendFormat("{0} ", pretty(r));
                    break;

                case HandCategories.TWO_OF_KIND_2X:
                    sb.AppendFormat("2 pairs, ({0}'s and {1}'s), Kicker: {2}", pretty(rankHigh), pretty(rankLow), pretty(kickers[0]));
                    break;

                case HandCategories.TWO_OF_KIND:
                    sb.AppendFormat("2 of a kind ({0}'s), Kickers: ", pretty(rankHigh));
                    foreach (Ranks r in kickers)
                        sb.AppendFormat("{0} ", pretty(r));
                    break;

                case HandCategories.HIGH_CARD:
                    sb.Append("High Card (sad hand).  Just for kickers: ");
                    foreach (Ranks r in kickers)
                        sb.AppendFormat("{0} ", pretty(r));
                    break;

                default:
                    throw new InvalidEnumArgumentException("Error: new type of hand is not yet supported");
            }

            return sb.ToString();
        }

        // TODO: this is less than ideal, but better than carrying info thru core eval code
        private string pretty(Ranks r)
        {
            return Card._dictPrettyRank[r].Display;
        }

    };

    // enable compare/contrast different eval strategies easily, leveraging the same tests
    public interface IEvaluateHand
    {
        EvalHandResult Evaluate(Hand hand);
    }

    //
    // holding area for static helper funcs that don't have a home yet and/or might never see the light of day
    //
    //
    public class HandEvaluatorMiscUtils
    {
        //
        // PickWinners() - pick the winner(s) from a list of hands.  Evaluate all hands and score.
        //
        // note: in poker ties are possible, hence the array return type
        //
        public static List<EvalHandResult> PickWinnersBruteForce(IEvaluateHand evaluator, List<Hand> handsToScore)
        {
            Console.WriteLine("-----------------------");
            Console.WriteLine("PickWinnersBruteForce()");
            Console.WriteLine("-----------------------");


            if (null == evaluator)
                throw new ArgumentNullException(nameof(evaluator));

            if (null == handsToScore)
                throw new ArgumentNullException(nameof(handsToScore));


            Console.WriteLine(Environment.NewLine + "Evaluating the following hands...");
            foreach(Hand h in handsToScore)
                Console.WriteLine("\t{0}", h);

            //
            // evaluate all hands, building results histogram by category.  [0] is highest.
            //
            // note: SortedDictionary has faster insertion, and the same (log n) retrevial as SortedList, but uses a wee bit more memory.
            //       as these set sizes are modest, this will do.
            SortedDictionary<int, List<EvalHandResult>> resultsByCategory = new SortedDictionary<int, List<EvalHandResult>>();

            foreach (Hand hand in handsToScore)
            {
                EvalHandResult ehr = evaluator.Evaluate(hand);

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

                Console.WriteLine("Eval --> {0}", hand);
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
                Console.WriteLine("\t{0}", ehr.hand);

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


        public static bool IsFlush(Hand hand)
        {
            Suits s = hand[0].suit;

            for (int i = 1; i < hand.Count; i++)
            {
                if (s != hand[i].suit)
                    return false;
            }

            return true;
        }

        // note: this is for a 5 card hand
        public static bool IsStraight(Hand hand, out Ranks rankHighCardOut)
        {
            // in-place sort by rank descending (highest first)
            hand.Sort((x, y) => x.rank.CompareTo(y.rank));

            // a straight is all 5 cards, monotonically increasing.  for example 3,4,5,6,7. delta(min, max) is always 4
            rankHighCardOut = hand[0].rank;

            int delta = hand[0].rank - hand[4].rank;
            return 4 == delta;
        }
    }
}
