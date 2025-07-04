using System.Collections.Generic;

public partial class Computer
{

    private BytecodeInterpreter interpeter;
    private ComputerScreen screen;

    public Computer(ComputerScreen screen)
    {
        Instruction[] bytecode = [
            new Instruction(Opcode.DEF_FUNC, "draw_char", "char"),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "chars")),
            new Instruction(Opcode.LOOKUP, new Argument(Argument.Type.Variable, "char")),
            new Instruction(Opcode.SET, "pixels", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "pixels")),
            new Instruction(Opcode.LEN),
            new Instruction(Opcode.SET, "len", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.SET, "i", new Argument(Argument.Type.Literal, 0)),
            new Instruction(Opcode.LABEL, "loop"),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "pixels")),
            new Instruction(Opcode.LOOKUP, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.SET, "pos", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "pos")),
            new Instruction(Opcode.LOOKUP, new Argument(Argument.Type.Literal, 0)),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "x_offset")),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "x", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "pos")),
            new Instruction(Opcode.LOOKUP, new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "y_offset")),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "y", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.SYSC, "set_pixel",
                new Argument(Argument.Type.Variable, "x"),
                new Argument(Argument.Type.Variable, "y"),
                new Argument(Argument.Type.Literal, 255)),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "i", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "len")),
            new Instruction(Opcode.CMP_LT),
            new Instruction(Opcode.JMP_IF, "loop"),
            new Instruction(Opcode.RET),

            new Instruction(Opcode.LABEL, "start"),
            new Instruction(Opcode.SET, "chars", new Argument(Argument.Type.Literal, new Dictionary<string, object> {
                { "a", new List<List<int>> {
                    new() {0, 0}, new() {0, 1}, new() {0, 2}, new() {0, 3}, new() {0, 4},
                    new() {1, 2}, new() {2, 2},
                    new() {3, 0}, new() {3, 1}, new() {3, 2}, new() {3, 3}, new() {3, 4}
                }},
                { "b", 2 }
            })),
            new Instruction(Opcode.SET, "x_offset", new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.SET, "y_offset", new Argument(Argument.Type.Literal, 1)),

            new Instruction(Opcode.LABEL, "loop2"),
            new Instruction(Opcode.CALL, "draw_char", new Argument(Argument.Type.Literal, "a")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "x_offset")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 5)),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "x_offset", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "x_offset")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 347)),
            new Instruction(Opcode.CMP_GTEQ),
            new Instruction(Opcode.NOT),
            new Instruction(Opcode.JMP_IF, "endif"),
            new Instruction(Opcode.SET, "x_offset", new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "y_offset")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 7)),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "y_offset", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.LABEL, "endif"),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "y_offset")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 275)),
            new Instruction(Opcode.CMP_GTEQ),
            new Instruction(Opcode.NOT),
            new Instruction(Opcode.JMP_IF, "endif2"),
            new Instruction(Opcode.SET, "y_offset", new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.LABEL, "endif2"),

            new Instruction(Opcode.JMP, "loop2"),
            new Instruction(Opcode.HALT)
        ];

        interpeter = new BytecodeInterpreter(this, bytecode);
        this.screen = screen;
    }

    public void Step()
    {
        if (interpeter.IsRunning)
        {
            interpeter.Step();
        }
    }

    public void SetPixel(int x, int y, int brightness)
    {
        screen.SetPixel(x, y, brightness);
    }

}
