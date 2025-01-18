using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
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

    public void DemoShootProjectile()
    {
        SpawnProjectile(lightAttack, princessSkill1Prefab);
    }
    public void StartSkill1()
    {
        StartCoroutine(Skill1Projecttile());
    }
    public IEnumerator Skill1Projecttile()
    {

        //TODO: add animation, delay for animation to finish
        for (int i = 0; i <numberOfBullet; i++)
        {
            Debug.Log($"Tia {i} ban ra");
            GameObject bullet = SpawnProjectile(lightAttack, princessSkill1Prefab);
            PrincessSkill1ProjecttileController projectileScript = bullet.GetComponent<PrincessSkill1ProjecttileController>();
            projectileScript.dir = Quaternion.Euler(0, 0, i* rotateDegree + startedDegree) * projectileScript.dir;
            yield return new WaitForSeconds(fireRate);
        }
    }

    protected GameObject SpawnProjectile<T>(T projectileData, GameObject projectilePrefab)
    {
        //thêm chỗ gắn data cho projectile
        //DummyProjectileController projectileController = GetComponent<DummyProjectileController>();
        //projectileController.projectile = projectileData;
        GameObject result = Instantiate(projectilePrefab, transform.parent);
        return result;
    }

    protected override void UpdateStateAlive()
    {
        base.UpdateStateAlive();
        if (Input.GetKey(keySkill1))
        {
            StartSkill1();
        }
    }
}
