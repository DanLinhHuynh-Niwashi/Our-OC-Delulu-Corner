using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Cursor Event Channel")]
public class CursorDataEventChannel : ScriptableObject
{
    public event Action<CursorData> OnEventRaised;

    public void RaiseEvent(CursorData data)
    {
        OnEventRaised?.Invoke(data);
    }
}
