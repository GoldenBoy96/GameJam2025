using UnityEngine;
public class DotcomSkillProjectileController : BaseProjectileController
{
    public void PlayAttackEffect()
    {

        Debug.Log("Quả cầu phát nổ!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            PlayAttackEffect();
            Destroy(gameObject); // Huỷ quả cầu sau khi va chạm
        }
    }
}
