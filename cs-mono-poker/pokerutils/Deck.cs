using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace pokerutils
{
    public class Deck
    {
        private Card[] _deck;
        private const int _numCardsInDeck = 52;

        //
        // [0,51], starts at 51
        //
        private int _dealPosition;

        private void InitDeck()
        {
            _deck= new Card[_numCardsInDeck];
            _dealPosition = _numCardsInDeck - 1;

            var numCardsCreated = 0;

            // reflection is a fun language feature.
            foreach (Suits s in Enum.GetValues(typeof(Suits)))
            {
                foreach (Ranks r in Enum.GetValues(typeof(Ranks)))
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
            if(_dealPosition == _numCardsInDeck)
                throw new ArgumentOutOfRangeException("ERROR: deck is already full");
            _dealPosition++;
            _deck[_dealPosition] = c;
        }

        public Deck()
        {
            InitDeck();
        }

        // Fisher-Yates is o(n) using swap + vector
        public void Shuffle()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());

            Debug.Assert(_deck.Length == _numCardsInDeck);
            int m = _deck.Length;

            while (m > 0)
            {
                // choose random card from what's left ( [0-m] ), advance m --> 0, swap
                int idx = random.Next() % m--;
                Card tmpCard = _deck[m];
                _deck[m] = _deck[idx];
                _deck[idx] = tmpCard;
            }
        }
        /*
        public void NaiveShuffle()
        {
            // this is lame, is not in place (e.g. swaps) and involves a good deal of list traversal.
            // there are a few better ways to do this.  that said, we're only ever shuffling 52 cards, not a large #

            // note: there's probably a faster array based approach that doesn't involve the traversals.  that said,
            //       traversals are fast, and there's 52 cards in a deck...
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

                // bad: list traversal
                _cards.Remove(cardToMove);
            }


            Debug.Assert(0 == _cards.Count);
            Debug.Assert(originalSize == shuffledDeck.Count);

            _cards = shuffledDeck;
        }
        */

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

        public Hand Deal(int numCards = 5)
        {
            if (numCards <= 0)
                throw new ArgumentException("cannot deal zero negative cards");

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
    }
}