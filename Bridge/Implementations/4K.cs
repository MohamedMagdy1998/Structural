using Bridge.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Implementations;

public class _4K : IVideoQuality
{
    public void Render()
    {
        Console.WriteLine("Rendering 4K video ");
    }
}
