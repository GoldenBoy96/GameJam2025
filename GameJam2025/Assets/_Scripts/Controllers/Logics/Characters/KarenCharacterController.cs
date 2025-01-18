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

    protected GameObject SpawnProjectile<T>(T projectileData, GameObject projectilePrefab)
    {
        //thêm chỗ gắn data cho projectile
        //DummyProjectileController projectileController = GetComponent<DummyProjectileController>();
        //projectileController.projectile = projectileData;
        GameObject result = Instantiate(projectilePrefab, transform.parent);
        return result;
    }

    }
