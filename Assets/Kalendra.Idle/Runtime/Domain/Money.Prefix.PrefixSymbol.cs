using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kalendra.Idle.Runtime
{
    public readonly partial struct Money
    {
        readonly partial struct Prefix
        {
            readonly struct PrefixSymbol
            {
                static readonly List<string> FixedSymbols = new List<string>() {"", "k", "M", "B", "T"};

                readonly string symbol;
                
                #region Constructors
                PrefixSymbol(string symbol)
                {
                    if(!IsSymbol(symbol))
                        throw new ArgumentException($"Unknown prefix symbol: {symbol}", nameof(symbol));

                    this.symbol = symbol;
                }

                static bool IsSymbol(string source)
                {
                    return FixedSymbols.Contains(source) ||
                           source.Length > 1 && source.All(char.IsLetter);
                }

                /// <remarks>
                /// Recursion makes this method extremely slow. 
                /// </remarks>
                public static int IndexOf(PrefixSymbol prefixSymbol)
                {
                    if(prefixSymbol == Empty)
                        return 0;

                    return 1 + IndexOf(prefixSymbol.Prev());
                }
                #endregion

                public PrefixSymbol Next()
                {
                    if(symbol == LastFixed)
                        return FirstNominal;

                    if(FixedSymbols.Contains(symbol))
                        return From(FixedSymbols[FixedSymbols.IndexOf(symbol) + 1]);

                    return NextFrom(symbol);
                }

                public PrefixSymbol Prev()
                {
                    if(this == Empty)
                        throw new InvalidOperationException("EmptyPrefixSymbol has no previous one");
                    
                    if(this == FirstNominal)
                        return LastFixed;

                    if(FixedSymbols.Contains(symbol))
                        return From(FixedSymbols[FixedSymbols.IndexOf(symbol) - 1]);

                    return PrevFrom(symbol);
                }

                #region Factory methods
                public static PrefixSymbol Empty => new PrefixSymbol("");
                public static PrefixSymbol FirstNominal => new PrefixSymbol("aa");
                public static PrefixSymbol LastFixed => new PrefixSymbol(FixedSymbols.Last());
                
                public static PrefixSymbol From(string symbol) => new PrefixSymbol(symbol);

                static PrefixSymbol PrevFrom(string symbol)
                {
                    var symbolBuilder = new StringBuilder(symbol);
                    
                    var carry = true;
                    for(var i = symbol.Length - 1; i >= 0 && carry; i--)
                    {
                        carry = symbol[i] == 'a';
                        symbolBuilder[i] = carry ? 'z' : (char)(symbol[i] - 1);
                    }

                    return From(symbolBuilder.ToString());
                }
                
                static PrefixSymbol NextFrom(string symbol)
                {
                    var symbolBuilder = new StringBuilder(symbol);
                    
                    var carry = true;
                    for(var i = symbol.Length - 1; i >= 0 && carry; i--)
                    {
                        carry = symbol[i] == 'z';
                        symbolBuilder[i] = carry ? 'a' : (char)(symbol[i] + 1);
                    }

                    return From(symbolBuilder.ToString());
                }
                #endregion
                
                #region Conversion operators
                public static implicit operator string(PrefixSymbol p) => p.symbol;
                #endregion
                
                #region Equality/Comparation
                public bool Equals(PrefixSymbol other) => other.symbol.Equals(symbol);
                public override bool Equals(object other) => other is PrefixSymbol o && Equals(o);
                public override int GetHashCode() => symbol.GetHashCode();

                public static bool operator ==(PrefixSymbol ps1, PrefixSymbol ps2) => ps1.Equals(ps2);
                public static bool operator !=(PrefixSymbol ps1, PrefixSymbol ps2) => !(ps1 == ps2);
                #endregion

                #region Formatting members
                public override string ToString() => symbol;
                #endregion
            }
        }
    }
}