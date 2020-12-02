using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class FightSettings : ScriptableObject
{
    [Header("Map")]
    public string mapSelection;
    [Header("Players")]
    public List<string> selectedCharacters;
    public List<int> selectedSkins;
    public int nPlayersCanJoin;
    [Header("Items")]
    public List<string> selectedItems;
    [Header("Type")]
    public string type;
    public int winFactorQ;
    [Header("StageChoice")]
    public string stageChoice;
    [Header("Colors/Textures/Materials")]
    public Color32[] colors;
}
