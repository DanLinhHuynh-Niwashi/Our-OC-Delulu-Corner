using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Skin Event Channel")]
public class SkinDataEventChannel : ScriptableObject
{
    public event Action<SkinData> OnEventRaised;

    public void RaiseEvent(SkinData data)
    {
        OnEventRaised?.Invoke(data);
    }
}
