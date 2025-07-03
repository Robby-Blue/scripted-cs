using System.Collections.Generic;

public partial class BytecodeInterpreter
{

    public bool IsRunning = true;

    private int ip = 0;
    private ComputerScreen screen;
    private Instruction[] instructions;

    private Dictionary<string, object> variables;
    private Stack<object> stack;

    public BytecodeInterpreter(ComputerScreen screen)
    {
        this.screen = screen;
        instructions = [
            new Instruction(Opcode.SYSC, "set_pixel",
                new Argument(Argument.Type.Literal, 2), new Argument(Argument.Type.Literal, 2), new Argument(Argument.Type.Literal, 255)),
            new Instruction(Opcode.HALT)
        ];
    }

    public void Step()
    {
        Instruction instruction = instructions[ip++];

        switch (instruction.Op)
        {
            case Opcode.SYSC:
                ExecuteSysc(instruction);
                break;
            case Opcode.HALT:
                IsRunning = false;
                break;
        }
    }

    private void ExecuteSysc(Instruction instruction)
    {
        string methodName = instruction.Args[0].ToString();
        if (methodName == "set_pixel")
        {
            int x = GetArgValue<int>(instruction, 1);
            int y = GetArgValue<int>(instruction, 2);
            int brightness = GetArgValue<int>(instruction, 3);
            screen.SetPixel(x, y, brightness);
        }
    }

    private T GetArgValue<T>(Instruction instruction, int index)
    {
        Argument arg = (Argument)instruction.Args[index];
        switch (arg.ValType)
        {
            case Argument.Type.Literal:
                return (T)arg.Data;
            case Argument.Type.Variable:
                return (T)variables[arg.Data.ToString()];
            case Argument.Type.Stack:
                return (T)stack.Pop();
        }
        return default(T);
    }

}
