﻿using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour, ICanBeDamage
{
    [Header("Self Object")]
    [SerializeField] GameObject bubble;

    //thêm object pooling cho đạn ở đây

    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float waterImpact = 0.005f; //TO DO: Chuyển cái này sang game manager

    //stats
    [SerializeField] float oxigen;
    [SerializeField] float stamina;
    [SerializeField] float chokeToDeadTime;

    [SerializeField] GameObject lightAttackPrefab;
    [SerializeField] GameObject heavyAttackPrefab;

    //input base on player
    [SerializeField] PlayerPosition playerPosition;
    private KeyCode keyUp;
    private KeyCode keyDown;
    private KeyCode keyLeft;
    private KeyCode keyRight;
    private KeyCode keyClockwise;
    private KeyCode keyCounterClockwise;
    private KeyCode keyLightAttack;
    private KeyCode keyHeavyAttack;
    private KeyCode keySkill1;
    private KeyCode keySkill2;

    //Moving input manager
    protected int horizontalInput = 0;
    protected int verticalInput = 0;

    //State machine
    protected PlayerState currentState = PlayerState.Alive;
    protected bool canUseSkill = false;
    protected bool canUsePumb = false;
    protected int chokeRecoveryNumber = 5;
    protected Coroutine chokeToDeadCoroutine;

    Rigidbody2D rb;

    private void Awake()
    {
        SetUpInput();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SwitchToState(PlayerState.Alive);
    }

    void Update()
    {
        switch(currentState)
        {
            case PlayerState.Alive:
                UpdateStateAlive();
                break;
            case PlayerState.Choking:
                UpdateStateChoke();
                break;
            case PlayerState.Dead:
                UpdateStateDead();
                break;
        }
        DoWaterEffect();
        //Debug.Log(currentState + " | " + rb.totalForce + " | " + rb.linearVelocity);
    }

    private void SetUpInput()
    {
        switch (playerPosition)
        {
            case PlayerPosition.Left:
                keyUp = InputConstants.PLAYER_1_UP;
                keyDown = InputConstants.PLAYER_1_DOWN;
                keyLeft = InputConstants.PLAYER_1_LEFT;
                keyRight = InputConstants.PLAYER_1_RIGHT;
                keyClockwise = InputConstants.PLAYER_1_CLOCKWISE;
                keyCounterClockwise = InputConstants.PLAYER_1_COUNTERCLOCKWISE;
                keyLightAttack = InputConstants.PLAYER_1_LIGHT_ATTACK;
                keyHeavyAttack = InputConstants.PLAYER_1_HEAVY_ATTACK;
                keySkill1 = InputConstants.PLAYER_1_SKILL_1;
                keySkill2 = InputConstants.PLAYER_1_SKILL_2;
                break;
            case PlayerPosition.Right:
                keyUp = InputConstants.PLAYER_2_UP;
                keyDown = InputConstants.PLAYER_2_DOWN;
                keyLeft = InputConstants.PLAYER_2_LEFT;
                keyRight = InputConstants.PLAYER_2_RIGHT;
                keyClockwise = InputConstants.PLAYER_2_CLOCKWISE;
                keyCounterClockwise = InputConstants.PLAYER_2_COUNTERCLOCKWISE;
                keyLightAttack = InputConstants.PLAYER_2_LIGHT_ATTACK;
                keyHeavyAttack = InputConstants.PLAYER_2_HEAVY_ATTACK;
                keySkill1 = InputConstants.PLAYER_2_SKILL_1;
                keySkill2 = InputConstants.PLAYER_2_SKILL_2;
                break;
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

    public void DoLightAttack()
    {
        if (!canUseSkill) { return; }
        ProjectileLightAttack projectileLightAttack = new ProjectileLightAttack();
        GameObject bullet = SpawnProjectile(projectileLightAttack, lightAttackPrefab);
        bullet.transform.position = this.transform.position;
    }

    public void SwitchToState(PlayerState incomingState)
    {
        //Change current state and status that follow the state
        switch (incomingState)
        {
            case PlayerState.Alive:
                canUseSkill = true;
                canUsePumb = false;
                if (chokeToDeadCoroutine != null) StopCoroutine(chokeToDeadCoroutine);
                bubble.SetActive(true);
                break;
            case PlayerState.Choking:
                canUseSkill = false;
                canUsePumb = true;
                chokeToDeadCoroutine = StartCoroutine(WaitChokeToDead());
                bubble.SetActive(false);
                break;
            case PlayerState.Dead:
                canUseSkill = false;
                canUsePumb = false;
                bubble.SetActive(false);
                break;
        }
        currentState = incomingState;
    }

    private void UpdateStateAlive()
    {
        horizontalInput = 0;
        verticalInput = 0;
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
        
    }

    private void DoWaterEffect()
    {
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
    }

    private void UpdateStateChoke()
    {
        //TO DO: Recover by click button follow random pattern
        if (Input.GetKey(keyUp))
        {
            //Subtract a lot oxigen
            oxigen -= 5;
            chokeRecoveryNumber -= 1;
        }
        if (chokeRecoveryNumber <= 0)
        {
            SwitchToState(PlayerState.Alive);
            chokeRecoveryNumber = 5;
        }
    }

    private void UpdateStateDead()
    {

    }
    private IEnumerator WaitChokeToDead()
    {
        yield return new WaitForSeconds(chokeToDeadTime);
        SwitchToState(PlayerState.Dead);
    }

    public void GetDamage()
    {
        SwitchToState(PlayerState.Choking);

    }
}
