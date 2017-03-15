using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

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
        // TODO: we probably don't need this ctor
        public EvalHandResult(HandCategories categoryIn, Ranks rankHighIn, Ranks rankLowIn, Ranks[] kickersIn)
        {
            category = categoryIn;
            rankHigh = rankHighIn;
            rankLow  = rankLowIn;
            kickers  = kickersIn;
        }

        public HandCategories category; // highest category that applies
        public Ranks          rankHigh; // see above
        public Ranks          rankLow;  // see above
        public Ranks[]        kickers;  // sorted array of kickers, [0] is highest.  could be null

        public override string ToString()
        {
            // TODO: IMPLEMENT ME! (EvalHandResult.ToString()
            return base.ToString();
        }
    };

    //
    // there is a bit of duplication in this class, for $reasons (bulk/batch hand eval)
    //
    public class HandEvaluator
    {


        //
        // note: heavy use of asserts as this is being written in a hurry
        //
        public static EvalHandResult EvalHandSinglePass(Hand hand)
        {
            // will be set to false by hand traversal as we eval if suits don't match / flush
            bool isFlush = true;
            Suits flushCheckSuit = hand[0].suit;

            // for straight detection
            Ranks maxRank = hand[0].rank;
            Ranks minRank = maxRank;

            // detect 2's, 3's, 4's.  Histogram := counter per Rank
            int nRanks = Enum.GetValues(typeof(Ranks)).Length;
            int[] rankHistogram = new int[nRanks];
            Array.Clear(rankHistogram, 0, nRanks);

            for (int i = 0; i < hand.Count; i++)
            {
                Card c = hand[i];
                Ranks r = c.rank;
                Suits s = c.suit;

                if (flushCheckSuit != s)
                    isFlush = false;

                if (r > maxRank)
                    maxRank = r;

                if (r < minRank)
                    minRank = r;

                // increment counter for this particular card Rank, stash value (note...compiler will use a register)
                rankHistogram[(int) r]++;
            }

            bool isStraight = (4 == (maxRank - minRank));

            // at this point we know if we have a straight, flush, and a histogram of cards w/ ranks.  Woot.
            EvalHandResult ehr = new EvalHandResult();

            // TODO: if we were plowing through thousands+ of hands looking for winners, this is fast to compute, and would fast-cull losers
            if (isStraight && isFlush)
            {
                ehr.category = HandCategories.STRAIGHT_FLUSH;
                ehr.rankHigh = maxRank;
                return ehr;
            }

            // straight
            if (isStraight)
            {
                ehr.category = HandCategories.STRAIGHT;
                ehr.rankHigh = maxRank;
                return ehr;
            }

            if (isFlush)
            {
                ehr.category = HandCategories.FLUSH;

                // note: we want high to low...
                hand.Sort((x, y) => y.rank.CompareTo(x.rank));
                Debug.Assert(hand[0].rank == maxRank);

                ehr.kickers = new Ranks[hand.Count];
                for (int i = 0; i < hand.Count; i++)
                    ehr.kickers[i] = hand[i].rank;

                return ehr;
            }

            // We need to use the histogram.   Process it with a single pass.  There's only a few variations possible for poker hands.
            // TODO: This screams of a pattern matching algo using bitfields and fast compares.  explore this idea later

            // POSSIBLE HISTOGRAM CONFIGS
            // 4,1       := 4's, 1 kicker     (2 ranks total)
            // 3,2       := full house        (2 ranks total)
            // 3,1,1     := 3's,    2 kickers (3 ranks total)
            // 2,2,1     := 2 pair, 1 kickers (3 ranks total)
            // 2,1,1,1   := 1 pair, 3 kickers (4 ranks total)
            // 1,1,1,1,1 := high card.  sad.  (5 ranks total)

            // key: count from histogram, value: List<ranks>.
            Dictionary<int, List<Ranks>> countToSortedRanks = new Dictionary<int, List<Ranks>>(5);

            // populate
            for (int i = 0; i < nRanks; i++)
            {
                int countValue = rankHistogram[i];

                if (0 == countValue)
                    continue;

                Ranks r = (Ranks) i;

                if (!countToSortedRanks.ContainsKey(countValue))
                {
                    List<Ranks> hs = new List<Ranks>();
                    hs.Add(r);
                    countToSortedRanks.Add(countValue, hs);
                }
                else
                {
                    countToSortedRanks[countValue].Add(r);
                }
            }

            // use
            if (countToSortedRanks.ContainsKey(4)) // 4 of a kind
            {
                // we should have a single kicker
                Debug.Assert(countToSortedRanks.ContainsKey(1));
                Debug.Assert(countToSortedRanks.Keys.Count == 2);

                ehr.category = HandCategories.FOUR_OF_KIND;
                ehr.rankHigh = countToSortedRanks[4][0];
                ehr.kickers = new Ranks[1];
                ehr.kickers[0] = countToSortedRanks[1][0];
            }
            else if (countToSortedRanks.ContainsKey(3) && countToSortedRanks.ContainsKey(2))
            {
                Debug.Assert(countToSortedRanks.Keys.Count == 2); // 2 ranks total

                ehr.category = HandCategories.FULL_HOUSE;
                ehr.rankHigh = countToSortedRanks[3][0];
                ehr.rankLow = countToSortedRanks[2][0];
            }
            else if (countToSortedRanks.ContainsKey(3) && countToSortedRanks.ContainsKey(1))
            {
                // we should have 2 kickers
                Debug.Assert(countToSortedRanks.Keys.Count == 3); // 3 ranks total
                Debug.Assert(countToSortedRanks[1].Count == 2);   // 2 kickers

                ehr.category = HandCategories.THREE_OF_KIND;
                ehr.rankHigh = countToSortedRanks[3][0];
                ehr.kickers = countToSortedRanks[1].ToArray();

                // kickers need to be high > low, in this case there are only 2 kickers.
                if(ehr.kickers[0] < ehr.kickers[1])
                    Swap(ref ehr.kickers[0], ref ehr.kickers[1]);

                Debug.Assert(ehr.kickers[0] > ehr.kickers[1]);
            }
            else if (countToSortedRanks.ContainsKey(2) && countToSortedRanks.ContainsKey(1))
            {
                // this is one of 2 cases:
                // 2,2,1     := 2 pair, 1 kickers (3 ranks total)
                if ((countToSortedRanks[2].Count == 2) && (countToSortedRanks[1].Count == 1))
                {
                    ehr.category = HandCategories.TWO_OF_KIND_2X;
                    ehr.rankHigh = countToSortedRanks[2][0];
                    ehr.rankLow = countToSortedRanks[2][1];

                    // on average we'll need to swap half the time. :)
                    if(ehr.rankLow > ehr.rankHigh)
                        Swap(ref ehr.rankLow, ref ehr.rankHigh);

                    // only 1 kicker
                    ehr.kickers = countToSortedRanks[1].ToArray();

                }
                // 2,1,1,1   := 1 pair, 3 kickers (4 ranks total)
                else if ((countToSortedRanks[2].Count == 1) && (countToSortedRanks[1].Count == 3))
                {
                    ehr.category = HandCategories.TWO_OF_KIND;
                    ehr.rankHigh = countToSortedRanks[2][0];

                    // note: this gives us high-to-low
                    countToSortedRanks[1].Sort((x, y) => y.CompareTo(x));
                    ehr.kickers = countToSortedRanks[1].ToArray();
                }
                else
                {
                    // ruh-roh, we're not understanding something basic here...or not covering a use case correctly.  bail.
                    // https://msdn.microsoft.com/en-us/library/system.invalidoperationexception(v=vs.110).aspx
                    Debug.Assert(false);
                    throw new InvalidOperationException("Hand evaluation logic has an internal error!");
                }
            }
            else if (countToSortedRanks.ContainsKey(1))           // high card (sad hand)
            {
                Debug.Assert(countToSortedRanks.Keys.Count == 5); // 5 ranks total
                Debug.Assert(countToSortedRanks[1].Count == 5);   // all ranks are single card

                ehr.category = HandCategories.HIGH_CARD;
                countToSortedRanks[1].Sort((x, y) => y.CompareTo(x));
                ehr.kickers = countToSortedRanks[1].ToArray();
            }
            else
            {
                // should never get here
                Debug.Assert(false);
                throw new InvalidOperationException("Hand evaluation logic has an internal error!");
            }

            return ehr;
        }


        static void Swap(ref Ranks x, ref Ranks y)
        {
#if true
            Ranks tempswap = x;
            x = y;
            y = tempswap;
#else
            // TODO: validate that in C# this is actually better (vs. wonky boxing etc)
            x ^= y;
            y ^= x;
            x ^= y;
#endif
        }

    }

    //
    // holding area for static helper funcs that don't have a home yet
    //
    public class HandEvaluatorMiscUtils
    {
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
