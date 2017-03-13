using System;
using System.Collections.Generic;
using System.Text;

namespace pokerutils
{
    // C# List is internally an array construct, analagous to std::vector<> (vs a linked-list data structure)
    //
    // note: at this point the Hand class is really just an abstraction so we can swap container types
    public class Hand : List<Card>
    {
        private Hand()
        {
        }

        // default List<> for most .Net versions is capacity 0, then 4, then double on realloc.  Paying for 2x realloc
        // and yielding 8 (with 3 wasted) PER HAND is silly.  We might still end up with wasted bytes depending on
        // Mono vs .Net Core vs Windoze impl's, but at least this is being specific.
        public Hand(int defaultHandSize = 5) : base(defaultHandSize)
        {
        }

        public override string ToString()
        {
            // StringBuilder allows for fast concat without realloc drama since strings are immutable in C#.
            // the "+4" allows for 4 tens worst case without realloc.  really this is OCD as the default
            // buffer size is much larger, but this a demonstration of ability to think/code right?  {smile}
            //
            // Were the buffer sizes larger it would matter :)
            StringBuilder sb = new StringBuilder((this.Count * 2) + 4);
            foreach (Card c in this)
            {
                sb.Append(c.ToString());
            }
            return sb.ToString();
        }
    }
}