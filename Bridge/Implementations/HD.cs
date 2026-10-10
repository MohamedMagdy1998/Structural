using Bridge.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Implementations;

public class HD : IVideoQuality
{
    public void Render()
    {
        Console.WriteLine("Rendering HD video ");
    }
}
