using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCharacterController_Refactor : MonoBehaviour
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
            case PlayerState.Choke:
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
                break;
            case PlayerState.Choke:
                break;
            case PlayerState.Dead:
                break;
        }
        currentState = incomingState;
    }

    #region State Alive
    protected virtual void EnterStateAlive()
    {
        canUseSkill = true;
        canUsePumb = false;
        if (chokeToDeadCoroutine != null) StopCoroutine(chokeToDeadCoroutine);
        bubble.SetActive(true);
        AudioManager.Instance.StopAudio(AudioConstants.BUBBLE_SHORTGUN);
    }

    protected virtual void UpdateStateAlive()
    {
    }
    protected virtual void ExitStateAlive()
    {
        StopAllCoroutines();
    }
    #endregion
    #region State Choke
    protected virtual void EnterStateChoke()
    {
        chokeToDeadCoroutine = StartCoroutine(WaitChokeToDead());
        if (chokeToDeadCoroutine == null)
        {
            canUseSkill = false;
            canUsePumb = true;
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
        bubble.SetActive(false);
        Observer.Notify(ObserverConstants.PLAYER_DEAD, playerPosition);
        animator.Play(AnimationConstants.Dead);
        characterHolder.transform.localPosition = new Vector3(0, -0.2f, 0);
        characterHolder.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
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
}
