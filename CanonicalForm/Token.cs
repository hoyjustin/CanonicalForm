using System.Collections.Generic;
using System.Text;

namespace CanonicalForm
{
    public abstract class Token
    {
        public string Identifier { get; set; }
    }

    public class Number : Token
    {
        public Number(string number)
        {
            Identifier = number;
        }
    }

    public class Operator : Token
    {
        char _symbol;

        public Operator(char op)
        {
            Identifier = op.ToString();
            _symbol = op;
        }

        public char Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                Identifier = Symbol.ToString();
            }
        }
    }

    public class Bracket : Token
    {
        public bool IsOpening { get; set; }

        public Bracket(char bracket, bool isOpening)
        {
            Identifier = bracket.ToString();
            IsOpening = isOpening;
        }
    }

    // Representative of a single variable and its exponent if it has one
    public class Factor
    {
        public float Coefficient { get; set; }
        public char Variable { get; set; }
        public int Exponent { get; set; }

        public Factor(float coefficient, char variable, int exponent)
        {
            Coefficient = coefficient;
            Variable = variable;
            Exponent = exponent;
        }

        public override string ToString()
        {
            string s;
            if (Exponent == 0)
            {
                s = "1";
            }
            else if (Exponent == 1)
            {
                s = Variable.ToString();
            }
            else
            {
                s = Variable.ToString() + "^" + Exponent.ToString();
            }
            return s;
        }
    }

    // Consists of many variable and their exponenents
    public class Summand : Token
    {
        private List<Factor> _factors;

        public Summand(List<Factor> factors)
        {
            _factors = factors;
            GenerateIdentifier();
        }

        private void GenerateIdentifier()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Factor v in _factors)
            {
                float coefficient = v.Coefficient;
                if (coefficient != 1)
                {
                    sb.Append(v.Coefficient.ToString());
                }
                sb.Append(v.Variable);
                if (v.Exponent != 1)
                {
                    sb.Append("^");
                    sb.Append(v.Exponent);
                }
            }
            Identifier = sb.ToString();
        }

        public List<Factor> Factors
        {
            get { return _factors; }
            set
            {
                _factors = value;
                GenerateIdentifier();
            }
        }

        public void AddFactor(Factor v)
        {
            _factors.Add(v);
            GenerateIdentifier();
        }

        public void SetLastFactor(Factor newFactor)
        {
            Factor previousFactor = _factors[Factors.Count - 1];
            _factors.RemoveAt(_factors.Count - 1);
            AddFactor(newFactor);
            GenerateIdentifier();
        }
    }

    // Creates a suitable token for a given character
    public class TokenFactory
    {
        public Token CreateToken(char c)
        {
            Token token = null;
            if (Definitions.IsPartOfANumber(c))
            {
                token = new Number(c.ToString());
            }
            else if (Definitions.IsOperator(c))
            {
                token = new Operator(c);
            }
            else if (Definitions.IsBracket(c))
            {
                token = new Bracket(c, Definitions.IsOpeningBracket(c));
            }
            else
            {
                token = new Summand(new List<Factor> { new Factor(1, c, 1) });
            }
            return token;
        }
    }
}