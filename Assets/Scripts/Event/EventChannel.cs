using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Event Channel")]
public class EventChannel : ScriptableObject
{
    public event Action OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
