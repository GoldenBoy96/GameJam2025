using System.Collections;
using UnityEngine;

public class LightAttackProjectileController : BaseProjectileController
{

    public Vector3 dir = Vector3.right;
    private void Start()
    {
        StartCoroutine(WaitToDestroy());
    }
    void Update()
    {
        transform.Translate(dir * 10 * Time.deltaTime);

    }
    IEnumerator WaitToDestroy()
    {
        //TODO: Chinh thoi gian mem trong model
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
