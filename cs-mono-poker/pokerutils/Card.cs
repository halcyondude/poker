using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;

namespace pokerutils
{
    // note: indexing these zero-based in case we need array indices later, and for rank, comparison/less predicate
    public enum Suits
    {
        HEART   = 0,
        DIAMOND = 1,
        SPADE   = 2,
        CLUB    = 3
    };

    public enum Ranks
    {
        TWO   = 0,
        THREE = 1,
        FOUR  = 2,
        FIVE  = 3,
        SIX   = 4,
        SEVEN = 5,
        EIGHT = 6,
        NINE  = 7,
        TEN   = 8,
        JACK  = 9,
        QUEEN = 10,
        KING  = 11,
        ACE   = 12
    }

    public class Card
    {

        private SuitInfo _suitInfo;
        private RankInfo _rankInfo;

        // the single char glyph from unicode 6.0
        private char _cardUnicodeDisplay; // TODO: not working yet (on Mono at least)

        // public properties
        public Suits suit { get; private set; }
        public Ranks rank { get; private set; }

        public string PrettySuit { get; private set; }
        public string PrettyRank { get; private set; }

        // disallow default ctor
        private Card()
        {
        }

        public Card(string inputRankSuit)
        {
            try
            {
                ParseInputValue(inputRankSuit);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException);
                throw;
            }

            PopulatePrettyInfo();
        }

        public Card(Suits suitIn, Ranks rankIn)
        {
            suit = suitIn;
            rank = rankIn;

            PopulatePrettyInfo();
        }


        //
        // while there are peephole optimizations that could be done here (e.g. clever parsing out of 10, followed
        // by static ASCII array lookup table), using a container that logically models the contents has merit.  We're
        // generally parsing the cards from input once, then using them.  If we needed to support chomping massive amounts
        // of input to test variations might make sense to tweak this but not addressing presently.

        //
        // For example if we need to improve this to allow for concurrent/locked access
        // it's just a type swap.  Since the lookup dict's are static they will be init'd just once and share
        // memory with other instances of the card class
        //
        // If the requirements change (for input parsing) this makes it easy to maintain.  Clear > Clever
        //
        // kvp lookups for Dictionary<> are hash table fast, O(1) - O(n) (worst)
        //
        private void ParseInputValue(string inputString)
        {
            /*
                valid input is "{rank}{suit}"
                Rank := {2,3,4,5,6,7,8,9,10,J,Q,K,A}
                Suit := {H,D,S,C}
             */

            // input string MUST be 2 or 3 (if 10) long, or is invalid
            if ((inputString.Length != 2) && (inputString.Length != 3))
                throw new ArgumentException("Invalid Input String: input is not 2 or 3 characters", inputString);

            inputString = inputString.ToUpper(CultureInfo.InvariantCulture);

            var inputSuit = inputString.Substring(inputString.Length - 1);
            var inputRank = inputString.Substring(0, inputString.Length - 1);

            // this is silly, but properties can't be used as out or retval
            Suits s;
            Ranks r;

            if(!_dictInputToSuit.TryGetValue(inputSuit, out s))
                throw new ArgumentException(String.Format("Invalid Input String: Unrecognized Suit ({0})", inputSuit), inputString);

            if(!_dictInputToRank.TryGetValue(inputRank, out r))
                throw new ArgumentException(String.Format("Invalid Input String: Unrecognized Rank ({0})", inputRank), inputString);

            suit = s;
            rank = r;
        }

        private void PopulatePrettyInfo()
        {
            // assumes inputs already validated
            _suitInfo = _dictPrettySuit[suit];
            _rankInfo = _dictPrettyRank[rank];

            // BOO - Mono seems to have some issues with Unicode 6.0, dumping the actual char works
            // TODO: would be cool to have unicode cards for display purposes.
            uint unicodeCardValue = _suitInfo.UnicodeCardSuitBase | _rankInfo.UnicodeCardRankSuffix;
            //_cardUnicodeDisplay = (char)unicodeCardValue; // TODO: this throws exception at runtime, char out of bounds

            PrettyRank = _rankInfo.Display;
            PrettySuit = _suitInfo.Display;

        }


