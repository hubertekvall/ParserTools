namespace ParserTools;

using System;
using System.Collections.Generic;
using ParserTools.Optionals;


public class Scanner<T> where T : struct
{
    protected IEnumerable<T> buffer;

    protected int scanOffset = 0;
    public int ScanOffset{ get { return scanOffset; } }

    public virtual void Reset() => scanOffset = 0;

    public bool IsAtEnd() => scanOffset >= buffer?.Count();

    public Optional<T> Peek() => IsAtEnd() ? null : buffer?.ElementAt(scanOffset);

    public virtual Optional<T> Pop() => IsAtEnd() ? buffer?.ElementAt(scanOffset++) : null;

    public Scanner(IEnumerable<T> source) => this.buffer = source;
}














