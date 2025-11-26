using UnityEngine;
using UnityEngine.Events;

public class SkinEventListener : MonoBehaviour
{
    [SerializeField] private SkinDataEventChannel eventChannel;
    [SerializeField] private UnityEvent<SkinData> response;

    private void OnEnable()
    {
        eventChannel.OnEventRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        eventChannel.OnEventRaised -= OnEventRaised;
    }

    private void OnEventRaised(SkinData data)
    {
        response?.Invoke(data);
    }
}
