using UnityEngine;

public class MinhInteract : MonoBehaviour, IInterable
{
    public float Cooldown { get; } = 0.5f;

    public void Interact()
    {
        Debug.Log($"Minh interaction: {gameObject.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IInterable>(out var target))
        {
            target.Interact();
        }

    }


}
