using System.Collections;
using UnityEngine;

public class DuolingoSkill2ProjectileController : BaseProjectileController
{
    public Vector3 dir = Vector3.left;
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
        yield return new WaitForSeconds(10f);
        ReturnProjectileToPool();
    }

    protected override void Interact(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BaseProjectileController>(out var target))
        {
            target.GetDamage();
            GetDamage();
        }
    }
}
