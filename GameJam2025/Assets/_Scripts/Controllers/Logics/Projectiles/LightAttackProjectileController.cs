using UnityEngine;

public class LightAttackProjectileController : BaseProjectileController
{
    [SerializeField] float speed;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        StartFlying();
    }

    private void StartFlying()
    {
        rb.AddForce(Vector3.right * speed);
    }
}
