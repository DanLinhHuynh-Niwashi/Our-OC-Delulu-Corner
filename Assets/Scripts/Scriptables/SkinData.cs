using Live2D.Cubism.Core;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkinData", menuName = "Controller/Skin/SkinData")]
public class SkinData : ScriptableObject
{
    public string skinId;
    public List<string> partsToHide;
    public List<string> partsToShow;
}
