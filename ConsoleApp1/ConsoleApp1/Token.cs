﻿namespace Compilador
{
    public class Token
    {
        public string Type { get; }
        public object Value { get; }

        public Token(string type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}