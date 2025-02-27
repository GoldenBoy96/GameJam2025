using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuolingoSkill3ProjectileController : BaseProjectileController
{
    private Vector3 dir = Vector3.right;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> spriteRandomList;
 
    public Vector3 Dir { get => dir; set => dir = value; }

    private void Start()
    {
        int random = Random.Range(0, spriteRandomList.Count);
        spriteRenderer.sprite = spriteRandomList[random];
        StartCoroutine(WaitToDestroy());
    }
    void Update()
    {
        transform.Translate(Dir * 7 * Time.deltaTime);
        RotateSelf();
    }
    IEnumerator WaitToDestroy()
    {
        //TODO: Chinh thoi gian mem trong model
        yield return new WaitForSeconds(10f);
        ReturnProjectileToPool();
    }


    protected void RotateSelf()
    {
        var random = Random.Range(-180, 180);
        spriteRenderer.transform.Rotate(Vector3.forward * random * Time.deltaTime);
    }
}
