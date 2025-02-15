using UnityEngine;

public class AnhInteract : MonoBehaviour, IInterable
{
    [SerializeField] private float CooldownSpaghetti = 5f;
    private float _currentCooldown;

    public float Cooldown => throw new System.NotImplementedException();

    private void FixedUpdate()
    {
        _currentCooldown -= Time.deltaTime;
    }
    public void Interact()
    {
        if (_currentCooldown > 0)
        {
            return;
        }

        Debug.Log($"Anh Interact {gameObject.name}");
        _currentCooldown = CooldownSpaghetti;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Interact();
    }
}
