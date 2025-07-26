using System;
using System.Collections.Generic;
using Godot;
using System.IO;

public partial class BytecodeReader
{

    public static Instruction[] Read(List<byte> bytes)
    {
        MemoryStream stream = new(bytes.ToArray());

        foreach (char c in "csc")
        {
            if (stream.ReadByte() != c)
            {
                return null;
            }
        }

        stream.ReadByte(); // version
        stream.ReadByte();
        stream.ReadByte();

        List<Instruction> instructions = [];
        while (true)
        {
            bool isDone = stream.ReadByte() == 1;
            if (isDone)
            {
                break;
            }
            instructions.Add(ReadInstruction(stream));
        }
        instructions.Add(ReadInstruction(stream));
        return instructions.ToArray();
    }

    public static Instruction ReadInstruction(MemoryStream stream)
    {
        Opcode op = (Opcode)stream.ReadByte();
        int argCount = stream.ReadByte();

        List<object> args = [];
        for (int i = 0; i < argCount; i++)
        {
            args.Add(ReadArgument(stream));
        }

        return new Instruction(op, args.ToArray());
    }

    public static object ReadArgument(MemoryStream stream)
    {
        BytecodeType type = (BytecodeType)stream.ReadByte();
        if (type == BytecodeType.STRING)
        {
            return ReadString(stream);
        }
        else if (type == BytecodeType.INT)
        {
            byte[] b = [0, 0, 0, 0];
            for (int i = 0; i < 4; i++)
            {
                b[i] = (byte)stream.ReadByte();
            }
            return BitConverter.ToInt32(b);
        }
        else if (type == BytecodeType.ARGUMENT)
        {
            Argument.Type argType = (Argument.Type)stream.ReadByte();
            if (argType == Argument.Type.Stack)
            {
                return new Argument(argType);
            }
            else
            {
                return new Argument(argType, ReadArgument(stream));
            }
        }
        else if (type == BytecodeType.TYPE)
        {
            return Type.GetType(ReadString(stream));
        }
        else
        {
            GD.PushError("BytecodeReader: read invalid type" + type);
            return null;
        }
    }

    public static string ReadString(MemoryStream stream)
    {
        List<char> chars = [];
        while (stream.CanRead)
        {
            byte b = (byte)stream.ReadByte();
            if (b == 0b0)
            {
                break;
            }
            chars.Add((char)b);
        }
        return new string(chars.ToArray());
    }

}
