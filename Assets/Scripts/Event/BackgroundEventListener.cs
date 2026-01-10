using UnityEngine;
using UnityEngine.Events;

public class BackgroundEventListener : MonoBehaviour
{
    [SerializeField] private BackgroundDataEventChannel eventChannel;
    [SerializeField] private UnityEvent<BackgroundData> response;

    private void OnEnable()
    {
        eventChannel.OnEventRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        eventChannel.OnEventRaised -= OnEventRaised;
    }

    private void OnEventRaised(BackgroundData data)
    {
        response?.Invoke(data);
    }
}
