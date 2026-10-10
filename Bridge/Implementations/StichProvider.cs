using Bridge.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Implementations;

public class StitchProvider : IVideoProvider
{
    private readonly IVideoQuality Quality;

    public StitchProvider(IVideoQuality quality)
    {
        Quality = quality;
    }
    public void PlayVideo(string videoId)
    {
        Console.WriteLine("Playing Stitch video: " + videoId);
        Quality.Render();
    }
}
