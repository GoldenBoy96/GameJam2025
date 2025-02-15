using UnityEngine;

public class Shooting : MonoBehaviour
{
    public static GameObject ShootProjectile(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
    {
        return PoolingHelper.SpawnObject(projectilePrefab, position, rotation);
    }
}
