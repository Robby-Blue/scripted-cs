using System.Collections.Generic;
using Godot;

public partial class Computer
{

    private Folder rootFolder = new(new Path("/"));
    private BytecodeInterpreter interpeter;
    private ComputerScreen screen;

    public Computer(ComputerScreen screen)
    {
        Instruction[] bytecode = [
            new Instruction(Opcode.DEF_FUNC, "read_font"),
            new Instruction(Opcode.SYSC, "open_file", new Argument(Argument.Type.Literal, "/font.sfb")),
            new Instruction(Opcode.SYSC, "read_file", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.SET, "data", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.NEW_DICT),
            new Instruction(Opcode.SET, "chars", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.SET, "i", new Argument(Argument.Type.Literal, 0)),

            new Instruction(Opcode.LABEL, "loop_read_char"),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "data")),
            new Instruction(Opcode.LOOKUP, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.SET, "char_idx", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.NEW_ARR),
            new Instruction(Opcode.SET, "coords", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "i", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.LABEL, "loop_read_coord"),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "data")),
            new Instruction(Opcode.LEN),
            new Instruction(Opcode.CMP_GTEQ),
            new Instruction(Opcode.JMP_IF, "done"),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "data")),
            new Instruction(Opcode.LOOKUP, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.CAST, typeof(int)),
            new Instruction(Opcode.SET, "x", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "x")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 65)),
            new Instruction(Opcode.CMP_GTEQ),
            new Instruction(Opcode.NOT),
            new Instruction(Opcode.JMP_IF, "continue_reading_char"),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "char_idx")),
            new Instruction(Opcode.CAST, typeof(char)),
            new Instruction(Opcode.WRITE, new Argument(Argument.Type.Variable, "chars"), new Argument(Argument.Type.Stack), new Argument(Argument.Type.Variable, "coords")),
            new Instruction(Opcode.JMP, "loop_read_char"),
            new Instruction(Opcode.LABEL, "continue_reading_char"),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "data")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.LOOKUP, new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.CAST, typeof(int)),
            new Instruction(Opcode.SET, "y", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.NEW_ARR),
            new Instruction(Opcode.SET, "coord", new Argument(Argument.Type.Stack)),
            new Instruction(Opcode.ARR_APPEND, new Argument(Argument.Type.Variable, "coord"), new Argument(Argument.Type.Variable, "x")),
            new Instruction(Opcode.ARR_APPEND, new Argument(Argument.Type.Variable, "coord"), new Argument(Argument.Type.Variable, "y")),
            new Instruction(Opcode.ARR_APPEND, new Argument(Argument.Type.Variable, "coords"), new Argument(Argument.Type.Variable, "coord")),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "i")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 2)),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "i", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.JMP, "loop_read_coord"),
            new Instruction(Opcode.LABEL, "done"),
            new Instruction(Opcode.RET, new Argument(Argument.Type.Variable, "chars")),

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
            new Instruction(Opcode.CALL, "read_font"),
            new Instruction(Opcode.SET, "chars", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.SET, "x_offset", new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.SET, "y_offset", new Argument(Argument.Type.Literal, 1)),

            new Instruction(Opcode.LABEL, "loop2"),
            new Instruction(Opcode.CALL, "draw_char", new Argument(Argument.Type.Literal, 'B')),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "x_offset")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 6)),
            new Instruction(Opcode.ADD),
            new Instruction(Opcode.SET, "x_offset", new Argument(Argument.Type.Stack)),

            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "x_offset")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 347)),
            new Instruction(Opcode.CMP_GTEQ),
            new Instruction(Opcode.NOT),
            new Instruction(Opcode.JMP_IF, "endif"),
            new Instruction(Opcode.SET, "x_offset", new Argument(Argument.Type.Literal, 1)),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Variable, "y_offset")),
            new Instruction(Opcode.PUSH, new Argument(Argument.Type.Literal, 8)),
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

        rootFolder.AddNode(new File(new Path("font.sfb"), ReadFont()));
    }

    private List<byte> ReadFont()
    {
        Color black = Color.Color8(0, 0, 0);

        Image fontImage = new();
        fontImage.Load("computer/font.png");

        int asciiAIndex = 65;
        int charWidth = 6;
        int charHeight = 7;
        List<byte> bytes = [];

        for (int i = 0; i <= fontImage.GetWidth() / charWidth; i++)
        {
            bytes.Add((byte)(i + asciiAIndex));
            for (int xOffset = 0; xOffset < charWidth - 1; xOffset++)
            {
                for (int yOffset = 0; yOffset < charHeight; yOffset++)
                {
                    Color c = fontImage.GetPixel(i * charWidth + xOffset, yOffset);
                    if (c == black)
                    {
                        continue;
                    }
                    bytes.Add((byte)xOffset);
                    bytes.Add((byte)yOffset);
                }
            }
        }

        return bytes;
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

    public FileNode GetFileNode(string pathString)
    {
        Path path = new(pathString);
        return rootFolder.GetFileNode(path);
    }

}
