public enum Opcode
{
    SET,
    LABEL,
    JMP,
    JMP_IF,
    DEF_FUNC,
    CALL,
    RET,
    PUSH,
    POP,
    CMP_EQ,
    CMP_LT,
    CMP_GT,
    CMP_LTEQ,
    CMP_GTEQ,
    ADD,
    SUB,
    MULT,
    DIV,
    NOT,
    AND,
    OR,
    LOOKUP,
    WRITE,
    LEN,
    SYSC,
    HALT
}

public class Instruction
{
    public Opcode Op;
    public object[] Args;

    public Instruction(Opcode op, params object[] args)
    {
        Op = op;
        Args = args;
    }
}
