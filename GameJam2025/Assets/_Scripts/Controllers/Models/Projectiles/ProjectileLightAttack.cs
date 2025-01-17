using UnityEngine;

public class ProjectileLightAttack : MonoBehaviour
{
    private Vector3 dir = Vector3.left;
    void Update()
    {
        transform.Translate(dir * 10 * Time.deltaTime);

        //if (transform.position.x <= -4)
        //{
        //    dir = Vector3.right;
        //}
        //else if (transform.position.x >= 4)
        //{
        //    dir = Vector3.left;
        //}
    }
}
