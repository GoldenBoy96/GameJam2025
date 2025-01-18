using System.Collections;
using UnityEngine;

public class LightAttackProjectileController : BaseProjectileController
{

    [SerializeField]private Vector3 dir = new Vector3(1, 0, 0);
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
