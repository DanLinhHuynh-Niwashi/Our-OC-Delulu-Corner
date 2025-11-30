using UnityEngine;
using UnityEngine.Events;

public class InteractionEventListener : MonoBehaviour
{
    [SerializeField] private InteractionDataEventChannel eventChannel;
    [SerializeField] private UnityEvent<InteractActionData> response;

    private void OnEnable()
    {
        eventChannel.OnEventRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        eventChannel.OnEventRaised -= OnEventRaised;
    }

    private void OnEventRaised(InteractActionData data)
    {
        response?.Invoke(data);
    }
}
