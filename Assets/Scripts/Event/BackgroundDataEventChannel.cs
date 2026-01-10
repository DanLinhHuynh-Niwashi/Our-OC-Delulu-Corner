using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Background Event Channel")]
public class BackgroundDataEventChannel : ScriptableObject
{
    public event Action<BackgroundData> OnEventRaised;

    public void RaiseEvent(BackgroundData data)
    {
        OnEventRaised?.Invoke(data);
    }
}
