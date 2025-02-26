using UnityEngine;

public class BaseProjectileController : MonoBehaviour, ICanBeDamage
{
    [SerializeField] BaseCharacterController owner;


    public void SetOwner(BaseCharacterController owner)
    {
        this.owner = owner;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
        Interact(collision);
    }

    public void TriggerFromChildren(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
        Interact(collision);
    }

    protected virtual void Interact(Collider2D collision)
    {
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

    public virtual void ReturnProjectileToPool()
    {
        PoolingHelper.ReturnObjectToPool(gameObject);
    }

    public void GetDamage()
    {
        PoolingHelper.ReturnObjectToPool(gameObject);
    }
}
