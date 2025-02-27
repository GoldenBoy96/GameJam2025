using Unity.VisualScripting;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public static GameObject ShootProjectile(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
    {
        return PoolingHelper.SpawnObject(projectilePrefab, position, rotation);
    }
    public static BaseProjectileController ShootProjectile(GameObject projectilePrefab, Vector3 position, Quaternion rotation, BaseCharacterController owner)
    {
        BaseProjectileController projectile =  PoolingHelper.SpawnObject(projectilePrefab, position, rotation).GetComponent<BaseProjectileController>();
        projectile.SetOwner(owner);
        Debug.Log(projectile.Owner);
        return projectile;
    }
}
