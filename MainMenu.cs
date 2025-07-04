using Godot;

public partial class MainMenu : Control
{
    private Computer computer;
    private ComputerScreen screen;
    private float deltaSum = 0f;

    public override void _Ready()
    {
        screen = GetNode<ComputerScreen>("Screen");
        computer = new Computer(screen);
    }

    public override void _Process(double delta)
    {
        float menuWidth = Size.X;
        float menuHeight = Size.Y;

        float screenWidth = screen.Size.X;
        float screenHeight = screen.Size.Y;

        float scaleX = menuWidth / screenWidth * 0.9f;
        float scaleY = menuHeight / screenHeight * 0.9f;

        float scale = (scaleX < scaleY) ? scaleX : scaleY;

        screen.Scale = new Vector2(scale, scale);
        screen.Position = new Vector2(
            menuWidth / 2f - screenWidth * scale / 2f,
            menuHeight / 2f - screenHeight * scale / 2f
        );
    }

    public override void _PhysicsProcess(double delta)
    {
        deltaSum += (float)delta;
        while (deltaSum > 0.000001f)
        {
            computer.Step();
            deltaSum -= 0.000001f;
        }
    }
}
