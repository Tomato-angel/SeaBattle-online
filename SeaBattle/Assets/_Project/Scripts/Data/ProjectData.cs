using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ProjectData
{
    public string name { get; set; }
    public string version { get; set; }
    public string description { get; set; }

    public static ProjectData Default
    {
        get => new ProjectData()
        {
            name = "Sea Battle",
            version = "close-beta 0.0.1",
            description = "The sea battle online game."
        };
    }

    public ProjectData() { }

}

