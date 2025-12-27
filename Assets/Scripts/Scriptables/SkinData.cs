using Live2D.Cubism.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkinData", menuName = "Controller/Skin/SkinData")]
public class SkinData : ScriptableObject
{
    public string skinId;
    public string setId;
    public bool isDrivenByParam = false;

    public List<ParamSetting> paramSettings;
    public float fadeTime = 0.5f;
    public List<string> partsToHide;
    public List<string> partsToShow;
}

[Serializable]
public struct ParamSetting
{
    public string ParamName;
    public float Value;
}