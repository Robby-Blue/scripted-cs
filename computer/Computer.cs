public partial class Computer
{

    private BytecodeInterpreter interpeter;
    private ComputerScreen screen;

    public Computer(ComputerScreen screen)
    {
        this.interpeter = new BytecodeInterpreter(screen);
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
