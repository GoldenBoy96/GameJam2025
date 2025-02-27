using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] float skill1StaminaConsumePerSecond;
    [SerializeField] float currentSkill1ChargeTime = 0;


    [Header("Skill 3 stats")]
    [SerializeField] float skill3Delay;
    [SerializeField] float skill3Spread;
    [SerializeField] GameObject skill3Effect;
    protected bool isDoingSkill3 = false;

    protected bool isEnoughStamina = true;


    protected override void SetUpSkill()
    {
        base.SetUpSkill();
        var skill1SortTmp = new List<float>() { skill1ChargeTime1From0, skill1ChargeTime2From0, skill1ChargeTime3From0 };
        skill1ChargeTime1From0 = skill1SortTmp[0];
        skill1ChargeTime2From0 = skill1SortTmp[1];
        skill1ChargeTime3From0 = skill1SortTmp[2];


        StartCoroutine(StartCooldownSkill1());
        StartCoroutine(StartCooldownSkill2());
        StartCoroutine(StartCooldownSkill3());

        skill3Effect.SetActive(false);

    }
    public override void DoLightAttack()
    {
        base.DoLightAttack();
        AudioManager.Instance.PlayAudio(AudioConstants.DUOLINGO_RIGHT);
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

            isEnoughStamina = UseStamina(skill1StaminaConsumePerSecond * Time.deltaTime);
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
    protected void DoSkill2Check()
    {
        if (!isAbleToUseSkill2) return;

        if (Input.GetKeyDown(keySkill2))
        {
            if (!UseStamina(skill2StaminaConsume)) return;
            //ProjectileLightAttack projectileLightAttack = new ProjectileLightAttack();
            DuolingoSkill2ProjectileController bullet = (DuolingoSkill2ProjectileController)Shooting.ShootProjectile(skill2ProjectilePrefab, GetShootingPosition(), GetShootingDirection(), this);
            bullet.transform.parent = transform;
            bullet.transform.localPosition = Vector3.zero;
            //add effect of skill here
            AudioManager.Instance.PlayAudio(AudioConstants.BUBBLE_BULLET);
            Observer.Notify(skill2AttackString, 1f);
            StartCoroutine(StartCooldownSkill2());
        }
    }

    protected void DoSkill3Check()
    {
        if (!isAbleToUseSkill3) return;

        if (Input.GetKeyDown(keySkill3))
        {
            if (isDoingSkill3)
            {
                //Stop skill 3
                isDoingSkill3 = false;                
                StartCoroutine(StartCooldownSkill3());
                skill3Effect.SetActive(false);
            }
            else
            {
                isDoingSkill3 = true;
                StartCoroutine(DoSkill3());
                skill3Effect.SetActive(true);
            }
        }
    }

    protected IEnumerator DoSkill3()
    {
        if (isDoingSkill3)
        {
            if (!UseStamina(skill3StaminaConsume))
            {
                isDoingSkill3 = false;
                skill3Effect.SetActive(false);
                StartCoroutine(StartCooldownSkill3());
            };
            DuolingoSkill3ProjectileController bullet = (DuolingoSkill3ProjectileController)Shooting.ShootProjectile(skill3ProjectilePrefab, GetShootingPosition(), GetShootingDirection(), this);
            float random = Random.Range(-skill3Spread, skill3Spread);
            //Debug.Log(random);
            bullet.Dir = Quaternion.Euler(0, 0, random) * bullet.Dir;

            AudioManager.Instance.PlayAudio(AudioConstants.DUOLINGO_WRONG);
            yield return new WaitForSeconds(skill3Delay);
            StartCoroutine(DoSkill3());
        }
    }

    protected IEnumerator StartCooldownSkill1()
    {
        isAbleToUseSkill1 = false;
        Observer.Notify(skill1AttackString);
        yield return new WaitForSeconds(skill1AttackCoolDown);
        isAbleToUseSkill1 = true;
        //Debug.Log("Skill 1 ready!");
    }

    protected IEnumerator StartCooldownSkill2()
    {
        isAbleToUseSkill2 = false;
        Observer.Notify(skill2AttackString);
        yield return new WaitForSeconds(skill2AttackCoolDown);
        isAbleToUseSkill2 = true;
        //Debug.Log("Skill 2 ready!");
    }
    protected IEnumerator StartCooldownSkill3()
    {
        isAbleToUseSkill3 = false;
        Observer.Notify(skill3AttackString);
        yield return new WaitForSeconds(skill3AttackCoolDown);
        isAbleToUseSkill3 = true;
        //Debug.Log("Skill 3 ready!");
    }


    #region State Machine
    bool isDoingSkill3Tmp;
    protected override void EnterStateWaiting()
    {
        base.EnterStateWaiting();
        isDoingSkill3Tmp = isDoingSkill3;
        isDoingSkill3 = false;
    }
    protected override void ExitStateWaiting()
    {
        base.ExitStateWaiting();
        isDoingSkill3 = isDoingSkill3Tmp;
    }

    protected override void UpdateStateAlive()
    {
        base.UpdateStateAlive();

        DoSkill1Check();
        DoSkill2Check();
        DoSkill3Check();
    }
    #endregion


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


}
