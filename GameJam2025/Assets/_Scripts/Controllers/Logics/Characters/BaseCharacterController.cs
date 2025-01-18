using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour, ICanBeDamage
{
    [Header("Self Object")]
    [SerializeField] GameObject bubble;
    [SerializeField] GameObject skillOutput;
    [SerializeField] GameObject projectileSpawnPoint;

    //thêm object pooling cho đạn ở đây

    [SerializeField] float speed = 5;
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float waterImpact = 0.002f; //TO DO: Chuyển cái này sang game manager
    [SerializeField] protected float rotateSpeed = 50;

    //stats
    [SerializeField] float oxigen;
    [SerializeField] float stamina;
    [SerializeField] float chokeToDeadTime;

    [SerializeField] GameObject lightAttackPrefab;
    [SerializeField] GameObject heavyAttackPrefab;

    //input base on player
    [SerializeField] PlayerPosition playerPosition;
    protected KeyCode keyUp;
    protected KeyCode keyDown;
    protected KeyCode keyLeft;
    protected KeyCode keyRight;
    protected KeyCode keyClockwise;
    protected KeyCode keyCounterClockwise;
    protected KeyCode keyLightAttack;
    protected KeyCode keyHeavyAttack;
    protected KeyCode keySkill1;
    protected KeyCode keySkill2;

    //Moving input manager
    protected int horizontalInput = 0;
    protected int verticalInput = 0;

    //State machine
    [SerializeField] protected PlayerState currentState = PlayerState.Alive;
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
        switch (currentState)
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
        //result.transform.position = projectileSpawnPoint.transform.position;
        //result.transform.eulerAngles = skillOutput.transform.eulerAngles;
        result.transform.SetLocalPositionAndRotation(projectileSpawnPoint.transform.position, skillOutput.transform.localRotation);
        Debug.Log(skillOutput.transform.eulerAngles + " | " + result.transform.eulerAngles);
        return result;
    }

    public void DoLightAttack()
    {
        if (!canUseSkill) { return; }
        ProjectileLightAttack projectileLightAttack = new ProjectileLightAttack();
        GameObject bullet = SpawnProjectile(projectileLightAttack, lightAttackPrefab);
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

    protected virtual void UpdateStateAlive()
    {
        //control movement
        horizontalInput = 0;
        verticalInput = 0;
        if (Input.GetKey(keyLeft))
        {
            horizontalInput = -1;
        }
        if (Input.GetKey(keyRight))
        {
            horizontalInput = 1;
        }
        if (Input.GetKey(keyUp))
        {
            verticalInput = 1;
        }
        if (Input.GetKey(keyDown))
        {
            verticalInput = -1;
        }

        //keep velocity below max speed
        if (Math.Abs(rb.linearVelocityX) < maxSpeed)
        {
            rb.AddForce(new Vector2(horizontalInput, 0) * speed);
        }
        if (Math.Abs(rb.linearVelocityX) < maxSpeed)
        {
            rb.AddForce(new Vector2(0, verticalInput) * speed);
        }

        //control rotate
        if (Input.GetKey(keyClockwise))
        {
            skillOutput.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey(keyCounterClockwise))
        {
            skillOutput.transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
        }

        //control skill
        if (Input.GetKey(keyLightAttack))
        {
            DoLightAttack();
        }

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

    protected virtual void UpdateStateChoke()
    {
        //TO DO: Recover by click button follow random pattern
        if (Input.GetKeyDown(keyUp))
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

    protected virtual void UpdateStateDead()
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
