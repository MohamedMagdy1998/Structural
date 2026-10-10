using System;
using System.Collections.Generic;
using System.Text;

namespace Bridge.Abstractions;

public interface IVideoProvider
{
    public void PlayVideo(string videoId);

}
