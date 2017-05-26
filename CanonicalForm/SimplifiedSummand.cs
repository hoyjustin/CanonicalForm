using CanonicalFormExceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CanonicalForm
{
    // Used to represent the most simplified summand composed of 1 coefficient with no repeating variables
    public class SimplifiedSummand
    {
        public float Coefficient { get; set; }
        // Saves a record on the occurance of each unique factor. Factors with 0 occurances are not saved.
        SortedDictionary<char, int> VariableOccurances { get; set; }

        public SimplifiedSummand(float coefficient, SortedDictionary<char, int> variableOccurances)
        {
            Coefficient = coefficient;
            VariableOccurances = variableOccurances;
        }

        // A constant can be represented as where the list is empty and the coefficient is the constant
        public SimplifiedSummand(Number number)
        {
            Coefficient = float.Parse(number.Identifier);
            VariableOccurances = new SortedDictionary<char, int> { };
        }

        // Simplify a summand by adding up all its variable occurances and multiplying coefficients
        public SimplifiedSummand(Summand summand)
        {
            Coefficient = 1f;
            VariableOccurances = new SortedDictionary<char, int>();
            foreach (Factor factor in summand.Factors)
            {
                Coefficient = Coefficient * factor.Coefficient;

                char variable = factor.Variable;
                if (VariableOccurances.ContainsKey(variable))
                {
                    VariableOccurances[variable] = VariableOccurances[variable] + factor.Exponent;
                }
                else
                {
                    VariableOccurances[variable] = factor.Exponent;
                }
            }
        }

        // Coefficients of powers are added if they have the same base and exponent: 2a^m + 3a^m = 5a^m
        // Otherwise this throws an InvalidOperationException
        public SimplifiedSummand Add(SimplifiedSummand addend)
        {
            if (VariableOccurances.SequenceEqual(addend.VariableOccurances))
            {
                return new SimplifiedSummand(this.Coefficient + addend.Coefficient, this.VariableOccurances);
            }
            throw new InvalidSummandOperationException();
        }

        // Coefficients of powers are subtracted if they have the same base and exponent: 2a^m - 3a^m = -1a^m
        // Otherwise this throws an InvalidOperationException
        public SimplifiedSummand Subtract(SimplifiedSummand subtrahend)
        {
            if (VariableOccurances.SequenceEqual(subtrahend.VariableOccurances))
            {
                return new SimplifiedSummand(this.Coefficient - subtrahend.Coefficient, this.VariableOccurances);
            }
            throw new InvalidSummandOperationException();
        }

        // Powers are simplified using the Product Rule if they have the same base: a^m ∙ a^n = a^(m + n)
        // Otherwise, add a new variable to the operand.
        public SimplifiedSummand Multiply(SimplifiedSummand multiplier)
        {
            float newCoefficient = this.Coefficient * multiplier.Coefficient;
            SimplifiedSummand newOperand = new SimplifiedSummand(newCoefficient, new SortedDictionary<char, int>(multiplier.VariableOccurances));
            List<char> originalKeys = new List<char>(VariableOccurances.Keys);
            foreach (char key in originalKeys)
            {
                // Check if operands have matching bases
                int occurance;
                if (newOperand.VariableOccurances.TryGetValue(key, out occurance))
                {
                    newOperand.VariableOccurances[key] = occurance + this.VariableOccurances[key];
                }
                else
                {
                    newOperand.VariableOccurances[key] = this.VariableOccurances[key];
                }
            }
            return newOperand;
        }

        // Switches the sign on the coefficient
        public SimplifiedSummand Negate()
        {
            return new SimplifiedSummand(this.Coefficient * -1f, VariableOccurances);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<string> strings = new List<string>();
            if (Coefficient == 0)
            {
                return "0";
            }

            // Summand is a constant if it has no keys
            if (VariableOccurances.Keys.Count == 0)
            {
                return Coefficient.ToString();
            }
            else
            {
                if (Coefficient != 1 && Coefficient != -1)
                {
                    sb.Append(Coefficient.ToString());
                }
                else if (Coefficient == -1)
                {
                    sb.Append("-");
                }

                foreach (char key in VariableOccurances.Keys)
                {
                    float exponent = VariableOccurances[key];
                    if (exponent == 0)
                    {
                        continue;
                    }
                    else if (exponent == 1)
                    {
                        sb.Append(key);
                    }
                    else
                    {
                        sb.Append(key);
                        sb.Append("^");
                        sb.Append(exponent);
                    }
                }
            }
            return sb.ToString();
        }
    }

    // Creates a suitable number or summand from a given token
    public class SimplifiedSummandFactory
    {
        public SimplifiedSummand CreateSimplifiedSummand(Token token)
        {
            SimplifiedSummand operand = null;
            if (token is Number)
            {
                operand = new SimplifiedSummand((Number)token);
            }
            else if (token is Summand)
            {
                operand = new SimplifiedSummand((Summand)token);
            }
            return operand;
        }
    }
}