using UnityEngine;

public class BaseProjectileController : MonoBehaviour
{
    [SerializeField] BaseCharacterController owner;


    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("OnCollisionEnter2D " + collision.gameObject.name);
    //    if (collision.gameObject.TryGetComponent<ICanBeDamage>(out var target))
    //    {
    //        target.GetDamage();

    //        Destroy(gameObject);
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<ICanBeDamage>(out var target))
        {
            BaseCharacterController targetController = collision.gameObject.GetComponent<BaseCharacterController>();
            if (owner != targetController)
            {
                target.GetDamage();

                Destroy(gameObject);
            }
        }
    }
}
