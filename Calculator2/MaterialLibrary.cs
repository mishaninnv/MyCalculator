using System;
using System.Collections.Generic;

namespace Calculator2
{
    public static class MaterialLibrary
    {
        public static string ValidSymbols => "0123456789+-*/().,";
        public static string SymbolsToStart => "+-(1234567890.,";
        public static string SpecialSymbolsToStart => "+-.,";
        public static string SymbolsBeforeSeparator => "0123456789).,";
        public static string SymbolsBeforeOpenBound => "0123456789)";
        public static string SpecialSymbolsAfterOpenBound => "+-";
        public static string ProhibitedSymbolsAfterOpenBound => "/)*";
        public static string SpecialSymbolsAfterCloseBound => "0123456789(";
        public static string ProhibitedSymbolsBeforeCloseBound => "+-*/.,";
        public static string ConsistensSymbols => "+-*/.,";
        public static Dictionary<string, int> OperatorValues { get; } = new Dictionary<string, int>
        {
            {"(", 0},
            {")", 1},
            {"+", 2},
            {"-", 2},
            {"*", 3},
            {"/", 3}
        };
        
        public static readonly Dictionary<string, Func<string, string, string>> Operations =
            new Dictionary<string, Func<string, string, string>>
            {
                {"+", (x, y) => (double.Parse(x) + double.Parse(y)).ToString()},
                {"-", (x, y) => (double.Parse(x) - double.Parse(y)).ToString()},
                {"*", (x, y) => (double.Parse(x) * double.Parse(y)).ToString()},
                {"/", (x, y) => (double.Parse(x) / double.Parse(y)).ToString()}
            };
    }
    
    public struct Element
    {
        public EType Type;
        public string Value;
    }

    public enum EType
    {
        Operator,
        Operand
    }
}