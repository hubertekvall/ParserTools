namespace ParserTools;

using System;
using System.Collections.Generic;
using ParserTools.Optionals;


public class LexingError : System.Exception
{

    int line;
    int column;

    public LexingError(string? message, LexemeScanner scanner) : base(message)
    {
        this.line = scanner.LineCount;
        this.column = scanner.ColumnCount;
    }
}



public class LexemeScanner : Scanner<char>
{
    protected int scanStart = 0;
    protected int lineCount = 0;
    protected int columnCount = 0;

    public int ScanStart { get => scanStart; }
    public int LineCount { get => lineCount; }
    public int ColumnCount { get => columnCount; }

    protected LexemeScanner(IEnumerable<char> source) : base(source)
    {
    }
}




public abstract class Lexer<TokenEnum> : LexemeScanner
{
    public struct Token
    {
        TokenEnum type;
        ReadOnlyMemory<char> content;
        int line;
        int column;

        public Token(TokenEnum type, ReadOnlyMemory<char> content, int line, int column)
        {
            this.type = type;
            this.content = content;
            this.line = line;
            this.column = column;
        }
    }







    private List<Token>? tokenBuffer;
    protected List<Token> TokenBuffer{ get { return tokenBuffer ??= new List<Token>(); } }


    public Lexer(IEnumerable<char> source) : base(source)
    {
    }


    public void BeginScan() => scanStart = 0;
    public void NewLine()
    {
        columnCount = 0;
        lineCount++;
    }


    public override void Reset()
    {
        columnCount = 0;
        lineCount = 0;
        scanOffset = 0;
        scanStart = 0;
        TokenBuffer.Clear();
    }

    public override Optional<char> Pop()
    {
        return base.Pop().IfNotNull((o) => columnCount++);
    }

    public Token FinalizeToken(TokenEnum type)
    {
        Token newToken = new(type, ((string)buffer).AsMemory(scanStart, scanOffset), lineCount, columnCount);
        TokenBuffer.Add(newToken);
        return newToken;
    }

   


    public Result<Token[], LexingError> Lex()
    {
        try
        {
            return Result<Token[], LexingError>.Create(RunLexer());
        }
        catch (LexingError e)
        {
            return Result<Token[], LexingError>.Create(e);
        }
    }

    public Result<Token[], LexingError> Lex(IEnumerable<char> source)
    {
        this.buffer = source;
        return Lex();
    }





    protected abstract Token[] RunLexer();
}



