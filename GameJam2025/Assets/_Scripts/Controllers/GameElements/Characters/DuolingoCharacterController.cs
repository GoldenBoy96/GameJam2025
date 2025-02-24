using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuolingoCharacterController : BaseCharacterController
{
    [Header("Prefab")]
    [SerializeField] GameObject skill1ProjectilePrefab;
    [SerializeField] GameObject skill2ProjectilePrefab;
    [SerializeField] GameObject skill3ProjectilePrefab;


    [Header("Skill 1 stats")]
    [SerializeField] float skill1ChargeTime1From0;
    [SerializeField] float skill1ChargeTime2From0;
    [SerializeField] float skill1ChargeTime3From0;
    [SerializeField] float staminaConsumePerSecond;

    [SerializeField] float currentSkill1ChargeTime = 0;
    bool isEnoughStamina = true;


    protected override void SetUpSkill()
    {
        base.SetUpSkill();
        var skill1SortTmp = new List<float>() { skill1ChargeTime1From0, skill1ChargeTime2From0, skill1ChargeTime3From0 };
        skill1ChargeTime1From0 = skill1SortTmp[0];
        skill1ChargeTime2From0 = skill1SortTmp[1];
        skill1ChargeTime3From0 = skill1SortTmp[2];


        StartCoroutine(StartCooldownSkill1());
        Observer.Notify(skill1AttackString);

    }
    protected void DoSkill1Check()
    {
        if (!isAbleToUseSkill1) return;
        if (Input.GetKey(keySkill1))
        {
            isAttackAble = false;
            currentSkill1ChargeTime += Time.deltaTime;

            //Charge animation
            if (currentSkill1ChargeTime < skill1ChargeTime1From0)
            {
            }
            else if (currentSkill1ChargeTime >= skill1ChargeTime1From0 && currentSkill1ChargeTime <= skill1ChargeTime2From0)
            {
            }
            else if (currentSkill1ChargeTime >= skill1ChargeTime2From0 && currentSkill1ChargeTime <= skill1ChargeTime3From0)
            {
            }
            else if (currentSkill1ChargeTime >= skill1ChargeTime3From0)
            {
            }

            isEnoughStamina = UseStamina(staminaConsumePerSecond * Time.deltaTime);
        }
        else if (Input.GetKeyUp(keySkill1) || !isEnoughStamina)
        {
            isAttackAble = true;

            //Code bắn ở đây
            if (currentSkill1ChargeTime >= skill1ChargeTime1From0)
            {
                GameObject bullet = SpawnProjectile(skill1ProjectilePrefab, skill1ProjectilePrefab);
                DuolingoSkill1ProrjectileController projectileScript = bullet.GetComponent<DuolingoSkill1ProrjectileController>();
                if (currentSkill1ChargeTime >= skill1ChargeTime1From0 && currentSkill1ChargeTime <= skill1ChargeTime2From0)
                {
                    projectileScript.SetLevel(0);
                }
                else if (currentSkill1ChargeTime >= skill1ChargeTime2From0 && currentSkill1ChargeTime <= skill1ChargeTime3From0)
                {
                    projectileScript.SetLevel(1);
                }
                else if (currentSkill1ChargeTime >= skill1ChargeTime3From0)
                {
                    projectileScript.SetLevel(2);
                }
                StartCoroutine(StartCooldownSkill1());
            }
            currentSkill1ChargeTime = 0;
        }

    }

    public override void DoLightAttack()
    {
        base.DoLightAttack();
        AudioManager.Instance.PlayAudio(AudioConstants.DUOLINGO_RIGHT);
    }

    protected IEnumerator StartCooldownSkill1()
    {
        Debug.Log("StartCooldownSkill1: " + skill1AttackString);
        isAbleToUseSkill1 = false;
        Observer.Notify(skill1AttackString);
        yield return new WaitForSeconds(skill1AttackCoolDown);
        isAbleToUseSkill1 = true;
        Debug.Log("Skill 1 ready!");
    }

    #region State Machine
    protected override void UpdateStateAlive()
    {
        base.UpdateStateAlive();

        DoSkill1Check();
    }
    #endregion

    //float skill1RateOfCharging = 1 / 60f;
    //[SerializeField] float startedDegree = 0;

    //public float maxChargeTime = 10f; // Max time to charge
    //public float chargeMultiplier = 2f; // Scale multiplier for size/power

    //[SerializeField] private float chargeTime = 0f;
    //private bool isCharging = false;
    //[SerializeField] private int numberOfBullet = 3;



    //protected override void DoAttackCheck()
    //{
    //    base.DoAttackCheck();

    //    if (Input.GetKeyDown(keySkill1))
    //    {
    //        isCharging = true;
    //        chargeTime = 0f;
    //    }

    //    // Keep charging while the button is held
    //    if (isCharging && Input.GetKey(keySkill1))
    //    {
    //        if (!UseStamina(1 / 12f))
    //        {
    //            StartSkill1(chargeTime);
    //            isCharging = false;
    //            return;
    //        }
    //        chargeTime += skill1RateOfCharging;
    //        chargeTime = Mathf.Min(chargeTime, maxChargeTime); // Clamp to max
    //        animator.Play(AnimationConstants.Charge);

    //    }

    //    // Release the projectile when the button is released
    //    if (isCharging && Input.GetKeyUp(keySkill1))
    //    {
    //        //FireProjectile(chargeTime);
    //        StartSkill1(chargeTime);
    //        isCharging = false;
    //    }
    //    //Debug.Log(isCharging);
    //    //Debug.Log(chargeTime);
    //}

    //public void StartSkill2()
    //{
    //    StartCoroutine(Skill1Projecttile());
    //}



    //public IEnumerator Skill1Projecttile()
    //{

    //    //TODO: add animation, delay for animation to finish

    //    for (int i = -(numberOfBullet / 2); i <= numberOfBullet / 2; i++)  // Tạo 3 viên đạn với góc lệch -1, 0, 1
    //    {
    //        Debug.Log($"Tia ban ra {i}");
    //        GameObject bullet = SpawnProjectile(new int(), skill2ProjectilePrefab);
    //        DuolingoSkill2ProjectileController projectileScript = bullet.GetComponent<DuolingoSkill2ProjectileController>();

    //        // Điều chỉnh hướng bay của từng viên đạn
    //        //projectileScript.dir = Quaternion.Euler(0, 0, i * rotateDegree + startedDegree) * projectileScript.dir;
    //    }
    //    yield return new WaitForSeconds(0.2f);
    //}

    //public void StartSkill1(float chargetime)
    //{

    //    //TODO: add animation, delay for animation to finish
    //    //TODO: make it so that the setting of projectile's length and content is set in the projectile's constructor/class
    //    if (chargeTime < 2) return;

    //    GameObject bullet = SpawnProjectile(skill1ProjectilePrefab, skill1ProjectilePrefab);
    //    DuolingoSkill1ProrjectileController projectileScript = bullet.GetComponent<DuolingoSkill1ProrjectileController>();

    //    if (chargetime < 5)
    //    {
    //        projectileScript.SetLevel(0);
    //    }
    //    else if (chargetime < 10)
    //    {
    //        projectileScript.SetLevel(1);
    //    }
    //    else
    //    {
    //        projectileScript.SetLevel(2);
    //    }


    //    AudioManager.Instance.PlayAudio(AudioConstants.DUOLINGO_WRONG);
    //    //float chargeRatio = chargeTime / maxChargeTime; // 0 to 1
    //    //float size = 1f + chargeRatio * (chargeMultiplier - 1f); // Scales from 1x to chargeMultiplier

    //    //projectileScript.spriteRenderer.transform.localScale = new Vector3(size, 1, 1);

    //}


    //protected GameObject SpawnProjectile<T>(T projectileData, GameObject projectilePrefab)
    //{
    //    //thêm chỗ gắn data cho projectile
    //    //DummyProjectileController projectileController = GetComponent<DummyProjectileController>();
    //    //projectileController.projectile = projectileData;
    //    GameObject result = Instantiate(projectilePrefab, transform.parent);
    //    return result;
    //}

}
