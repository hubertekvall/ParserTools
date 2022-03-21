namespace ParserTools.Samples;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserTools.Optionals;


public enum ArithmeticTypes
{
    Number,
    Addition,
    Subtraction,
    Multiplication,
    Division,
    LeftParenthesis,
    RightParenthesis
}


public class ArithmeticLexer : Lexer<ArithmeticTypes>
{
    public ArithmeticLexer(string source) : base(source)
    {
    }


   
    protected override Token[] RunLexer()
    {
        Reset();

        while (Peek())
        {
            BeginScan();
            var ch = Pop();

            switch (ch.Value)
            {
                case ' ':
                case '\t':
                case '\r':
                    break;

                case '\n':
                    NewLine();
                    break;


                // Number
                case char d when Char.IsDigit(d):

                    while (Char.IsDigit(Peek().Value)) Pop();
                    if (Peek().Value == '.') Pop();
                    while (Char.IsDigit(Peek().Value)) Pop();

                    break;


                // Parenthesis
                case '(':
                    FinalizeToken(ArithmeticTypes.LeftParenthesis);
                    break;

                case ')':
                    FinalizeToken(ArithmeticTypes.RightParenthesis);
                    break;


                // Operators:

                case '+':
                case '-':
                case '*':
                case '/':

                    var type = ch.Value switch
                    {
                        '+' => ArithmeticTypes.Addition,
                        '-' => ArithmeticTypes.Subtraction,
                        '*' => ArithmeticTypes.Multiplication,
                        '/' => ArithmeticTypes.Division
                    };

                    FinalizeToken(type);
                    break;

                default:

                    throw new LexingError("Unknown symbol", this);
                    

            }
        }

        return TokenBuffer.ToArray();
    }
}

