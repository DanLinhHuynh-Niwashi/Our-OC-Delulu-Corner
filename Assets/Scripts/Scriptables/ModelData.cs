using Live2D.Cubism.Core;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewModelData", menuName = "Controller/ModelData")]
public class ModelData : ScriptableObject
{
    public string modelName;
    public Sprite modelImage;
    public float modelImageWidth = 200;
    public Sprite modelAvatar;
    public Sprite backSprite;
    public Sprite logoSprite;

    public Color primaryModulate;
    public Color secondaryModulate;
    
    public CubismModel modelPrefab;

    public List<CursorData> cursorList;
    public CursorData starterCursor;

    public List<BackgroundData> backgroundList;
    public BackgroundData starterBackground;

    public SkinSet skinListHair;
    public SkinSet skinListTop;
    public SkinSet skinListBot;

    public InteractListById interactionSet;
    public AnimationClip starterMotion;
    public int starterExpression;
    public float modelScreenFill;
    public float topScreenMarginPercent;
}
