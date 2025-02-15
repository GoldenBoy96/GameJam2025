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
    [SerializeField] GameObject characterRenderer;
    [SerializeField] protected Animator animator;

    //thêm object pooling cho đạn ở đây
    [Header("Character Data")]
    [SerializeField] protected string characterName = "Unknown Character";
    [SerializeField] protected float speed = 5;
    [SerializeField] protected float maxSpeed = 5;
    [SerializeField] protected float waterImpact = 0.002f; //TO DO: Chuyển cái này sang game manager
    [SerializeField] protected float rotateSpeed = 100;
    [SerializeField] public float oxigen;
    [SerializeField] public float stamina;
    [SerializeField] float chokeToDeadTime;


    [Header("Gravity")]
    [SerializeField] float bubbleGravity = -0.12f;
    [SerializeField] float noBubbleGravity = 0.5f;


    [SerializeField] GameObject lightAttackPrefab;
    [SerializeField] GameObject heavyAttackPrefab;


    [Header("Character Position and Direction")]
    [SerializeField] PlayerPosition playerPosition = PlayerPosition.Left;
    [SerializeField] PlayerDirection playerDirection = PlayerDirection.Right;

    //input base on player
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

    public string CharacterName { get => characterName; set => characterName = value; }

    private void Awake()
    {
        SetUpInput();
        isAttackAble = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupFlip();
        SwitchToState(PlayerState.Alive);
    }

    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Alive:
                UpdateStateAlive();
                break;
            case PlayerState.Choke:
                UpdateStateChoke();
                break;
            case PlayerState.Dead:
                UpdateStateDead();
                break;
        }

        //Debug.Log(currentState + " | " + rb.totalForce + " | " + rb.linearVelocity);
    }

    public void SetupSecondaryColor()
    {
        if (characterRenderer.TryGetComponent<SpriteRenderer>(out var sprite))
        {
            ColorUtility.TryParseHtmlString(ColorConstants.CHARATER_SECONDARY_COLOR, out Color colorFromHex);
            sprite.color = colorFromHex;
            //Debug.Log(sprite + " | " + ColorConstants.CHARATER_SECONDARY_COLOR + " | " + sprite.color + " | " + colorFromHex);
        }
    }
    private void SetupFlip()
    {
        switch (playerPosition)
        {
            case PlayerPosition.Left:
                Flip(PlayerDirection.Right);
                break;
            case PlayerPosition.Right:
                Flip(PlayerDirection.Left);
                break;
        }
    }
    public void Flip(PlayerDirection direction)
    {
        if (playerDirection == direction) return;
        switch (direction)
        {
            case PlayerDirection.Left:
                characterHolder.transform.localScale = new Vector3(-1, 1, 1);
                break;
            case PlayerDirection.Right:
                characterHolder.transform.localScale = new Vector3(1, 1, 1);
                break;
             
        }
        playerDirection = direction;
        //Debug.Log(playerPosition + " | " + direction + " | " + transform.localScale);
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

    #region State Machine
    public void SwitchToState(PlayerState incomingState)
    {
        switch (currentState)
        {
            case PlayerState.Alive:
                ExitStateAlive();
                break;
            case PlayerState.Choke:
                ExitStateChoke();
                break;
            case PlayerState.Dead:
                ExitStateDead();
                break;
        }
        switch (incomingState)
        {
            case PlayerState.Alive:
                EnterStateAlive();
                break;
            case PlayerState.Choke:
                EnterStateChoke();
                break;
            case PlayerState.Dead:
                EnterStateDead();
                break;
        }
        currentState = incomingState;
    }

    #region State Alive
    protected virtual void EnterStateAlive()
    {
        canUseSkill = true;
        canUsePumb = false;
        rb.gravityScale = bubbleGravity;
        if (chokeToDeadCoroutine != null) StopCoroutine(chokeToDeadCoroutine);
        bubble.SetActive(true);
        AudioManager.Instance.StopAudio(AudioConstants.BUBBLE_SHORTGUN);

        StartCoroutine(RecoverStamina());
    }

    protected virtual void UpdateStateAlive()
    {//control movement
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

        characterRenderer.transform.localPosition = Vector3.Lerp(characterRenderer.transform.localPosition, new Vector3(rb.linearVelocity.x / 10, rb.linearVelocity.y / 10), 5 * Time.deltaTime);
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
                    characterRenderer.transform.rotation = Quaternion.Euler(Vector2.zero);
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
                characterRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -30));
            }
            else
            {
                characterRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 30));
            }
        }

        DoAttackCheck();
    }
    protected virtual void ExitStateAlive()
    {
        StopAllCoroutines();
    }
    #endregion
    #region State Choke
    protected virtual void EnterStateChoke()
    {
        if (chokeToDeadCoroutine == null)
        {
            chokeToDeadCoroutine = StartCoroutine(WaitChokeToDead());
            canUseSkill = false;
            canUsePumb = true;
            rb.gravityScale = noBubbleGravity;
            //chokeToDeadCoroutine = StartCoroutine(WaitChokeToDead());
            bubble.SetActive(false);
            animator.Play(AnimationConstants.Choke);
            AudioManager.Instance.PlayAudio(AudioConstants.SHIELD_BROKEN);
            AudioManager.Instance.PlayAudio(AudioConstants.BUBBLE_SHORTGUN);
        }
    }
    protected virtual void UpdateStateChoke()
    {

    }
    protected virtual void ExitStateChoke()
    {
        StopAllCoroutines();
    }
    #endregion
    #region State Dead
    protected virtual void EnterStateDead()
    {
        canUseSkill = false;
        canUsePumb = false;
        rb.gravityScale = noBubbleGravity;
        bubble.SetActive(false);
        Observer.Notify(ObserverConstants.PLAYER_DEAD, playerPosition);
        animator.Play(AnimationConstants.Dead);
        characterRenderer.transform.localPosition = new Vector3(0, -0.2f, 0);
        characterRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        AudioManager.Instance.StopAudio(AudioConstants.BUBBLE_SHORTGUN);
        AudioManager.Instance.PlayAudio(AudioConstants.HIT);
    }
    protected virtual void UpdateStateDead()
    {
    }
    protected virtual void ExitStateDead()
    {
        StopAllCoroutines();
    }
    #endregion

    private IEnumerator WaitChokeToDead()
    {
        yield return new WaitForSeconds(chokeToDeadTime);
        SwitchToState(PlayerState.Dead);
    }

    #region Sprite Controller
    public void SwitchToSecondaryColor()
    {

    }
    public void FlipLeft()
    {

    }
    public void FlipRight()
    {

    }
    #endregion
    #endregion
    #region Attack
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

        if (playerDirection == PlayerDirection.Left)
        {
            result.transform.localRotation = Quaternion.Euler(
            new Vector3(
                result.transform.localRotation.eulerAngles.x,
                result.transform.localRotation.eulerAngles.y,
                result.transform.localRotation.eulerAngles.z * characterRenderer.transform.localScale.x + 180
                ));

        }
        

        Debug.Log(skillOutput.transform.eulerAngles + " | " + result.transform.eulerAngles);
        animator.Play(AnimationConstants.Attack);
        return result;
    }

    protected virtual Vector3 GetShootingPosition()
    {
        return projectileSpawnPoint.transform.position;
    }

    protected virtual Quaternion GetShootingDirection()
    {
        Quaternion result = skillOutput.transform.localRotation;
        if (playerDirection == PlayerDirection.Left)
        {
            result = Quaternion.Euler(
            new Vector3(
                result.eulerAngles.x,
                result.eulerAngles.y,
                result.eulerAngles.z * characterRenderer.transform.localScale.x + 180
                ));
        }
        return result;
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
    public IEnumerator TemporaryDisableAttack()
    {
        isAttackAble = false;
        yield return new WaitForSeconds(1f);
        isAttackAble = true;
    }
    public virtual void DoLightAttack()
    {
        if (!UseStamina(5)) return;
        if (!canUseSkill) {
            Debug.Log("Light attack is cooling down");
            return; 
        }
        //ProjectileLightAttack projectileLightAttack = new ProjectileLightAttack();
        GameObject bullet = Shooting.ShootProjectile(lightAttackPrefab, GetShootingPosition(), GetShootingDirection());
        //add effect of skill here
        AudioManager.Instance.PlayAudio(AudioConstants.BUBBLE_BULLET);
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
    #endregion
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

    public void GetDamage()
    {
        SwitchToState(PlayerState.Choke);
    }

    public void SetSpawnPositionRight(bool needToChangeColor)
    {
        if (needToChangeColor)
        {
            SetupSecondaryColor();
        }
        playerPosition = PlayerPosition.Right;
        Flip(PlayerDirection.Right);
        SetUpInput();
    }
}
