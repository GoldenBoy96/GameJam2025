using System;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    //thêm object pooling ở đây

    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float waterImpact = 0.005f;

    [SerializeField] GameObject lightAttackPrefab;
    [SerializeField] GameObject heavyAttackPrefab;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        int horizontalInput = 0;
        int verticalInput = 0;
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1;
        }

        if (Math.Abs(rb.linearVelocityX) < maxSpeed)
        {
            rb.AddForce(new Vector2(horizontalInput, 0) * speed);
        }
        if (Math.Abs(rb.linearVelocityX) < maxSpeed)
        {
            rb.AddForce(new Vector2(0, verticalInput) * speed);
        }

        //rb.AddForce(new Vector2(horizontalInput, verticalInput) * speed);
        if (rb.linearVelocityX > 0)
        {
            rb.linearVelocityX -= waterImpact;
        }
        else if (rb.linearVelocityX < 0)
        {
            rb.linearVelocityX += waterImpact;
        }
        if (rb.linearVelocityY > 0)
        {
            rb.linearVelocityY -= waterImpact;
        }
        else if (rb.linearVelocityY < 0)
        {
            rb.linearVelocityY += waterImpact;
        }
        //Debug.Log(rb.linearVelocity);
        //Vector3 velocity = transform.rotation * new Vector3(horizontalInput * speed, 0, verticalInput * speed);
        //rb.linearVelocity = new Vector2(velocity.x, velocity.y);
    }

    protected GameObject SpawnProjectile<T>(T projectileData, GameObject projectilePrefab)
    {
        //thêm chỗ gắn data cho projectile
        //DummyProjectileController projectileController = GetComponent<DummyProjectileController>();
        //projectileController.projectile = projectileData;
        GameObject result = Instantiate(projectilePrefab, transform.parent);
        return result;
    }

    public void DoLightAttack()
    {
        ProjectileLightAttack projectileLightAttack = new ProjectileLightAttack();
        GameObject bullet = SpawnProjectile(projectileLightAttack,lightAttackPrefab);
        bullet.transform.position = this.transform.position;
    }
}
