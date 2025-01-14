using UnityEngine;

public class MinhInteract : MonoBehaviour, IInterable
{
    public float Cooldown { get; } = 0.5f;

    private void Awake()
    {

        Observer.AddObserver(ObserverConstants.DEMO_OBSERVER, (x) => { Debug.Log($"Test 1"); });
        Observer.AddObserver(ObserverConstants.DEMO_OBSERVER, (x) => { Debug.Log("Test 2"); });
    }

    public void Interact()
    {
        Debug.Log($"Minh interaction: {gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IInterable>(out var target))
        {
            Observer.Notify(ObserverConstants.DEMO_OBSERVER, new int[] { 50, 100, 150 });
        }

    }


}
