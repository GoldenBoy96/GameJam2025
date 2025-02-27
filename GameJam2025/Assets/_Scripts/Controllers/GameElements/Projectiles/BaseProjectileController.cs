using UnityEngine;

public class BaseProjectileController : MonoBehaviour, ICanBeDamage
{
    [SerializeField] private BaseCharacterController owner;

    public BaseCharacterController Owner { get => owner; }

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
            if (collision.gameObject.TryGetComponent<BaseCharacterController>(out var targetController))
            {
                if (Owner.ToString() != targetController.ToString())
                {
                    Debug.Log(Owner + " | " + target + " | " + targetController);
                    target.GetDamage();
                    ReturnProjectileToPool();
                }

            }
            //BaseCharacterController targetController = collision.gameObject.GetComponent<BaseCharacterController>();
            
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
