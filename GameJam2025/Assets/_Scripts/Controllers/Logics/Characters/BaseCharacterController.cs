using System;
using System.Collections;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour, ICanBeDamage
{
    [Header("Self Object")]
    [SerializeField] GameObject bubble;
    [SerializeField] GameObject skillOutput;
    [SerializeField] GameObject projectileSpawnPoint;
    [SerializeField] GameObject characterHolder;
    [SerializeField] protected Animator animator;

    //thêm object pooling cho đạn ở đây

    [SerializeField] protected float speed = 5;
    [SerializeField] protected float maxSpeed = 5;
    [SerializeField] protected float waterImpact = 0.002f; //TO DO: Chuyển cái này sang game manager
    [SerializeField] protected float rotateSpeed = 100;

    //stats
    [SerializeField] public float oxigen;
    [SerializeField] public float stamina;
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

    //constants
    public bool isAttackAble = true;
    public float lightAttackCoolDown = 1f;
    public float lightAttackCoolDownCurrent = 1f;

    protected Rigidbody2D rb;

    private void Awake()
    {
        SetUpInput(); isAttackAble = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SwitchToState(PlayerState.Alive);
        StartCoroutine(RecoverStamina());
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

        //Debug.Log(currentState + " | " + rb.totalForce + " | " + rb.linearVelocity);
    }

    private IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(1f);
        if (stamina < 100)
        {
            stamina++;
        }
        else
        {
            stamina = 100;
        }
        StartCoroutine(RecoverStamina());
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
        BaseProjectileController projectile = result.GetComponent<BaseProjectileController>();
        projectile.SetOwner(this);
        //result.transform.position = projectileSpawnPoint.transform.position;
        //result.transform.eulerAngles = skillOutput.transform.eulerAngles;
        result.transform.SetLocalPositionAndRotation(projectileSpawnPoint.transform.position, skillOutput.transform.localRotation);
        //Debug.Log(skillOutput.transform.eulerAngles + " | " + result.transform.eulerAngles);
        animator.Play(AnimationConstants.Attack);
        return result;
    }

    public virtual void DoLightAttack()
    {
        if (!UseStamina(5)) return;
        //Debug.Log(canUseSkill);
        //if (!canUseSkill) { return; }
        ProjectileLightAttack projectileLightAttack = new ProjectileLightAttack();
        GameObject bullet = SpawnProjectile(projectileLightAttack, lightAttackPrefab);
        AudioManager.Instance.PlayAudio(AudioConstants.BUBBLE_BULLET);
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
                AudioManager.Instance.StopAudio(AudioConstants.BUBBLE_SHORTGUN);
                break;
            case PlayerState.Choking:
                if (chokeToDeadCoroutine == null)
                {
                    canUseSkill = false;
                    canUsePumb = true;
                    chokeToDeadCoroutine = StartCoroutine(WaitChokeToDead());
                    bubble.SetActive(false);
                    animator.Play(AnimationConstants.Choke);
                    AudioManager.Instance.PlayAudio(AudioConstants.SHIELD_BROKEN);
                    AudioManager.Instance.PlayAudio(AudioConstants.BUBBLE_SHORTGUN);
                }
                break;
            case PlayerState.Dead:
                canUseSkill = false;
                canUsePumb = false;
                bubble.SetActive(false);
                Observer.Notify(ObserverConstants.PLAYER_DEAD, playerPosition);
                animator.Play(AnimationConstants.Dead);
                characterHolder.transform.localPosition = new Vector3(0, -0.2f, 0);
                characterHolder.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                AudioManager.Instance.StopAudio(AudioConstants.BUBBLE_SHORTGUN);
                AudioManager.Instance.PlayAudio(AudioConstants.HIT);
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
        if (Math.Abs(rb.linearVelocityY) < maxSpeed)
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
        //TODO: add cooldown?
        //Spagheti, use GetKeyDown
        //if (Input.GetKey(keyLightAttack))
        //{
        //    DoLightAttack();
        //}




        characterHolder.transform.localPosition = Vector3.Lerp(characterHolder.transform.localPosition, new Vector3(rb.linearVelocity.x / 10, rb.linearVelocity.y / 10), 5 * Time.deltaTime);
        //float z = (float)Math.Sqrt(rb.linearVelocity.x * rb.linearVelocity.x + rb.linearVelocity.y * rb.linearVelocity.y);
        //characterHolder.transform.Rotate(new Vector3(0, 0, z));
        //if (rb.linearVelocity != Vector2.zero)
        //{
        //    characterHolder.transform.rotation = Quaternion.Slerp(characterHolder.transform.rotation, Quaternion.LookRotation(rb.linearVelocity), Time.deltaTime * 40f);
        //    characterHolder.transform.rotation = Quaternion.Euler(0, 0, characterHolder.transform.rotation.z);
        //}
        //transform.position = Vector3.Lerp(transform.position, myTargetPosition.position, speed * Time.deltaTime);

        if (horizontalInput == 0 && verticalInput == 0)
        {
            DoWaterEffect();
            try
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != AnimationConstants.Idle
                    && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != AnimationConstants.Attack)
                {
                    animator.Play(AnimationConstants.Idle);
                    characterHolder.transform.rotation = Quaternion.Euler(Vector2.zero);
                }
            }
            catch { }
        }
        else
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != AnimationConstants.Swim)
        {
            animator.Play(AnimationConstants.Swim);
            if (horizontalInput > 0)
            {
                characterHolder.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -30));
            }
            else
            {
                characterHolder.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 30));
            }
        }

        DoAttackCheck();
    }

    protected virtual void DoAttackCheck()
    {
        if (!isAttackAble) { return; }


        if (lightAttackCoolDownCurrent <= 0)
        {
            if (Input.GetKeyDown(keyLightAttack))
            {
                DoLightAttack();
                lightAttackCoolDownCurrent = lightAttackCoolDown;
                StartCoroutine(TemporaryDisableAttack());
            }
        }
        else
        {
            lightAttackCoolDownCurrent -= 1 / 60f;
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
            AudioManager.Instance.PlayAudio(AudioConstants.BUMP);
        }
        if (oxigen <= 0)
        {
            SwitchToState(PlayerState.Dead);
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

    public void SpawnRight()
    {
        characterHolder.GetComponent<SpriteRenderer>().flipX = false;
        skillOutput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        playerPosition = PlayerPosition.Right;
        SetUpInput();
    }
    public IEnumerator TemporaryDisableAttack()
    {
        isAttackAble = false;
        yield return new WaitForSeconds(1f);
        isAttackAble = true;
    }

    public bool IsEnoughStamina(float amount)
    {
        if (stamina >= amount)
        {
            return true;
        }

        return false;
    }

    public bool UseStamina(float amount)
    {
        if (IsEnoughStamina(amount))
        {
            stamina -= amount; return true;
        }
        return false;
    }
}
