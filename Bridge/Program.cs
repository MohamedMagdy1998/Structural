using Bridge.Abstractions;
using Bridge.Implementations;

IVideoProvider videoProvider = new YoutubeProvider(new _4K());
videoProvider.PlayVideo("abc123");

