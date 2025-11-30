using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Interaction Event Channel")]
public class InteractionDataEventChannel : ScriptableObject
{
    public event Action<InteractActionData> OnEventRaised;

    public void RaiseEvent(InteractActionData data)
    {
        OnEventRaised?.Invoke(data);
    }
}
