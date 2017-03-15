using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace pokerutils
{
    // C# List is internally an array construct, analagous to std::vector<> (vs a linked-list data structure)
    //
    // note: at this point the Hand class is really just an abstraction so we can swap container types
    public class Hand : List<Card>
    {
        public Hand(int numCards) : base(numCards)
        {
        }

        // handy for tests
        public Hand(string[] cardStringArray)
        {
            foreach (string s in cardStringArray)
                this.Add(new Card(s));
        }

        public Hand(string jsonHand)
        {
            string[] cardStringArray = ParseJsonStringArray(jsonHand);
            foreach (string s in cardStringArray)
                this.Add(new Card(s));
        }

        private static string[] ParseJsonStringArray(string jsonStringArray)
        {
            // TODO: brainstorm additional bad input we should be handling

            if(String.IsNullOrEmpty(jsonStringArray))
                throw new ArgumentNullException("jsonStringArray", "Error: null || empty input string");

            string[] retStringArray;

            try
            {
                retStringArray = JsonConvert.DeserializeObject<string[]>(jsonStringArray);
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error Parsing Input: {0}", jsonStringArray));
                Console.WriteLine(e.Message);
                throw;
            }

            return retStringArray;
        }


        // lazy init
        private string _displayString;
        public override string ToString()
        {
            //
            // StringBuilder allows for fast concat without realloc drama. Strings are immutable in C#.
            // the "+ 4" allows for 4 tens worst case without realloc.  really this is OCD as the default
            // buffer size is much larger, but this a demonstration of ability to think/code right?  {smile}
            //
            // Were the buffer sizes larger it (w||c)ould matter...
            //
            if (String.IsNullOrEmpty(_displayString))
            {
                StringBuilder sb = new StringBuilder((this.Count * 2) + 4);
                foreach (Card c in this)
                {
                    sb.Append(c.ToString());
                    sb.Append(" ");
                }
                _displayString = sb.ToString();
            }

            return _displayString;
        }
    }
}