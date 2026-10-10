using Bridge.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Implementations;

public class YoutubeProvider : IVideoProvider
{
    private readonly IVideoQuality Quality;

    public YoutubeProvider(IVideoQuality quality)
    {
        Quality = quality;
    }
    public void PlayVideo(string videoId)
    {
        Console.WriteLine("Playing YouTube video: " + videoId);
        Quality.Render();
    }
}
