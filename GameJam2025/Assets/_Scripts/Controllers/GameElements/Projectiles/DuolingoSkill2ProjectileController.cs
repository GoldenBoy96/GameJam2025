using System.Collections;
using UnityEngine;

public class DuolingoSkill2ProjectileController : BaseProjectileController
{
    public Vector3 dir = Vector3.left;
    [SerializeField] protected float existTime;
    private void Start()
    {
        StartCoroutine(WaitToDestroy());
    }
    void Update()
    {


    }
    IEnumerator WaitToDestroy()
    {
        //TODO: Chinh thoi gian mem trong model
        yield return new WaitForSeconds(existTime);
        ReturnProjectileToPool();
    }

    protected override void Interact(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BaseProjectileController>(out var target))
        {
            if (target.Owner.ToString().Trim() != Owner.ToString().Trim())
            {
                Debug.Log(target.Owner.ToString().Trim() == Owner.ToString().Trim() + " | \n" + target.ToString() + " | \n" + Owner.ToString());
                target.GetDamage();
                GetDamage();

            }
        }
    }
}
