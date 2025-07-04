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
            [Opcode.LOOKUP] = ExecuteLookup,
            [Opcode.WRITE] = ExecuteWrite,
            [Opcode.LEN] = ExecuteLen,
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
        ip = (int)stack.Pop();
        variables.RemoveAt(variables.Count - 1);
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

    private void ExecuteLookup(object[] args)
    {
        object o = stack.Pop();
        if (o is IList list)
        {
            int index = GetArgValue<int>(args[0]);
            object value = list[index];
            stack.Push(value);
        }
        if (o is IDictionary dict)
        {
            string key = GetArgValue<string>(args[0]);
            object value = dict[key];
            stack.Push(value);
        }
    }

    private void ExecuteWrite(object[] args)
    {
        throw new NotImplementedException();
    }

    private void ExecuteLen(object[] args)
    {
        object v = stack.Pop();
        IList l = (IList)v;
        stack.Push(l.Count);
    }

    private void ExecuteSysc(object[] args)
    {
        string methodName = args[0].ToString();

        if (methodName.Equals("set_pixel"))
        {
            int x = GetArgValue<int>(args[1]);
            int y = GetArgValue<int>(args[2]);
            int brightness = GetArgValue<int>(args[3]);
            computer.SetPixel(x, y, brightness);
        }
    }

    private void ExecuteHalt(object[] args)
    {
        IsRunning = false;
    }

    private (T, T) StackPopTwo<T>()
    {
        T a = (T)stack.Pop();
        T b = (T)stack.Pop();
        return (b, a);
    }

    private object GetArgValue(object argument)
    {
        Argument arg = (Argument)argument;
        switch (arg.ValType)
        {
            case Argument.Type.Literal:
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
        return (T)GetArgValue(arg);
    }

}
