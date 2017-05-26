using CanonicalFormExceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CanonicalForm
{
    public class Parser
    {
        // Transforms a given file line by line into canonical form
        // Output is the same file name with a ".out" extension
        public string TransformFileToCanonical(string inputFilename)
        {
            string outputExtention = ".out";
            string outputFileName = inputFilename + outputExtention;
            string inputLine;
            string outputLine;

            using (StreamReader inputStream = new StreamReader(inputFilename))
            {
                using (StreamWriter outputStream = new StreamWriter(outputFileName))
                {
                    while ((inputLine = inputStream.ReadLine()) != null)
                    {
                        try
                        {
                            outputLine = TransformEquationToCanonical(inputLine);
                            outputStream.WriteLine(outputLine);
                        }
                        catch (InvalidEquationException)
                        {
                            outputStream.WriteLine("INVALID INPUT");
                        }
                    }
                }
            }
            return outputFileName;
        }

        // Transforms an equation into canonical form
        public String TransformEquationToCanonical(string equation)
        {
            if (equation == "")
            {
                return "";
            }
            // Check for a valid equation before doing any transformations
            if (!IsValidEquation(equation))
            {
                throw new InvalidEquationException();
            }

            string[] equationSides = equation.Split('=');
            // Create tokens on each side of the equation
            List<Token> leftTokensInfix = new Tokenizer().Tokenize(equationSides[0]);
            List<Token> rightTokensInfix = new Tokenizer().Tokenize(equationSides[1]);
            // Move the right hand side of the equation to the left
            leftTokensInfix.AddRange(negateExpression(rightTokensInfix));
            // Turn it into postfix and define precedence
            List<Token> tokensPostfix = new PostfixConverter().InfixToPostfix(leftTokensInfix);
            // Simplify the resulting expression
            List<SimplifiedSummand> canonicalList = ParsePostfix(tokensPostfix);
            // Build the canonical equation as a string
            String canonicalString = CanonicalToString(canonicalList);
            canonicalString = canonicalString + " = 0";
            return canonicalString;
        }

        // Output a string representation of a list of simplified summands
        private string CanonicalToString(List<SimplifiedSummand> list)
        {
            StringBuilder sb = new StringBuilder();
            if (list.Count > 0)
            {
                sb.Append(list[0].ToString());
            }
            for(int i = 1; i < list.Count; i++)
            {
                SimplifiedSummand s = list[i];
                if (s.Coefficient < 0)
                {
                    sb.Append(" - ").Append(s.ToString().Substring(1));
                }
                else if (s.Coefficient > 0)
                {
                    sb.Append(" + ").Append(s.ToString());
                }
                // Summand has a coefficient of 0 so we don't need to add it
                else
                {
                    continue;
                } 
            }
            return sb.ToString();
        }

        private static List<String> TokenListToStringList(List<Token> tokens)
        {
            List<String> list = new List<String>();
            foreach (Token token in tokens)
            {
                list.Add(token.Identifier);
            }
            return list;
        }

        private static List<Token> negateExpression(List<Token> tokens)
        {
            List<Token> newTokens = new List<Token>(tokens);
            newTokens.Insert(0, new Bracket('(', true));
            newTokens.Insert(0, new Operator('-'));
            newTokens.Add(new Bracket(')', false));
            return newTokens;
        }

        // Simplifies a list of tokens in postfix form by applying applicable operators on summands.
        private List<SimplifiedSummand> ParsePostfix(List<Token> tokens)
        {
            SimplifiedSummandFactory ssf = null;
            Stack<List<SimplifiedSummand>> resultStack = new Stack<List<SimplifiedSummand>>();
            foreach (Token token in tokens)
            {
                if (token is Operator)
                {
                    List<SimplifiedSummand> result1 = null;
                    List<SimplifiedSummand> result2 = null;
                    Operator oper = (Operator)token;

                    if (resultStack.Count > 0)
                    {
                        result2 = resultStack.Pop();
                    }
                    // Special case in which equation is lead by a neagtive sign
                    if (resultStack.Count == 0 && oper.Symbol == '-')
                    {
                        ssf = new SimplifiedSummandFactory();
                        result2.ForEach(item => item.Negate());
                        resultStack.Push(result2);
                        continue;
                    }
                    if (resultStack.Count > 0)
                    {
                        result1 = resultStack.Pop();
                    }
                    if (result1 == null)
                    {
                        resultStack.Push(result2);
                    }
                    else if (result2 == null)
                    {
                        resultStack.Push(result1);
                    }
                    else
                    {
                        if (oper.Symbol == '*')
                        {
                            resultStack.Push(Multiply(result1, result2));
                        }
                        else if (oper.Symbol == '+')
                        {
                            resultStack.Push(Add(result1, result2));
                        }
                        else if (oper.Symbol == '-')
                        {
                            resultStack.Push(Subtract(result1, result2));
                        }
                    }
                }
                else
                {
                    ssf = new SimplifiedSummandFactory();
                    resultStack.Push(new List<SimplifiedSummand> { ssf.CreateSimplifiedSummand(token) });
                }
            }
            return CreateList(resultStack);
        }

        // Simplifies a list by expanding each variable back onto itself
        private static List<SimplifiedSummand> Consolidate(List<SimplifiedSummand> expression)
        {
            bool found;
            if (expression.Count <= 1)
            {
                return expression;
            }
            List<SimplifiedSummand> newResult = new List<SimplifiedSummand>(expression);
            List<SimplifiedSummand> unsuccessAdd = new List<SimplifiedSummand>();

            while (newResult.Count > 0)
            {
                SimplifiedSummand front = newResult[0];
                newResult.RemoveAt(0);
                found = false;
                for (int j = newResult.Count - 1; j >= 0; j--)
                {
                    try
                    {
                        newResult[j] = newResult[j].Add(front);
                        found = true;
                        break;
                    }
                    catch (InvalidSummandOperationException)
                    {
                    }
                }
                // Couldn't find a suitable operand to add to
                if (!found)
                {
                    unsuccessAdd.Add(front);
                }
            }
            newResult.AddRange(unsuccessAdd);
            return newResult;
        }

        // Creates a list of SimplifiedSummands out of a stack
        private static List<SimplifiedSummand> CreateList(Stack<List<SimplifiedSummand>> resultStack)
        {
            List<SimplifiedSummand> canonicalList = new List<SimplifiedSummand>();
            List<SimplifiedSummand> operands;
            while (resultStack.Count > 0)
            {
                operands = resultStack.Pop();
                canonicalList.AddRange(operands);
            }

            return canonicalList;
        }

        // Simplifies two lists if any summands of one can be added with another.
        private static List<SimplifiedSummand> Add(List<SimplifiedSummand> expression1, List<SimplifiedSummand> expression2)
        {
            List<SimplifiedSummand> newResult = new List<SimplifiedSummand>(expression1);
            for (int i = expression2.Count - 1; i >= 0; i--)
            {
                newResult = Add(newResult, expression2[i]);
            }
            return newResult;
        }

        // Adds a summand on to a list of summands if possible
        private static List<SimplifiedSummand> Add(List<SimplifiedSummand> expression1, SimplifiedSummand expression2)
        {
            List<SimplifiedSummand> newResult = new List<SimplifiedSummand>(expression1);
            for (int i = newResult.Count - 1; i >= 0; i--)
            {
                try
                {
                    newResult[i] = expression1[i].Add(expression2);
                    return newResult;
                }
                catch (InvalidSummandOperationException)
                {
                }
            }
            // Couldn't find a suitable operand to add to
            newResult.Add(expression2);
            return newResult;
        }

        // Simplifies two lists if any summands of one can be subtracted with another.
        private static List<SimplifiedSummand> Subtract(List<SimplifiedSummand> expression1, List<SimplifiedSummand> expression2)
        {
            List<SimplifiedSummand> newResult = new List<SimplifiedSummand>(expression1);
            for (int i = expression2.Count - 1; i >= 0; i--)
            {
                newResult = Subtract(newResult, expression2[i]);
            }
            return newResult;
        }

        // Subtracts a summand from a list of summands if possible
        private static List<SimplifiedSummand> Subtract(List<SimplifiedSummand> expression1, SimplifiedSummand expression2)
        {
            List<SimplifiedSummand> newResult = new List<SimplifiedSummand>(expression1);
            for (int i = newResult.Count - 1; i >= 0; i--)
            {
                try
                {
                    newResult[i] = expression1[i].Subtract(expression2);
                    return newResult;
                }
                catch (InvalidSummandOperationException)
                {
                }
            }
            // Couldn't find a suitable operand to subtract from
            newResult.Add(expression2.Negate());
            return newResult;
        }

        // Expands two lists of summands and simplifies them
        private static List<SimplifiedSummand> Multiply(List<SimplifiedSummand> expression1, List<SimplifiedSummand> expression2)
        {
            List<SimplifiedSummand> newResult = new List<SimplifiedSummand>();
            foreach (SimplifiedSummand operand1 in expression1)
            {
                foreach (SimplifiedSummand operand2 in expression2)
                {
                    newResult.Add(operand1.Multiply(operand2));
                }
            }
            return Consolidate(newResult);
        }

        // Further input error should be added for valid expressions
        // TODO:    check only 1 '=' sign
        //          check 2 operators not next to each other and is followed with summands
        //          check input is a number, a valid operator, or a letter
        private static bool IsValidEquation(string s)
        {
            int equalSignCount = s.Split('=').Length - 1;
            if (equalSignCount != 1)
            {
                return false;
            }
            return true;
        }

    }
}