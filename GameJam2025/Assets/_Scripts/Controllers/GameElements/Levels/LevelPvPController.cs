using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelPvPController : BaseLevelController
{
    [Header("Player Spawn Point")]
    [SerializeField] GameObject player1Spawner;
    [SerializeField] GameObject player2Spawner;


    [Header("Character Selection")]
    [SerializeField] List<GameObject> characterPrefab;
    [SerializeField] int player1Selection;
    [SerializeField] int player2Selection;
    [SerializeField] bool player1Ready = false;
    [SerializeField] bool player2Ready = false;


    [Header("Runtime Data")]
    [SerializeField] GameObject player1Object;
    [SerializeField] GameObject player2Object;

    [SerializeField] BaseCharacterController player1;
    [SerializeField] BaseCharacterController player2;

    [Header("UI Handler")]
    [SerializeField] public GameObject StartScreen;
    [SerializeField] public GameObject EndScreen;
    [SerializeField] public TextMeshProUGUI WinnerLabel;
    [SerializeField] public TMP_Dropdown Player1Selection;
    [SerializeField] public TMP_Dropdown Player2Selection;
    [SerializeField] private SkillIconController player1Skill1Icon;
    [SerializeField] private SkillIconController player1Skill2Icon;
    [SerializeField] private SkillIconController player1Skill3Icon;
    [SerializeField] private SkillIconController player2Skill1Icon;
    [SerializeField] private SkillIconController player2Skill2Icon;
    [SerializeField] private SkillIconController player2Skill3Icon;

    [Header("State")]
    [SerializeField] LevelState currentState = LevelState.Prepare;

    public BaseCharacterController Player1 { get => player1; }
    public BaseCharacterController Player2 { get => player2; }

    private void Awake()
    {
        Observer.AddObserver(ObserverConstants.PLAYER_DEAD, (x) =>
        {
            StopGame((PlayerPosition)x[0]);
            SwitchToState(LevelState.Ending);
        });
        Observer.AddObserver(ObserverConstants.PLAYER_CHOKE, (x) =>
        {
            SwitchToState(LevelState.Waiting);
        });
        Observer.AddObserver(ObserverConstants.PLAYER_REVIVE, (x) =>
        {
            SwitchToState(LevelState.Playing);
        });
    }
    protected override void SetupLevel()
    {
        player1.SetUpUI(new()
        {
            (Sprite iconSprite, string eventTrigger, float skillCooldownTime, float delayCooldownTime, PlayerPosition playerPosition)
                => player1Skill1Icon.SetUp(iconSprite, eventTrigger, skillCooldownTime, delayCooldownTime, playerPosition),
            (Sprite iconSprite, string eventTrigger, float skillCooldownTime, float delayCooldownTime, PlayerPosition playerPosition)
                => player1Skill2Icon.SetUp(iconSprite, eventTrigger, skillCooldownTime, delayCooldownTime, playerPosition),
            (Sprite iconSprite, string eventTrigger, float skillCooldownTime, float delayCooldownTime, PlayerPosition playerPosition)
                => player1Skill3Icon.SetUp(iconSprite, eventTrigger, skillCooldownTime, delayCooldownTime, playerPosition),
        });
        player2.SetUpUI(new()
        {
            (Sprite iconSprite, string eventTrigger, float skillCooldownTime, float delayCooldownTime, PlayerPosition playerPosition)
                => player2Skill1Icon.SetUp(iconSprite, eventTrigger, skillCooldownTime, delayCooldownTime, playerPosition),
            (Sprite iconSprite, string eventTrigger, float skillCooldownTime, float delayCooldownTime, PlayerPosition playerPosition)
                => player2Skill2Icon.SetUp(iconSprite, eventTrigger, skillCooldownTime, delayCooldownTime, playerPosition),
            (Sprite iconSprite, string eventTrigger, float skillCooldownTime, float delayCooldownTime, PlayerPosition playerPosition)
                => player2Skill3Icon.SetUp(iconSprite, eventTrigger, skillCooldownTime, delayCooldownTime, playerPosition),
        });
    }

    public void Player1ReadyToggle()
    {
        if (player1Ready)
        {
            player1Ready = false;
            Observer.Notify(ObserverConstants.PLAYER_1_READY, false);
        }
        else
        {
            player1Ready = true;
            Observer.Notify(ObserverConstants.PLAYER_1_READY, true);
        }

        if (player1Ready && player2Ready)
        {
            StartGame();
        }

    }

    public void Player2ReadyToggle()
    {
        if (player2Ready)
        {
            player2Ready = false;
            Observer.Notify(ObserverConstants.PLAYER_2_READY, false);
        }
        else
        {
            player2Ready = true;
            Observer.Notify(ObserverConstants.PLAYER_2_READY, true);
        }

        if (player1Ready && player2Ready)
        {
            StartGame();
        }

    }

    public override void StartGame()
    {
        Debug.Log("StartGame");


        player1Selection = Player1Selection.value;
        player2Selection = Player2Selection.value;
        StartScreen.SetActive(false);
        //AudioManager.Instance.StopAudio(AudioConstants.CITY_OF_TEAR);
        AudioManager.Instance.PlayAudio(AudioConstants.DECISIVE_BATTLE);

        player1Object = Instantiate(characterPrefab[player1Selection], transform);
        player1Object.transform.localPosition = player1Spawner.transform.localPosition;


        player2Object = Instantiate(characterPrefab[player2Selection], transform);
        player2Object.transform.localPosition = player2Spawner.transform.localPosition;



        player1 = player1Object.GetComponent<BaseCharacterController>();
        player2 = player2Object.GetComponent<BaseCharacterController>();

        if (player1.CharacterName.Equals(player2.CharacterName))
        {
            player2Object.GetComponent<BaseCharacterController>().SetSpawnPositionRight(true);
        }
        else
        {
            player2Object.GetComponent<BaseCharacterController>().SetSpawnPositionRight(false);
        }

        SetupLevel();
        SwitchToState(LevelState.Playing);
    }

    public override void StopGame(PlayerPosition deadPlayer)
    {
        if (deadPlayer == PlayerPosition.Left)
        {
            Debug.Log("Player 2 win!!!");
            EndScreen.SetActive(true);
            WinnerLabel.text = "Player 2 win!!!";
        }
        else if (deadPlayer == PlayerPosition.Right)
        {
            Debug.Log("Player 1 win!!!");
            EndScreen.SetActive(true);
            WinnerLabel.text = "Player 1 win!!!";
        }
    }

    public override void Restart()
    {
        //reload scene
    }

    public override void ReturnToMainMenu()
    {
        //load scene menu
    }


    private IEnumerator MakePlayerFaceToFace()
    {
        //Debug.Log(player1Object.transform.position.x < player2Object.transform.position.x);
        if (player1Object.transform.position.x < player2Object.transform.position.x)
        {
            player1.Flip(PlayerDirection.Right);
            player2.Flip(PlayerDirection.Left);
        }
        else
        {
            player1.Flip(PlayerDirection.Left);
            player2.Flip(PlayerDirection.Right);
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(MakePlayerFaceToFace());
    }

    #region State Machine
    public override void SwitchToState(LevelState incomingState)
    {
        switch (currentState)
        {
            case LevelState.Prepare:
                ExitStatePrepare();
                break;
            case LevelState.Playing:
                ExitStatePlaying();
                break;
            case LevelState.Waiting:
                ExitStateWaiting();
                break;
            case LevelState.Ending:
                ExitStateEnding();
                break;
        }
        currentState = incomingState;
        switch (incomingState)
        {
            case LevelState.Prepare:
                EnterStatePrepare();
                break;
            case LevelState.Playing:
                EnterStatePlaying();
                break;
            case LevelState.Waiting:
                EnterStateWaiting();
                break;
            case LevelState.Ending:
                EnterStateEnding();
                break;
        }
        //Debug.Log(currentState.ToString());
    }

    #region State Prepare
    protected virtual void EnterStatePrepare()
    {
    }

    protected virtual void UpdateStatePrepare()
    {
    }
    protected virtual void ExitStatePrepare()
    {
        StopAllCoroutines();
    }
    #endregion
    #region State Playing
    protected virtual void EnterStatePlaying()
    {
        Debug.Log("EnterStatePlaying");
        player1.SwitchToState(PlayerState.Alive);
        player2.SwitchToState(PlayerState.Alive);
        StartCoroutine(MakePlayerFaceToFace());
    }

    protected virtual void UpdateStatePlaying()
    {
    }
    protected virtual void ExitStatePlaying()
    {
        StopAllCoroutines();
    }
    #endregion
    #region State Waiting
    protected virtual void EnterStateWaiting()
    {
        if (player1.CurrentState == PlayerState.Alive)
        {
            player1.SwitchToState(PlayerState.Waiting);
        }

        if (player2.CurrentState == PlayerState.Alive)
        {
            player2.SwitchToState(PlayerState.Waiting);
        }
    }

    protected virtual void UpdateStateWaiting()
    {
    }
    protected virtual void ExitStateWaiting()
    {
        StopAllCoroutines();
        if (player1.CurrentState == PlayerState.Waiting)
        {
            player1.SwitchToState(PlayerState.Alive);
        }

        if (player2.CurrentState == PlayerState.Waiting)
        {
            player2.SwitchToState(PlayerState.Alive);
        }
    }
    #endregion
    #region State Ending
    protected virtual void EnterStateEnding()
    {
        if (player1.CurrentState != PlayerState.Dead)
        {
            player1.SwitchToState(PlayerState.Waiting);
        }

        if (player2.CurrentState != PlayerState.Dead)
        {
            player2.SwitchToState(PlayerState.Waiting);
        }
    }

    protected virtual void UpdateStateEnding()
    {
    }
    protected virtual void ExitStateEnding()
    {
        StopAllCoroutines();
    }
    #endregion
    #endregion
}
