namespace CanonicalForm
{
    public class Operator
    {
        public Operator(char symbol, int precedence, char associativity)
        {
            Symbol = symbol;
            Precedence = precedence;
            Associativity = associativity;
        }
        public char Symbol { get; set; }
        public int Precedence { get; set; }
        // right or left
        public char Associativity { get; set; }
    }

    public class Bracket
    {
        public Bracket(char opening, char closing)
        {
            Opening = opening;
            Closing = closing;
        }
        public char Opening { get; set; }
        public char Closing { get; set; }
    }
}
