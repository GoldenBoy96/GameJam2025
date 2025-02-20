using UnityEngine;

public class BaseProjectileController : MonoBehaviour
{
    [SerializeField] BaseCharacterController owner;


    public void SetOwner(BaseCharacterController owner)
    {
        this.owner = owner;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<ICanBeDamage>(out var target))
        {
            BaseCharacterController targetController = collision.gameObject.GetComponent<BaseCharacterController>();
            if (owner != targetController)
            {
                target.GetDamage();

                ReturnProjectileToPool();
            }
        }
    }

    public void TriggerFromChildren(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
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

    public virtual void ReturnProjectileToPool()
    {
        PoolingHelper.ReturnObjectToPool(gameObject);
    }
}
