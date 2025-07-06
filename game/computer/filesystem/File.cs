using System.Collections.Generic;

public partial class File : FileNode
{

    private List<byte> content;

    public File(Path path, List<byte> content) : base(path)
    {
        this.content = content;
    }

    public List<byte> Read()
    {
        return content;
    }

    public List<byte> Read(int start, int length)
    {
        return content.Slice(start, length);
    }

}