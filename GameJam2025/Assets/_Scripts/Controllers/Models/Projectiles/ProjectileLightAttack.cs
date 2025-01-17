using UnityEngine;

public class ProjectileLightAttack : MonoBehaviour
{
    private Vector3 dir = Vector3.left;
    void Update()
    {
        transform.Translate(10 * Time.deltaTime * dir);

    }
}
