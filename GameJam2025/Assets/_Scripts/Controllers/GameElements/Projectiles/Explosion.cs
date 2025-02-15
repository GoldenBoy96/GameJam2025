using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] BaseCharacterController owner;


    public void SetOwner(BaseCharacterController owner)
    {
        owner = gameObject.AddComponent<BaseCharacterController>();
    }
    private void Start()
    {
        StartCoroutine(WaitToDestroy());
        AudioManager.Instance.PlayAudio(AudioConstants.EXPLOSION_SOUND);
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<ICanBeDamage>(out var target))
        {
            BaseCharacterController targetController = collision.gameObject.GetComponent<BaseCharacterController>();
            target.GetDamage();
        }
    }
}
