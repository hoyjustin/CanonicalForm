using System.Collections.Generic;

namespace CanonicalForm
{
    class PostfixConverter
    {
        // Removes brackets and generates an output token list as a postfix expression using Dijkstra's Shunting-yard algorithm
        // Assumes an input of tokens representing an infix expression.
        public List<Token> InfixToPostfix(List<Token> infix)
        {
            List<Token> postfix = new List<Token>();
            Stack<Token> stack = new Stack<Token>();
            Token top;
            foreach (Token token in infix)
            {
                // Current token is an operator, push it to output
                if (token is Operator)
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(token);
                    }
                    else
                    {
                        while (stack.Count > 0)
                        {
                            top = stack.Peek();
                            if (!(top is Operator))
                            {
                                break;
                            }
                            // Pop the top off the stack, onto the output if
                            // 1. operator is right associative, and has precedence less than the top
                            // 2. operator is left associative and its precedence is less or equal to the top
                            int prec1 = Definitions.GetPrecedence(((Operator)token).Symbol);
                            int prec2 = Definitions.GetPrecedence(((Operator)top).Symbol);
                            char assoc1 = Definitions.GetAssociativity(((Operator)token).Symbol);
                            char assoc2 = Definitions.GetAssociativity(((Operator)top).Symbol);
                            bool condition1 = (assoc1 == 'r' && prec1 < prec2);
                            bool condition2 = (assoc1 == 'l' && prec1 <= prec2);
                            if (condition1 || condition2)
                            {
                                postfix.Add(stack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                        stack.Push(token);
                    }
                }
                // Current token is an opening bracket
                else if (token is Bracket && ((Bracket)token).IsOpening == true)
                {
                    stack.Push(token);
                }
                // Current token is an closing bracket, solve entire brace by popping until a opening brace is found
                else if (token is Bracket && ((Bracket)token).IsOpening == false)
                {
                    while (stack.Count > 0)
                    {
                        top = stack.Peek();
                        bool isAPartnerBracket = (top.Identifier[0] == Definitions.GetPartnerBracket(token.Identifier[0]));
                        if (top is Bracket && ((Bracket)top).IsOpening && isAPartnerBracket)
                        {
                            stack.Pop();
                            break;
                        }
                        else
                        {
                            postfix.Add(stack.Pop());
                        }
                    }
                }
                // Current token is a Summand, add it to the result
                else
                {
                    postfix.Add(token);
                }
            }
            // Empty out remainder
            while (stack.Count > 0)
            {
                postfix.Add(stack.Pop());
            }
            return postfix;
        }
    }
}
