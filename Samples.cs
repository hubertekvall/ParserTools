namespace ParserTools.Samples;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using ParserTools.Optionals;

using static ArithmeticTypes;
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




public class ArithmeticLexer : Lexer
{
    public ArithmeticLexer(string source) : base(source)
    {
    }



    protected override Token[] RunLexer()
    {
        Reset();

        while (Peek())
        {
            NewScan();
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

                    FinalizeToken((int)Number);

                    break;


                // Parenthesis
                case '(':
                    FinalizeToken((int)LeftParenthesis);
                    break;

                case ')':
                    FinalizeToken((int)RightParenthesis);
                    break;


                // Operators:

                case '+':
                    FinalizeToken((int)Addition);
                    break;
                case '-':
                    FinalizeToken((int)Subtraction);
                    break;
                case '*':
                    FinalizeToken((int)Multiplication);
                    break;
                case '/':
                    FinalizeToken((int)Division);
                    break;



                default:

                    throw new LexingError("Unknown symbol", this);


            }
        }

        Logger.LogMessage("Token length: " + TokenBuffer.Count);

        return TokenBuffer.ToArray();
    }
}











public class ParsingException : Exception
{

}





public class ArithmeticParser : Scanner<Token>
{
    public class Node
    {
        Token token;
        Node? left;
        Node? right;

        public Node(Token token, Node? left = null, Node? right = null)
        {
            this.token = token;
            this.left = left;
            this.right = right;
        }

        static Stack<int> _stack;
        static public Stack<int> NumberStack { get { return _stack ?? new Stack<int>(); } }
        public static void Calculate(Node n)
        {


            if (n is null) return;
            if (n.left is not null) Calculate(n.left);
            if (n.right is not null) Calculate(n.right);

            switch (n.token.Type)
            {
                case (int)Number:
                    NumberStack.Push(int.Parse(n.token.Content.Span));
                    break;

                case (int)Addition:
                    {
                        int v1 = NumberStack.Pop();
                        int v2 = NumberStack.Pop();
                        NumberStack.Push(v2 + v1);
                    }
                    break;
                case (int)Subtraction:
                    {
                        int v1 = NumberStack.Pop();
                        int v2 = NumberStack.Pop();
                        NumberStack.Push(v2 - v1);
                    }
                    break;
                case (int)Multiplication:
                    {
                        int v1 = NumberStack.Pop();
                        int v2 = NumberStack.Pop();
                        NumberStack.Push(v2 * v1);
                    }
                    break;
                case (int)Division:
                    {
                        int v1 = NumberStack.Pop();
                        int v2 = NumberStack.Pop();
                        NumberStack.Push(v2 / v1);
                    }
                    break;

            }
        }
    }


    public ArithmeticParser(IEnumerable<Token> source) : base(source)
    {
    }


    public Result<Node, ParsingException> Parse()
    {
        try
        {
            return Result<ArithmeticParser.Node, ParsingException>.Create(Expression());
        }

        catch (ParsingException e)
        {
            return Result<ArithmeticParser.Node, ParsingException>.Create(e);
        }

    }


    Node Expression()
    {
       return Term();
    }

    Node Term()
    {
        Node root = Factor();

        while (Peek().Value.Type == (int)Addition || Peek().Value.Type == (int)Subtraction)
        {
            root = new Node(Pop().Value, root, Factor());
        }

        return root;
    }

    Node Factor()
    {
        Node root = Primary();

        while (Peek().Value.Type == (int)Multiplication || Peek().Value.Type == (int)Division)
        {
            root = new Node(Pop().Value, root, Primary());
        }

        return root;
    }

    Node Primary()
    {
        if (Peek().Value.Type == (int)Number) return new Node(Pop().Value);
        throw new Exception("Expected expression");
    }

}

