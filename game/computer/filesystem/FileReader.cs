using System.Collections.Generic;

public partial class FileReader
{

    private int position = 0;
    private File file;

    public FileReader(File file)
    {
        this.file = file;
    }

    public List<byte> Read()
    {
        List<byte> content = file.Read();
        position = content.Count;
        return content;
    }

    public List<byte> Read(int length)
    {
        List<byte> content = file.Read(position, length);
        position += length;
        return content;
    }

}