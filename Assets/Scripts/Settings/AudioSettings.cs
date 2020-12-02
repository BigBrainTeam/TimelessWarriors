using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class AudioSettings : ScriptableObject
{
    [Header("Master")]
    public float masterv;
    [Header("SFX")]
    public float sfxv;
    [Header("Music")]
    public float musicv;
    [Header("Voices")]
    public float voicesv;
}
