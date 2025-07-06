using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class BytecodeInterpreter
{

    private Computer computer;
    private Instruction[] instructions;

    private int ip = 0;
    public bool IsRunning = true;

    private List<Dictionary<string, object>> variables = [[]];
    private Stack<object> stack = [];

    private Dictionary<string, int> labels = [];
    private Dictionary<string, int> funcs = [];

    private Dictionary<Opcode, Action<object[]>> bytecode_funcs;

    public BytecodeInterpreter(Computer computer, Instruction[] instructions)
    {
        bytecode_funcs = new()
        {
            [Opcode.SET] = ExecuteSet,
            [Opcode.LABEL] = ExecuteLabel,
            [Opcode.JMP] = ExecuteJmp,
            [Opcode.JMP_IF] = ExecuteJmpIf,
            [Opcode.DEF_FUNC] = ExecuteDefFunc,
            [Opcode.CALL] = ExecuteCall,
            [Opcode.RET] = ExecuteRet,
            [Opcode.PUSH] = ExecutePush,
            [Opcode.POP] = ExecutePop,
            [Opcode.CMP_EQ] = ExecuteCmpEq,
            [Opcode.CMP_LT] = ExecuteCmpLt,
            [Opcode.CMP_GT] = ExecuteCmpGt,
            [Opcode.CMP_LTEQ] = ExecuteCmpLtEq,
            [Opcode.CMP_GTEQ] = ExecuteCmpGtEq,
            [Opcode.ADD] = ExecuteAdd,
            [Opcode.SUB] = ExecuteSub,
            [Opcode.MULT] = ExecuteMult,
            [Opcode.DIV] = ExecuteDiv,
            [Opcode.NOT] = ExecuteNot,
            [Opcode.AND] = ExecuteAnd,
            [Opcode.OR] = ExecuteOr,
            [Opcode.CAST] = ExecuteCast,
            [Opcode.LOOKUP] = ExecuteLookup,
            [Opcode.ARR_APPEND] = ExecuteArrAppend,
            [Opcode.WRITE] = ExecuteWrite,
            [Opcode.LEN] = ExecuteLen,
            [Opcode.NEW_ARR] = ExecuteNewArr,
            [Opcode.NEW_DICT] = ExecuteNewDict,
            [Opcode.SYSC] = ExecuteSysc,
            [Opcode.HALT] = ExecuteHalt
        };

        this.computer = computer;
        this.instructions = instructions;

        int i = 0;
        foreach (Instruction instruction in instructions)
        {
            var opcode = instruction.Op;
            if (opcode == Opcode.LABEL)
            {
                labels[instruction.Args[0].ToString()] = i;
            }
            if (opcode == Opcode.DEF_FUNC)
            {
                funcs[instruction.Args[0].ToString()] = i;
            }
            i++;
        }

        ip = labels["start"];
    }

    public void Step()
    {
        Instruction instruction = instructions[ip++];

        Action<object[]> func = bytecode_funcs[instruction.Op];
        func(instruction.Args);
    }

    private void ExecuteSet(object[] args)
    {
        string varName = (string)args[0];
        variables.Last()[varName] = GetArgValue(args[1]);
    }

    private void ExecuteLabel(object[] args)
    {
        Step();
    }

    private void ExecuteJmp(object[] args)
    {
        string label = args[0].ToString();
        ip = labels[label];
    }

    private void ExecuteJmpIf(object[] args)
    {
        if (!(stack.Pop() is true))
            return;
        string label = args[0].ToString();
        ip = labels[label];
    }

    private void ExecuteDefFunc(object[] args)
    {
        for (int i = 1; i < args.Length; i++)
        {
            string varName = args[i].ToString();
            variables.Last()[varName] = stack.Pop();
        }
        Step();
    }

    private void ExecuteCall(object[] args)
    {
        List<object> variablesStack = new();
        for (int i = 1; i < args.Length; i++)
        {
            variablesStack.Add(GetArgValue(args[i]));
        }

        variables.Add(new Dictionary<string, object>());
        stack.Push(ip);
        ip = funcs[args[0].ToString()];

        foreach (object var in variablesStack)
        {
            stack.Push(var);
        }
    }

    private void ExecuteRet(object[] args)
    {
        List<object> variablesStack = new();
        for (int i = 0; i < args.Length; i++)
        {
            variablesStack.Add(GetArgValue(args[i]));
        }

        ip = (int)stack.Pop();
        variables.RemoveAt(variables.Count - 1);

        foreach (object var in variablesStack)
        {
            stack.Push(var);
        }
    }

    private void ExecutePush(object[] args)
    {
        stack.Push(GetArgValue(args[0]));
    }

    private void ExecutePop(object[] args)
    {
        stack.Pop();
    }

    private void ExecuteCmpEq(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a == b);
    }

    private void ExecuteCmpGt(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a > b);
    }

    private void ExecuteCmpLt(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a < b);
    }

    private void ExecuteCmpGtEq(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a >= b);
    }

    private void ExecuteCmpLtEq(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a <= b);
    }

    private void ExecuteAdd(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a + b);
    }

    private void ExecuteSub(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a - b);
    }

    private void ExecuteMult(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a * b);
    }

    private void ExecuteDiv(object[] args)
    {
        (int a, int b) = StackPopTwo<int>();
        stack.Push(a / b);
    }

    private void ExecuteNot(object[] args)
    {
        bool b = (bool)stack.Pop();
        stack.Push(!b);
    }

    private void ExecuteAnd(object[] args)
    {
        (bool a, bool b) = StackPopTwo<bool>();
        stack.Push(a && b);
    }

    private void ExecuteOr(object[] args)
    {
        (bool a, bool b) = StackPopTwo<bool>();
        stack.Push(a || b);
    }

    private void ExecuteCast(object[] args)
    {
        object val = stack.Pop();
        Type type = (Type)args[0];
        stack.Push(Convert.ChangeType(val, type));
    }

    private void ExecuteLookup(object[] args)
    {
        object lookupKey = GetArgValue(args[0]);
        object collection = stack.Pop();
        if (collection is IList list && lookupKey is int index)
        {
            object value = list[index];
            stack.Push(value);
        }
        if (collection is IDictionary dict)
        {
            object value = dict[lookupKey];
            stack.Push(value);
        }
    }

    private void ExecuteArrAppend(object[] args)
    {
        IList array = GetArgValue<IList>(args[0]);
        object data = GetArgValue<object>(args[1]);
        array.Add(data);
    }

    private void ExecuteWrite(object[] args)
    {
        object collection = GetArgValue(args[0]);
        object lookupKey = GetArgValue(args[1]);
        object value = GetArgValue(args[2]);
        if (collection is IList list && lookupKey is int index)
        {
            list[index] = value;
        }
        if (collection is IDictionary dict)
        {
            dict[lookupKey] = value;
        }
    }

    private void ExecuteLen(object[] args)
    {
        object v = stack.Pop();
        IList l = (IList)v;
        stack.Push(l.Count);
    }

    private void ExecuteNewArr(object[] args)
    {
        stack.Push(new List<object>());
    }

    private void ExecuteNewDict(object[] args)
    {
        stack.Push(new Dictionary<object, object>());
    }

    private void ExecuteSysc(object[] args)
    {
        string methodName = args[0].ToString();

        if (methodName.Equals("write_stdout"))
        {
            GD.Print(GetArgValue(args[1]));
        }
        if (methodName.Equals("set_pixel"))
        {
            int x = GetArgValue<int>(args[1]);
            int y = GetArgValue<int>(args[2]);
            int brightness = GetArgValue<int>(args[3]);
            computer.SetPixel(x, y, brightness);
        }
        if (methodName.Equals("open_file"))
        {
            string path = GetArgValue<string>(args[1]);
            FileNode fileNode = computer.GetFileNode(path);
            stack.Push(new FileReader((File)fileNode));
        }
        if (methodName.Equals("read_file"))
        {
            FileReader reader = GetArgValue<FileReader>(args[1]);
            List<byte> contents = reader.Read();
            stack.Push(contents);
        }
    }

    private void ExecuteHalt(object[] args)
    {
        IsRunning = false;
    }

    private (T, T) StackPopTwo<T>()
    {
        T a = (T)CastVariable<T>(stack.Pop());
        T b = (T)CastVariable<T>(stack.Pop());
        return (b, a);
    }

    private object GetArgValue(object argument)
    {
        Argument arg = (Argument)argument;
        switch (arg.ValType)
        {
            case Argument.Type.Literal:
                if (arg.Data is ICloneable cloneable)
                {
                    return cloneable.Clone();
                }
                return arg.Data;
            case Argument.Type.Variable:
                string key = arg.Data.ToString();
                if (variables.Last().ContainsKey(key))
                {
                    return variables.Last()[key];
                }
                return variables.First()[key];
            case Argument.Type.Stack:
                return stack.Pop();
        }
        return null;
    }

    private T GetArgValue<T>(object arg)
    {
        return (T)CastVariable<T>(GetArgValue(arg));
    }

    private T CastVariable<T>(object value)
    {
        if (value is IConvertible)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        else
        {
            return (T)value;
        }
    }

}
