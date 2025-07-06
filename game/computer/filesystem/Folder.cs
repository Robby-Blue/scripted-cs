using System.Collections.Generic;

public partial class Folder : FileNode
{

    private Dictionary<string, FileNode> children = new();

    public Folder(Path path) : base(path)
    {
    }

    public void AddNode(FileNode node)
    {
        children[node.Path.GetName()] = node;
    }

    public FileNode GetFileNode(Path path)
    {
        string name = path.Parts[0];
        FileNode child = children[name];

        path.Parts.RemoveAt(0);

        if (path.Parts.Count > 0)
        {
            return ((Folder)child).GetFileNode(path);
        }
        else
        {
            return child;
        }
    }

}