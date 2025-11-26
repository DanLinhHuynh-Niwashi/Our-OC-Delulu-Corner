using Live2D.Cubism.Core;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkinSet", menuName = "Controller/Skin/SkinSet")]
public class SkinSet : ScriptableObject
{
    public Sprite buttonSelect;
    public Sprite buttonDeselect;
    public List<SkinData> skinList;
}

