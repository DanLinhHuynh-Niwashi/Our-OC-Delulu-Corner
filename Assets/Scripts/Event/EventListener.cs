using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    [SerializeField] private EventChannel eventChannel;
    [SerializeField] private UnityEvent response;

    private void OnEnable()
    {
        eventChannel.OnEventRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        eventChannel.OnEventRaised -= OnEventRaised;
    }

    private void OnEventRaised()
    {
        response?.Invoke();
    }
}
