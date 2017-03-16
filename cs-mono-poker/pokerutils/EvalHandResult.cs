using System;
using System.ComponentModel;
using System.Text;

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
                    sb.Append("High Card, Kickers: ");
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
}