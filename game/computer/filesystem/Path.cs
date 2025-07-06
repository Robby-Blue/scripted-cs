using System;
using System.Collections.Generic;
using System.Linq;

public partial class Path
{

    public List<string> Parts;

    public Path(string pathString)
    {
        if (pathString.StartsWith("/"))
        {
            pathString = pathString.Substring(1);
        }
        Parts = pathString.Split("/").ToList();
    }

    public string GetName()
    {
        return Parts[Parts.Count - 1];
    }

}