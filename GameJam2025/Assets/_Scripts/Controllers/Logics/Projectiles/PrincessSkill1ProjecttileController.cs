using System.Collections;
using UnityEngine;
public class PrincessSkill1ProjecttileController : BaseProjectileController
{
    public Vector3 dir = Vector3.right;
    private void Start()
    {
        StartCoroutine(WaitToDestroy());
    }
    void Update()
    {
        //dir = transform.rotation * new Vector3(1, 1, 0);
        transform.Translate(dir * 10 * Time.deltaTime);
        //transform.Translate(transform.rotation.normalized * Vector3.one);

    }
    IEnumerator WaitToDestroy()
    {
        //TODO: Chinh thoi gian mem trong model
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
