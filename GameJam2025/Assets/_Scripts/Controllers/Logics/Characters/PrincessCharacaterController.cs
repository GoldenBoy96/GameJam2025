using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
//TODO: Sua lai sang ke thua baseCharaterProjectile
public class PrincessCharacaterController : MonoBehaviour
{
    
    //thêm object pooling ở đây
    ProjectileLightAttack lightAttack; // get from prefab while awake
    [SerializeField] GameObject lightAttackPrefab;


    [SerializeField] float fireRate = 0.2f;
    [SerializeField] float rotateDegree = 5;
    [SerializeField] int numberOfBullet = 5;
    [SerializeField] float startedDegree = 0;

    public void DemoShootProjectile()
    {
        SpawnProjectile(lightAttack, lightAttackPrefab);
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
            GameObject bullet = SpawnProjectile(lightAttack, lightAttackPrefab);
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

}
