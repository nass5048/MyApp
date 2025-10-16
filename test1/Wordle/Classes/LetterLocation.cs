using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser
{
    public class LetterLocation
    {
        public char letter;
        public int location;

        public LetterLocation(char letter, int location)
        {
            this.letter = letter;
            this.location = location;
        }
    }

    public class CharacterComparer : IEqualityComparer<LetterLocation>
    {
        public bool Equals(LetterLocation x, LetterLocation y)
        {
            // Define your custom equality logic here.
            // For example, compare based on a specific property:
            return x.location == y.location && x.letter == y.letter;
        }

        public int GetHashCode(LetterLocation obj)
        {
            // Generate a hash code based on the properties used in Equals.
            // A common way is to combine hash codes of relevant properties:
            return obj == null ? 0 : obj.location;
        }
    }
}
