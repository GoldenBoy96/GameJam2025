using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] GameObject player1Spawner;
    [SerializeField] GameObject player2Spawner;

    [SerializeField] List<GameObject> characterPrefab;
    [SerializeField] int player1Selection;
    [SerializeField] int player2Selection;
    [SerializeField] bool player1Ready = false;
    [SerializeField] bool player2Ready = false;


    [SerializeField] GameObject player1Object;
    [SerializeField] GameObject player2Object;

    [SerializeField] public BaseCharacterController player1;
    [SerializeField] public BaseCharacterController player2;

    [SerializeField] public GameObject StartScreen;
    [SerializeField] public GameObject EndScreen;
    [SerializeField] public TextMeshProUGUI WinnerLabel;
    [SerializeField] public TMP_Dropdown Player1Selection;
    [SerializeField] public TMP_Dropdown Player2Selection;

    public static GameManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Observer.AddObserver(ObserverConstants.PLAYER_DEAD, (x) => { StopGame((PlayerPosition)x[0]); });

        }
        else
        {
            Destroy(gameObject);

        }
    }

    public void Start()
    {
        
        //StartCoroutine(WaitForBattle());
    }

    IEnumerator WaitForBattle()
    {
        yield return new WaitForSeconds(1);
        ////AudioManager.Instance.PlayAudio(AudioConstants.CITY_OF_TEAR);
        StartGame();
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
    public void StartGame()
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
        player2Object.GetComponent<BaseCharacterController>().SpawnRight();


        player1 = player1Object.GetComponent<BaseCharacterController>();
        player2 = player2Object.GetComponent<BaseCharacterController>();
    }

    private void StopGame(PlayerPosition deadPlayerPosition)
    {
        if (deadPlayerPosition == PlayerPosition.Left)
        {
            Debug.Log("Player 2 win!!!");
            EndScreen.SetActive(true);
            WinnerLabel.text = "Player 2 win!!!";
        }
        else if (deadPlayerPosition == PlayerPosition.Right)
        {
            Debug.Log("Player 1 win!!!");
            EndScreen.SetActive(true);
            WinnerLabel.text = "Player 1 win!!!";
        }
    }

    public void Restart()
    {
        //reload scene
    }

    public void ReturnToMainMenu()
    {
        //load scene menu
    }
}
