using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    //thêm object pooling ở đây

    protected void SpawnProjectile<T>(T projectileData, GameObject projectilePrefab)
    {
        //thêm chỗ gắn data cho projectile
        //DummyProjectileController projectileController = GetComponent<DummyProjectileController>();
        //projectileController.projectile = projectileData;
        Instantiate(projectilePrefab, transform.parent);
    }
}
