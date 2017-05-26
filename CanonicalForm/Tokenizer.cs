using CanonicalFormExceptions;
using System;
using System.Collections.Generic;

namespace CanonicalForm
{
    class Tokenizer
    {
        // Tokenizes on numbers, summands, brackets and (+, -, *) operators.
        // ie: Given an input such as 3 * (3.4x^2y  +  4.0x)
        // outputs a token list of 3, *, (, 3.4x^2y, +, 4.0x, )
        public List<Token> Tokenize(string line)
        {
            String noSpaceLine = line.Replace(" ", "");

            // Create and collect appropriate tokens based on the character
            TokenFactory tf = new TokenFactory();
            List<Token> tokens = new List<Token>();
            for (int i = 0; i < noSpaceLine.Length; i++)
            {
                char c = noSpaceLine[i];
                if (i == 0)
                {
                    if (c == '-')
                    {
                        tokens.Add(new Number(c.ToString()));
                    }
                    else
                    {
                        tokens.Add(tf.CreateToken(c));
                    }
                }
                else if (Definitions.IsPartOfANumber(c))
                {
                    Token previousToken = tokens[tokens.Count - 1];
                    // Append to the previous token if it was a number
                    if (previousToken is Number)
                    {
                        previousToken.Identifier = previousToken.Identifier + c;
                    }
                    else if (previousToken is Summand)
                    {
                        // Calculate summand exponents by taking numbers right of exponent sign
                        Number number = (Number)tf.CreateToken(c);
                        while (i + 1 < noSpaceLine.Length)
                        {
                            char c2 = noSpaceLine[i + 1];
                            if (Definitions.IsPartOfANumber(c2))
                            {
                                number.Identifier = number.Identifier + c2;
                                i++;
                            }
                            else if (Definitions.IsVariable(c2))
                            {
                                Factor factor = new Factor(float.Parse(number.Identifier), c2, 1);
                                ((Summand)previousToken).AddFactor(factor);
                                i++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        tokens.Add(tf.CreateToken(c));
                    }
                }
                else if (Definitions.IsOperator(c))
                {
                    Token previousToken = tokens[tokens.Count - 1];
                    if (previousToken is Operator)
                    {
                        throw new InvalidEquationException();
                    }
                    tokens.Add(tf.CreateToken(c));
                }
                else if (Definitions.IsBracket(c))
                {
                    tokens.Add(tf.CreateToken(c));
                }
                else if (Definitions.IsExponent(c))
                {
                    // Gather all digits in the exponent
                    String exponent = "";
                    while (i + 1 < noSpaceLine.Length)
                    {
                        char e = noSpaceLine[i + 1];
                        if (Definitions.IsPartOfANumber(e))
                        {
                            exponent = exponent + e;
                            Summand previousToken = (Summand)tokens[tokens.Count - 1];
                            List<Factor> previousFactors = previousToken.Factors;
                            Factor previousFactor = previousToken.Factors[previousToken.Factors.Count - 1];
                            Factor newFactor = new Factor(previousFactor.Coefficient, previousFactor.Variable, int.Parse(exponent));
                            previousToken.SetLastFactor(newFactor);
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                // Variable (Beginning of summand)
                else
                {
                    Token previousToken = tokens[tokens.Count - 1];
                    // Recycle last summand
                    if (previousToken is Summand)
                    {
                        ((Summand)previousToken).AddFactor(new Factor(1, c, 1));
                    }
                    // Make the previous token part of the summand if it was a number
                    else if (previousToken is Number)
                    {
                        Summand summand = (Summand)tf.CreateToken(c);
                        if (previousToken.Identifier == "-")
                        {
                            previousToken.Identifier = "-1";
                        }
                        Factor newFactor = new Factor(float.Parse(previousToken.Identifier), c, 1);
                        summand.SetLastFactor(newFactor);
                        tokens.RemoveAt(tokens.Count - 1);
                        tokens.Add(summand);
                    }
                    else
                    {
                        tokens.Add(tf.CreateToken(c));
                    }
                }
            }
            return tokens;
        }
    }
}
