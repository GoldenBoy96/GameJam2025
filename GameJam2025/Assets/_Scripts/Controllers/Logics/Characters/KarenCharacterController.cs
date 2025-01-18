using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class KarenCharacterController : MonoBehaviour 
    {
    
    //thêm object pooling ở đây
    ProjectileLightAttack lightAttack; // get from prefab while awake
    [SerializeField] GameObject lightAttackPrefab;

    [SerializeField] float rotateDegree = 5;
    [SerializeField] int numberOfBullet = 3;
    [SerializeField] float startedDegree = 0;

    public float maxChargeTime = 2f; // Max time to charge
    public float chargeMultiplier = 2f; // Scale multiplier for size/power

    private float chargeTime = 0f;
    private bool isCharging = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            chargeTime = 0f;
        }

        // Keep charging while the button is held
        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxChargeTime); // Clamp to max
        }

        // Release the projectile when the button is released
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            //FireProjectile(chargeTime);
            Skill1Projecttile(chargeTime);
            isCharging = false;
        }
        Debug.Log(isCharging);
        Debug.Log(chargeTime);
    }

    public void DemoShootProjectile()
    {
        SpawnProjectile(lightAttack, lightAttackPrefab);
    }
    public void StartSkill2()
    {
        StartCoroutine(Skill2Projecttile());
    }
    public IEnumerator Skill2Projecttile()
    {

        //TODO: add animation, delay for animation to finish
       
        for (int i = -(numberOfBullet / 2); i <= numberOfBullet/2; i++)  // Tạo 3 viên đạn với góc lệch -1, 0, 1
        {
            Debug.Log($"Tia ban ra {i}");
            GameObject bullet = SpawnProjectile(lightAttack, lightAttackPrefab);
            KarenSkill2ProjectileController projectileScript = bullet.GetComponent<KarenSkill2ProjectileController>();

            // Điều chỉnh hướng bay của từng viên đạn
            projectileScript.dir = Quaternion.Euler(0, 0, i * rotateDegree + startedDegree) * projectileScript.dir;
        }
        yield return new WaitForSeconds(0.2f);
    }

    public void Skill1Projecttile(float chargetime)
    {

        //TODO: add animation, delay for animation to finish
        //TODO: make it so that the setting of projectile's length and content is set in the projectile's constructor/class

        GameObject bullet = SpawnProjectile(lightAttack, lightAttackPrefab);
        KarenSkill1ProrjectileController projectileScript = bullet.GetComponent<KarenSkill1ProrjectileController>();

        float chargeRatio = chargeTime / maxChargeTime; // 0 to 1
        float size = 1f + chargeRatio * (chargeMultiplier - 1f); // Scales from 1x to chargeMultiplier

        projectileScript.spriteRenderer.transform.localScale = new Vector3(1, 1, size);

    }

    public void Skill1Projecttile(float chargetime)
    {

        //TODO: add animation, delay for animation to finish
        //TODO: make it so that the setting of projectile's length and content is set in the projectile's constructor/class

        GameObject bullet = SpawnProjectile(lightAttack, lightAttackPrefab);
        KarenSkill1ProrjectileController projectileScript = bullet.GetComponent<KarenSkill1ProrjectileController>();

        float chargeRatio = chargeTime / maxChargeTime; // 0 to 1
        float size = 1f + chargeRatio * (chargeMultiplier - 1f); // Scales from 1x to chargeMultiplier

        projectileScript.spriteRenderer.transform.localScale = new Vector3(1, 1, size);

    }

    protected GameObject SpawnProjectile<T>(T projectileData, GameObject projectilePrefab)
    {
        //thêm chỗ gắn data cho projectile
        //DummyProjectileController projectileController = GetComponent<DummyProjectileController>();
        //projectileController.projectile = projectileData;
        GameObject result = Instantiate(projectilePrefab, transform.parent);
        return result;
    }

    }
