using Live2D.Cubism.Core;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewModelData", menuName = "Controller/ModelData")]
public class ModelData : ScriptableObject
{
    public string modelName;
    public Texture2D modelAvatar;
    public CubismModel modelPrefab;

    public List<CursorData> cursorList;
    public CursorData starterCursor;
    
    public InteractListById interactionSet;
    public AnimationClip starterMotion;
    public int starterExpression;
    public float modelScreenFill;
    public float topScreenMarginPercent;
}
