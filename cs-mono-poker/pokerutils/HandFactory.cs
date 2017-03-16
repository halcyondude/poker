using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace pokerutils
{

    public class HandFactory
    {

        public static List<Hand> CreateAllPossibleHands(string jsonAllCards, int nCardsInEachHand = 5)
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
        // (n,k) --> n! / (k! * (n   -k)!)
        //
        // For example, if we're dealing 8 cards (n), and we want to generate all unique 5 card (k) hands...
        //
        // (7,5)  --> 7! / (5! * 2!)   --> 5040/240  --> 21 Hands
        // (8,5)  --> 8! / (5! * (3)!) --> 40320/720 --> 56 Hands
        // (10,5) -->       252
        // (13,5) -->     1,287
        // (26,5) -->    65,780
        // (52,5) --> 2,598,960 :)
        //
        public static List<Hand> CreateAllPossibleHands(Hand hand, int nCardsInEachHand = 5)
        {
            if(nCardsInEachHand > hand.Count)
                throw new ArgumentOutOfRangeException("hand", String.Format("Error: nCardsInHand is greater than the total number of cards"));

            // TODO: put some guards in place for nCardsInEachHand.  For now trust.

            Console.WriteLine("Generating all possible {0} card hands of {1}", nCardsInEachHand, hand);
            // reasonable default for most poker variants
            List<Hand> allPossibleHands = new List<Hand>(64);

            //Console.WriteLine("DEBUG: Calling AlgoNChooseK({0}, {1})", hand, nCardsInEachHand);

            // generate all cards
            Card[] cardArray = hand.ToArray();
            bool[] boolArrayCardsToUse = new bool[hand.Count];
            AlgoNChooseK(cardArray, nCardsInEachHand, 0,0,boolArrayCardsToUse, ref allPossibleHands);

            return allPossibleHands;

        }

        // note to reviewers, full disclosure:
        //
        // after pretending it was the 90's, and attempting to derive this algo (because it was fun to try)
        // I adapted the code below from here:
        //
        // - http://algorithms.tutorialhorizon.com/print-all-combinations-of-subset-of-size-k-from-given-array
        //
        // by porting to C# and making it a bit more digestible, and making it specific to poker.
        // I particularly found the visualizion of the algo to be worthwhile.
        //
        // This is n-choose-k, based on pascal's triangle.  I found this 4 min video to be also be useful if the reader
        // isn't sure what's going on w.r.t. the recursion tree, or what binomial coeffecients are.
        //
        // - https://www.youtube.com/watch?v=Hmld7MhFUDk&t=19s
        //
        public static void AlgoNChooseK(Card[] inputArray, int k, int startIndex, int currLen, bool[] used, ref List<Hand> accumulatedHands)
        {
            if (currLen == k)
            {
                Hand newHand = new Hand(k);
                for (int i = 0; i < inputArray.Length; i++)
                {
                    if (used[i] == true)
                    {
                        Card c = inputArray[i];
                        newHand.Add(new Card(c.suit, c.rank));
                    }
                }

                accumulatedHands.Add(newHand);
                //Console.WriteLine("new hand --> {0}", newHand);
                return;
            }

            if (startIndex == inputArray.Length)
            {
                return;
            }

            // For every index we have two options,
            // 1.. Either we select it, means put true in used[] and make currLen+1
            used[startIndex] = true;
            AlgoNChooseK(inputArray, k, startIndex + 1, currLen + 1, used, ref accumulatedHands);

            // 2.. OR we dont select it, means put false in used[] and dont increase
            // currLen
            used[startIndex] = false;
            AlgoNChooseK(inputArray, k, startIndex + 1, currLen, used, ref accumulatedHands);
        }


        #region learning

        public void Subset(int[] A, int k, int start, int currLen, bool[] used)
        {
            if (currLen == k)
            {
                for (int i = 0; i < A.Length; i++)
                {
                    if (used[i] == true)
                    {
                        Console.Write("{0} ", A[i].ToString());
                    }
                }
                Console.Write(Environment.NewLine);
                return;
            }
            if (start == A.Length)
            {
                return;
            }
            // For every index we have two options,
            // 1.. Either we select it, means put true in used[] and make currLen+1
            used[start] = true;
            Subset(A, k, start + 1, currLen + 1, used);
            // 2.. OR we dont select it, means put false in used[] and dont increase
            // currLen
            used[start] = false;
            Subset(A, k, start + 1, currLen, used);
        }

        public void SubsetInternalTest()
        {
            int[] A = {1, 2, 3, 4, 5, 6, 7};
            bool[] B = new bool[A.Length];
            Subset(A, 5, 0, 0, B);

        }

        #endregion

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

} // end namespace