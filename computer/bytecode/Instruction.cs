enum Opcode
{
    SYSC,
    HALT
}

class Instruction
{
    public Opcode Op;
    public object[] Args;

    public Instruction(Opcode op, params object[] args)
    {
        Op = op;
        Args = args;
    }
}
