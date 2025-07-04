using System;

class Argument
{
    public Type ValType;
    public object Data;

    public Argument(Type type)
    {
        this.ValType = type;
    }

    public Argument(Type type, object data)
    {
        ValType = type;
        Data = data;
    }

    public enum Type
    {
        Literal,
        Variable,
        Stack
    }

}
