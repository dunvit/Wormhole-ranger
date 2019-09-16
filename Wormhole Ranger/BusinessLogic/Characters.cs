using System.Collections.Generic;

namespace Wormhole_Ranger.BusinessLogic
{
    public class Characters
    {
        List<Character> characters = new List<Character>();

        public int Count
        {
            get
            {
                return characters.Count;
            }
        }

        public List<Character> List
        {
            get
            {
                return characters;
            }
        }

        public void Add(Character character)
        {
            characters.Add(character);
        }
    }
}
