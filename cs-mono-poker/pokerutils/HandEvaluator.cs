using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        HIGH_CARD      = 9  // sad hand.  kickers contains the entire hand sorted.
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
    public struct HandEvalResult
    {
        public HandEvalResult(HandCategories categoryIn, Ranks rankHighIn, Ranks rankLowIn, Ranks[] kickersIn)
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
    };

    //
    // there is a bit of duplication in this class, for $reasons
    //
    public class HandEvaluator
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

        public static HandEvalResult EvaluateHandSinglePass(Hand hand)
        {
            // will be set to false by hand traversal
            bool isFlush = true;
            Suits flushCheckSuit = hand[0].suit;

            // for straight detection
            Ranks maxRank = hand[0].rank;
            Ranks minRank = maxRank;

            // detect 2's, 3's, 4's
            uint[] rankHistogram = new uint[Enum.GetValues(typeof(Ranks)).Length];   // create counters for each Rank
            Array.Clear(rankHistogram, 0, rankHistogram.Length);                     // memset()

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

                rankHistogram[i]++;
            }

            bool isStraight = (4 == (maxRank - minRank));

            // TODO: process histogram

             // (not an HRESULT...ha!)
            HandEvalResult hr = new HandEvalResult();
            return hr;
        }


    }
}