        public override string ToString()
        {
            // TODO wire in pretty unicode if we can get it sorted.
            // ♣A ♠K ♥Q ♦J
            return _suitInfo.UnicodeDarkSuit + _rankInfo.Display;
        }

        public string ToStringDebug()
        {
            return String.Format("{0} : {1} : {2} : {4}", _cardUnicodeDisplay,
                                                       _suitInfo.UnicodeDarkSuit, _suitInfo.UnicodeWhiteSuit,
                                                       _suitInfo.Display, _rankInfo.Display);
        }


        public string ToBoringString()
        {
            return String.Format("{0,7} : {1} ", suit, rank);
        }

    #region static_data
        //
        // making these public as we might need them later for printing elsewhere.
        //
        public static readonly Dictionary<string, Ranks> _dictInputToRank = new Dictionary<string, Ranks>
        {
            {"2", Ranks.TWO},
            {"3", Ranks.THREE},
            {"4", Ranks.FOUR},
            {"5", Ranks.FIVE},
            {"6", Ranks.SIX},
            {"7", Ranks.SEVEN},
            {"8", Ranks.EIGHT},
            {"9", Ranks.NINE},
            {"10", Ranks.TEN},
            {"J", Ranks.JACK},
            {"Q", Ranks.QUEEN},
            {"K", Ranks.KING},
            {"A", Ranks.ACE},
        };

        public static readonly Dictionary<string, Suits> _dictInputToSuit = new Dictionary<string, Suits>
        {
            {"H", Suits.HEART},
            {"D", Suits.DIAMOND},
            {"S", Suits.SPADE},
            {"C", Suits.CLUB}
        };


        //
        // for pretty printing: https://en.wikipedia.org/wiki/Playing_cards_in_Unicode
        //
        public struct SuitInfo
        {
            public SuitInfo(string display, char unicodeDarkSuit, char unicodeWhiteSuit, uint unicodeCardSuiteBase)
            {
                this.Display             = display;
                this.UnicodeDarkSuit     = unicodeDarkSuit;
                this.UnicodeWhiteSuit    = unicodeWhiteSuit;
                this.UnicodeCardSuitBase = unicodeCardSuiteBase;
            }

            public string Display;
            public char UnicodeDarkSuit;
            public char UnicodeWhiteSuit;
            public uint UnicodeCardSuitBase;
        };

        public static readonly Dictionary<Suits, SuitInfo> _dictPrettySuit = new Dictionary<Suits, SuitInfo>()
        {
            {Suits.SPADE,   new SuitInfo("S", '\u2660', '\u2664', 0x1F0A0)},
            {Suits.HEART,   new SuitInfo("H", '\u2665', '\u2661', 0x1F0B0)},
            {Suits.DIAMOND, new SuitInfo("D", '\u2666', '\u2662', 0x1F0C0)},
            {Suits.CLUB,    new SuitInfo("C", '\u2663', '\u2667', 0x1F0D0)},
        };

        public struct RankInfo
        {
            public RankInfo(string display, uint unicodeCardRankSuffix)
            {
                this.Display = display;
                this.UnicodeCardRankSuffix = unicodeCardRankSuffix;
            }

            public string Display;
            public uint UnicodeCardRankSuffix;
        };

        public static readonly Dictionary<Ranks, RankInfo> _dictPrettyRank = new Dictionary<Ranks, RankInfo>()
        {
            {Ranks.TWO,   new RankInfo("2", 0x2)},
            {Ranks.THREE, new RankInfo("3", 0x3)},
            {Ranks.FOUR,  new RankInfo("4", 0x4)},
            {Ranks.FIVE,  new RankInfo("5", 0x5)},
            {Ranks.SIX,   new RankInfo("6", 0x6)},
            {Ranks.SEVEN, new RankInfo("7", 0x7)},
            {Ranks.EIGHT, new RankInfo("8", 0x8)},
            {Ranks.NINE,  new RankInfo("9", 0x9)},
            {Ranks.TEN,   new RankInfo("10",0xA)},
            {Ranks.JACK,  new RankInfo("J", 0xB)},
            {Ranks.QUEEN, new RankInfo("Q", 0xC)},
            {Ranks.KING,  new RankInfo("K", 0xD)},
            {Ranks.ACE,   new RankInfo("A", 0xE)}
        };

#endregion


    }
}

