using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
//TODO: Sua lai sang ke thua baseCharaterProjectile
public class PrincessCharacaterController : BaseCharacterController
{

    //thêm object pooling ở đây
    ProjectileLightAttack lightAttack; // get from prefab while awake
    [SerializeField] GameObject princessSkill1Prefab;


    [SerializeField] float fireRate = 0.2f;
    [SerializeField] float rotateDegree = 5;
    [SerializeField] int numberOfBullet = 5;
    [SerializeField] float startedDegree = 0;
    [SerializeField] float skill1Cooldown = 10;
    [SerializeField] float skill1CurrentCooldown = 10;
    [SerializeField] float skill2Cooldown = 15;
    [SerializeField] float skill2CurrentCooldown = 15;
    [SerializeField] bool skill2IsActive = false;

    public void StartSkill1()
    {
        if (!UseStamina(20)) return;
        StartCoroutine(Skill1Projecttile());
    }
    public IEnumerator Skill1Projecttile()
    {
        int random = Random.Range(0, 2);
        if (random == 0) random = -1;
        //TODO: add animation, delay for animation to finish
        for (int i = 0; i < numberOfBullet; i++)
        {
            //Debug.Log($"Tia {i} ban ra");
            GameObject bullet = SpawnProjectile(lightAttack, princessSkill1Prefab);
            PrincessSkill1ProjecttileController projectileScript = bullet.GetComponent<PrincessSkill1ProjecttileController>();
            projectileScript.Dir = Quaternion.Euler(0, 0, i * rotateDegree * random + startedDegree) * projectileScript.Dir;
            yield return new WaitForSeconds(fireRate);
        }
    }


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
                StartSkill1();
                skill1CurrentCooldown = skill1Cooldown;
                StartCoroutine(TemporaryDisableAttack());
            }
        }
        else
        {
            skill1CurrentCooldown -= 1 / 60f;
        }

        if (skill2CurrentCooldown <= 0)
        {
            if (Input.GetKeyDown(keySkill2))
            {
                StartSkill2();
                skill2CurrentCooldown = skill2Cooldown;
            }
        }
        else
        {
            skill2CurrentCooldown -= 1 / 60f;
        }

        if (skill2IsActive)
        {
            horizontalInput = 0;
            verticalInput = 0;
            if (Input.GetKey(keyLeft))
            {
                horizontalInput = -1;
            }
            if (Input.GetKey(keyRight))
            {
                horizontalInput = 1;
            }
            if (Input.GetKey(keyUp))
            {
                verticalInput = 1;
            }
            if (Input.GetKey(keyDown))
            {
                verticalInput = -1;
            }
            if (Math.Abs(rb.linearVelocityX) < maxSpeed * 2f)
            {
                rb.AddForce(new Vector2(horizontalInput, 0) * speed * 2);
            }
            if (Math.Abs(rb.linearVelocityY) < maxSpeed * 2f)
            {
                rb.AddForce(new Vector2(0, verticalInput) * speed * 2);
            }

        }
    }

    public void StartSkill2()
    {
        if (!UseStamina(30)) return;
        skill2IsActive = true;
        StartCoroutine(CountDownSkill2());
    }

    IEnumerator CountDownSkill2()
    {
        yield return new WaitForSeconds(5);
        skill2CurrentCooldown = skill2Cooldown;
        skill2IsActive = false;
    }
}
