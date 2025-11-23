using UnityEngine;
using UnityEngine.Events;

public class CursorEventListener : MonoBehaviour
{
    [SerializeField] private CursorDataEventChannel eventChannel;
    [SerializeField] private UnityEvent<CursorData> response;

    private void OnEnable()
    {
        eventChannel.OnEventRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        eventChannel.OnEventRaised -= OnEventRaised;
    }

    private void OnEventRaised(CursorData data)
    {
        response?.Invoke(data);
    }
}
