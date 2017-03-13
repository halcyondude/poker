using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace pokerutils
{
    //
    // This is meant to provide deck semantics (deal/return to deck) to support potential for
    // games with mutiple hands, so we don't get stats messed up because every hand is merely
    // a random batch of cards.  If there's already three aces dealt to player1, player2 can't
    // have a pair of aces, etc.
    //
    public class Deck
    {
        private Card[] _deck;
        private int _numCardsInDeck;

        // [0,_numCardsInDeck-1], starts at _numCardsInDeck-1
        private int _dealPosition;

        public Deck()
        {
            InitDeck();
        }

        private void InitDeck()
        {
            var allSuitValues = Enum.GetValues(typeof(Suits));
            var allRankValues = Enum.GetValues(typeof(Ranks));

            _numCardsInDeck = allSuitValues.Length * allRankValues.Length;
            _deck= new Card[_numCardsInDeck];
            _dealPosition = _numCardsInDeck - 1;

            var numCardsCreated = 0;

            // reflection is a fun language feature.
            foreach (Suits s in allSuitValues)
            {
                foreach (Ranks r in allRankValues)
                {
                    Debug.Assert(numCardsCreated < _numCardsInDeck);

                    _deck[numCardsCreated] = new Card(s, r);
                    numCardsCreated++;
                }
            }
        }

        private Card dealCard()
        {
            if(_dealPosition < 0)
                throw new ArgumentOutOfRangeException("ERROR: cannot deal from an empty deck");

            Card c = _deck[_dealPosition];

            // when last card is dealt this will be -1
            _dealPosition--;
            return c;

        }

        private void returnCard(Card c)
        {
            if((_dealPosition + 1) == _numCardsInDeck)
                throw new ArgumentOutOfRangeException("ERROR: deck is already full");
            _dealPosition++;
            _deck[_dealPosition] = c;
        }

        // Fisher-Yates is o(n) using swap + vector
        public void Shuffle()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());

           int m = _dealPosition;

            while (m > 0)
            {
                // choose random card from what's left ( [0-m] ), advance m --> 0, swap
                int idx = random.Next() % m--;
                Card tmpCard = _deck[m];
                _deck[m] = _deck[idx];
                _deck[idx] = tmpCard;
            }
        }

        public Hand Deal(int numCards = 5)
        {
            if (numCards <= 0)
                throw new ArgumentException("cannot deal zero or negative cards");

            if(numCards > (1 + _dealPosition))
                throw new ArgumentOutOfRangeException(String.Format("Error: {0} cards requested, {1} exist in deck presently!", numCards, 1 + _dealPosition));

            Hand hand = new Hand();
            for (int i = 0; i < numCards; ++i)
            {
                hand.Add(dealCard());
            }

            return hand;
        }

        public void Return(Hand hand)
        {
            foreach (Card c in hand)
            {
                returnCard(c);
            }
        }

        public override string ToString()
        {
            return "TODO: add ToString impl to Deck";
        }

        #region defunctTodoRemoveme
        /*
        public void NaiveShuffle()
        {
            // this is lame, is not in place (e.g. swaps) and involves a good deal of list traversal.  It also causes
            // a new List<> to be created (not really expensive TBH) but is philosophically dirty.
            // there are better ways to do this.  that said, we're only ever shuffling 52 cards, not a large #
            // this was first approach, and is only here to show iteration on an idea.

            List<Card> shuffledDeck = new List<Card>();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int originalSize = _cards.Count;

            while (_cards.Count > 0)
            {
                // pick a random card from what's left
                int cIndex = random.Next() % (_cards.Count-1);

                // get a ref
                Card cardToMove = _cards[cIndex];

                shuffledDeck.Add(cardToMove);

                // bad: List<> is actually a managed array that can grow/shrink. (akin to C++ std::vector<>)
                // This makes holes that need to be fixed up to be contigous after remove.
                _cards.Remove(cardToMove);
            }


            Debug.Assert(0 == _cards.Count);
            Debug.Assert(originalSize == shuffledDeck.Count);

            _cards = shuffledDeck;
        }

        // TODO: should we deal List<> or simple arrays?
        public List<Card> DealList(int numCards = 5)
        {
            if (numCards <= 0)
                throw new ArgumentException("cannot deal zero negative cards");

            if(numCards > (1 + _dealPosition))
                throw new ArgumentOutOfRangeException(String.Format("Error: {0} cards requested, {1} exist in deck presently!", numCards, 1 + _dealPosition));

            List<Card> hand = new List<Card>();

            // while List<> (and generic collections broadly support "range" operations, keeping this simple/portable
            for (int i = 0; i < numCards; ++i)
                hand.Add(dealCard());

            return hand;
        }

        public void ReturnList(List<Card> handIn)
        {
            if (null == handIn)
                throw new ArgumentNullException("handIn is null");

            foreach (Card c in handIn)
            {
                returnCard(c);
            }
        }
        */
        #endregion


    }
}