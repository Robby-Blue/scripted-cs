using Godot;

public partial class ComputerScreen : TextureRect
{

    private bool wasTextureChanged = false;
    private Image image;
    private int width = 351;
    private int height = 280;

    public override void _Ready()
    {
        image = Image.CreateEmpty(width, height, false, Image.Format.Rgb8);
        image.Fill(Color.Color8(0, 0, 0));

        wasTextureChanged = true;
    }

    public override void _Process(double delta)
    {
        if (!wasTextureChanged)
        {
            return;
        }
        UpdateScreen();
        wasTextureChanged = false;
    }

    public void SetPixel(int x, int y, int brightness)
    {
        byte brightness_byte = (byte)brightness;
        Color color = Color.Color8(brightness_byte, brightness_byte, brightness_byte);
        image.SetPixel(x, y, color);

        wasTextureChanged = true;
    }

    public void UpdateScreen()
    {
        Texture2D Texture = ImageTexture.CreateFromImage(image);
        this.Texture = Texture;
    }

}
