using UnityEngine;

public class StonkCharacterController : BaseCharacterController
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float skill1Cooldown = 10;
    [SerializeField] float skill1CurrentCooldown = 10;
    protected override void UpdateStateAlive()
    {
        base.UpdateStateAlive();
        //TODO: add cooldown?
        DoAttackCheck();

    }
    protected override void DoAttackCheck()
    {
        base.DoAttackCheck();
        if (skill1CurrentCooldown <= 0)
        {
            if (Input.GetKeyDown(keySkill1))
            {
                DoExplosionSkill();
                skill1CurrentCooldown = skill1Cooldown;
                StartCoroutine(TemporaryDisableAttack());
            }
        }
        else
        {
            skill1CurrentCooldown -= 1 / 60f;
        }

    }

    public void DoExplosionSkill()
    {
        GameObject explosionObj = Instantiate(explosionPrefab, transform.parent);
        explosionObj.transform.position = Vector3.zero;
    }

}
