namespace CalcEngine
{
    /// <summary>
    /// 表达式树节点
    /// </summary>
    internal class Token
    {
        public Tkid Id;
        public Tktype Type;
        public object Value;

        public Token(object value, Tkid id, Tktype type)
        {
            Value = value;
            Id = id;
            Type = type;
        }
    }

    /// <summary>
    /// 表达式类型
    /// </summary>
    internal enum Tkid
    {
        GT, LT, GE, LE, EQ, NE, // COMPARE
        ADD, SUB, // ADDSUB
        MUL, DIV, DIVINT, MOD, // MULDIV
        POWER, // POWER
        OPEN, CLOSE, END, COMMA, PERIOD, // GROUP
        ATOM, // LITERAL, IDENTIFIER
    }

    /// <summary>
    /// 表达式优先级类型
    /// </summary>
    internal enum Tktype
    {
        COMPARE,	// < > = <= >=
        ADDSUB,		// + -
        MULDIV,		// * /
        POWER,		// ^
        GROUP,		// ( ) , .
        LITERAL,	// 123.32, "Hello", etc.
        IDENTIFIER  // functions, external objects, bindings
    }
}
