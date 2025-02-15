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


    protected override void SetupLevel()
    {
        Observer.AddObserver(ObserverConstants.PLAYER_DEAD, (x) => { StopGame((PlayerPosition)x[0]); });
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
        StartCoroutine(MakePlayerFaceToFace());
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
        Debug.Log(player1Object.transform.position.x < player2Object.transform.position.x);
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

}
