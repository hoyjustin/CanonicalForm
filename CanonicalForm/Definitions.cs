using System;
using System.Collections.Generic;

namespace CanonicalForm
{
    public static class Definitions
    {
        static char[] openingBrackets = new char[] { '[', '(', '{' };
        static char[] closingBrackets = new char[] { ']', ')', '}' };

        static Dictionary<char, Tuple<int, char>> operatorsDictionary = new Dictionary<char, Tuple<int, char>>()
        {
            { '+', Tuple.Create(2, 'l')},
            { '-', Tuple.Create(2, 'l')},
            { '*', Tuple.Create(3, 'l')}
        };

        // Check if the operator can be applied to 2 summands
        public static bool IsOperator(char c)
        {
            char[] operators = new char[] { '+', '-', '*' };
            return (Array.IndexOf(operators, c) != -1) ? true : false;
        }

        public static bool IsExponent(char c)
        {
            return (c == '^') ? true : false;
        }

        public static bool IsPartOfANumber(char c)
        {
            if (Char.IsDigit(c) || c == '.' || c == ',')
            {
                return true;
            }
            return false;
        }

        public static bool IsBracket(char c)
        {
            return (IsOpeningBracket(c) || IsClosingBracket(c)) ? true : false;
        }

        public static bool IsOpeningBracket(char c)
        {
            return (Array.IndexOf(openingBrackets, c) != -1) ? true : false;
        }
        
        public static bool IsClosingBracket(char c)
        {
            return (Array.IndexOf(closingBrackets, c) != -1) ? true : false;
        }

        public static bool IsVariable(char c)
        {
            return (char.IsLetter(c)) ? true : false;
        }

        public static int GetPrecedence(char c)
        {
            Tuple<int, char> tuple;
            operatorsDictionary.TryGetValue(c, out tuple);
            return tuple.Item1;
        }

        public static char GetAssociativity(char c)
        {
            Tuple<int, char> tuple;
            operatorsDictionary.TryGetValue(c, out tuple);
            return tuple.Item2;
        }

        public static char GetPartnerBracket(char c)
        {
            char bracket = 'a';
            if (IsClosingBracket(c))
            {
                int index = Array.IndexOf(closingBrackets, c);
                bracket = openingBrackets[index];
            }
            else if (IsOpeningBracket(c))
            {
                int index = Array.IndexOf(openingBrackets, c);
                bracket = closingBrackets[index];
            }
            return bracket;
        }

    }
}