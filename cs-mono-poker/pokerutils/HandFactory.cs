using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace pokerutils
{
    public class HandFactory
    {

        public static Hand[] CreateAllPossibleHands(string jsonAllCards, int nCardsInEachHand)
        {
            Hand hand = new Hand(jsonAllCards);
            return CreateAllPossibleHands(hand, nCardsInEachHand);
        }

        //
        // n-choose-k (e.g. binomial coeffecient)
        //
        // n := set of cards in hand to choose from
        // k := nCardsInEachHand
        //
        // This function will generate all combinations of size k, given n source cards.
        // The core recursive algorithm is inspired by Pascal's triangle, and will generate the following
        // number of combinations:
        //
        // (n,k) --> n! / (k! * (n-k)!)
        //
        // For example, if we're dealing 8 cards (n), and we want to generate all unique 5 card (k) hands...
        //
        // (8,5) --> 8! / (5! * (3)!) --> 40320/720 --> 56 Hands
        //
        public static Hand[] CreateAllPossibleHands(Hand hand, int nCardsInEachHand)
        {


            if(nCardsInEachHand > hand.Count)
                throw new ArgumentOutOfRangeException("hand", String.Format("Error: nCardsInHand is greater than the total number of cards"));

            // TODO: complete part 3 (FindUniqueHands())
            return new Hand[1];

        }

        /*
        // cards - the set of cards (n)
        //
        private static Hand[] FindUniqueHands(Card[] allCards, int nCardsInEachHand, int allCardsIndex = 0, Hand data)
        {
            // left branch of recursion tree (ignore it)
            //FindUniqueHands(allCards, nCardsInEachHand, allCardsIndex + 1);

            // right branch of recursion tree (use it)
            //FindUniqueHands(allCards, nCardsInEachHand, allCardsIndex + 1);

        }
*/




    }
}