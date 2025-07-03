using Godot;

public partial class ComputerScreen : TextureRect
{

    Image image;
    int width = 352;
    int height = 280;

    public override void _Ready()
    {
        image = Image.CreateEmpty(width, height, false, Image.Format.Rgb8);
        image.Fill(Color.Color8(0, 0, 0));

        UpdateScreen();
    }

    public void SetPixel(int x, int y, int brightness)
    {
        byte brightness_byte = (byte)brightness;
        Color color = Color.Color8(brightness_byte, brightness_byte, brightness_byte);
        image.SetPixel(x, y, color);

        UpdateScreen();
    }

    public void UpdateScreen()
    {
        Texture2D Texture = ImageTexture.CreateFromImage(image);
        this.Texture = Texture;
    }

}
