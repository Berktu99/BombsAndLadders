using UnityEngine;
using UnityEngine.Events;

// Type, Event, UnityEventResponse
public abstract class BaseGameEventListener <T, E, UER> : MonoBehaviour, IGameEventListener<T> where E : BaseGameEvent<T> where UER : UnityEvent<T>
{
    [SerializeField] private E gameEvent;
    public E GameEvent { get { return gameEvent; } set { gameEvent = value; } }

    [SerializeField] private UER unityEventResponse;

    public void OnEventRaised(T item)
    {
        if (unityEventResponse != null)
        {
            unityEventResponse.Invoke(item);
        }
    }

    private void OnEnable()
    {
        if (gameEvent == null)
        {
            return;
        }

        GameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (GameEvent == null)
        {
            return;
        }

        GameEvent.RegisterListener(this);
    }
}
