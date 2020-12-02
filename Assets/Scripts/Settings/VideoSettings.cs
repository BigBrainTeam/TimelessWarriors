using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class VideoSettings : ScriptableObject
{
    [Header("FullScreen")]
    public bool fullscreen;
    [Header("TextureQuality")]
    public int textureQuality;
    [Header("Antialiasing")]
    public int antialiasing;
    [Header("Resolution")]
    public int resolutionIndex;
}
