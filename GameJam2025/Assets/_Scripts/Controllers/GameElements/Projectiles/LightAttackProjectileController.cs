using System.Collections;
using UnityEngine;

public class LightAttackProjectileController : BaseProjectileController
{

    [SerializeField] private Vector3 dir = new(1, 0, 0);
     void Start()
    {
        StartCoroutine(WaitToDestroy());
    }
    void Update()
    {
        transform.Translate(10 * Time.deltaTime * dir);

    }
    IEnumerator WaitToDestroy()
    {
        //TODO: Chinh thoi gian mem trong model
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
