using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class DotcomCharacterController : BaseCharacterController
{
    //thêm object pooling ở đây
    ProjectileLightAttack lightAttack; // get from prefab while awake
    [SerializeField] GameObject spherePrefab;
    [SerializeField] Transform player;
    [SerializeField] Transform target;

    [SerializeField] float minSize = 1;
    [SerializeField] float maxSize = 5;
    [SerializeField] float amountIncrease = 1;
    [SerializeField] float coolDown = 1;
    [SerializeField] float speed = 5f;
    [SerializeField] float followDistance = 2f;
    [SerializeField] KeyCode growKey = KeyCode.E; // Nút kích hoạt tăng kích thước

    GameObject sphereInstance;
    bool isAttacking = false;
    void Start()
    {
        // Tạo quả cầu và đặt vị trí cạnh người chơi
        sphereInstance = Instantiate(spherePrefab, player.position + Vector3.right * followDistance, Quaternion.identity);
        sphereInstance.transform.localScale = Vector3.one * minSize;
    }

    void Update()
    {
        if (!isAttacking)
        {
            // Quả cầu di chuyển theo người chơi
            Vector3 followPos = player.position + Vector3.right * followDistance;
            sphereInstance.transform.position = Vector3.Lerp(sphereInstance.transform.position, followPos, Time.deltaTime * speed);

            // Tăng kích thước khi nhấn phím đã cấu hình
            if (Input.GetKeyDown(growKey))
            {
                GrowSphere();
            }
        }
    }

    void GrowSphere()
    {
        float newSize = sphereInstance.transform.localScale.x + amountIncrease;
        if (newSize >= maxSize)
        {
            sphereInstance.transform.localScale = Vector3.one * maxSize;
            StartCoroutine(AttackTarget());
        }
        else
        {
            sphereInstance.transform.localScale = Vector3.one * newSize;
        }
    }

    IEnumerator AttackTarget()
    {
        isAttacking = true;

        while (Vector3.Distance(sphereInstance.transform.position, target.position) > 0.1f)
        {
            sphereInstance.transform.position = Vector3.MoveTowards(sphereInstance.transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }

        sphereInstance.transform.localScale = Vector3.one * minSize;
        isAttacking = false;
    }
}

