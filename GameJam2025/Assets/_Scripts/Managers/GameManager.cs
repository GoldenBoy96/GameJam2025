using System.Collections;
using System.Collections.Generic;
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

    public static GameManager Instance { get => instance; }

    private void Awake()
    {
        Observer.AddObserver(ObserverConstants.PLAYER_DEAD, (x) => { StopGame((PlayerPosition)x[0]); });

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);

        }
    }

    private void Start()
    {
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
    private void StartGame()
    {
        player1Object = Instantiate(characterPrefab[player1Selection], transform);
        player1Object.transform.localPosition = player1Spawner.transform.localPosition;


        player2Object = Instantiate(characterPrefab[player1Selection], transform);
        player2Object.transform.localPosition = player2Spawner.transform.localPosition;
        player2Object.GetComponent<BaseCharacterController>().SpawnRight();
    }

    private void StopGame(PlayerPosition deadPlayerPosition)
    {
        if (deadPlayerPosition == PlayerPosition.Left)
        {
            Debug.Log("Player 2 win!!!");
        }
        else if (deadPlayerPosition == PlayerPosition.Right)
        {
            Debug.Log("Player 1 win!!!");
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
