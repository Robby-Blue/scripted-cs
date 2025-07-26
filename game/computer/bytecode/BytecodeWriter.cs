using System;
using System.Collections.Generic;
using Godot;

public partial class BytecodeWriter
{

    public static List<byte> Write(Instruction[] instructions)
    {
        List<byte> bytes = [];

        foreach (char c in "csc")
        {
            bytes.Add((byte)c); // signature
        }
        bytes.Add(0); // version major
        bytes.Add(0); // version minor
        bytes.Add(0); // version patch
        foreach (Instruction instruction in instructions)
        {
            bytes.Add(0);
            bytes.AddRange(WriteInstruction(instruction));
        }

        bytes.Add(1);

        return bytes;
    }

    public static List<byte> WriteInstruction(Instruction instruction)
    {
        List<byte> bytes = [];
        bytes.Add((byte)instruction.Op);

        bytes.Add((byte)instruction.Args.Length);
        foreach (object arg in instruction.Args)
        {
            bytes.AddRange(WriteArgument(arg));
        }

        return bytes;
    }

    public static List<byte> WriteArgument(object arg)
    {
        List<byte> bytes = [];
        if (arg is string str)
        {
            bytes.Add((byte)BytecodeType.STRING);
            bytes.AddRange(WriteString(str));
        }
        else if (arg is int num)
        {
            bytes.Add((byte)BytecodeType.INT);
            bytes.AddRange(BitConverter.GetBytes(num));
        }
        else if (arg is Argument argument)
        {
            bytes.Add((byte)BytecodeType.ARGUMENT);
            bytes.Add((byte)argument.ValType);
            if (argument.ValType != Argument.Type.Stack)
            {
                bytes.AddRange(WriteArgument(argument.Data));
            }
        }
        else if (arg is Type type)
        {
            bytes.Add((byte)BytecodeType.TYPE);
            string serialized = type.AssemblyQualifiedName;
            bytes.AddRange(WriteString(serialized));
        }
        else
        {
            GD.PushError("BytecodeWriter: write invalid type " + arg.GetType().FullName);
        }
        return bytes;
    }

    public static List<byte> WriteString(string str)
    {
        List<byte> bytes = [];
        foreach (char c in str)
        {
            bytes.Add((byte)c);
        }
        bytes.Add(0);
        return bytes;
    }

}
