using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField] Slider player1Oxigen;
    [SerializeField] Slider player1Stamina;
    [SerializeField] Slider player2Oxigen;
    [SerializeField] Slider player2Stamina;

    private void Start()
    {
        StartCoroutine(FollowPlayerStats());
    }
    IEnumerator FollowPlayerStats()
    {
        yield return new WaitForSeconds(0.1f);
        try
        {
            player1Oxigen.value = GameManager.Instance.player1.oxigen / 100f;
            player2Oxigen.value = GameManager.Instance.player2.oxigen / 100f;
            player1Stamina.value = GameManager.Instance.player1.stamina / 100f;
            player2Stamina.value = GameManager.Instance.player2.stamina / 100f;

        }
        catch { }
        StartCoroutine(FollowPlayerStats());
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Start Screen");
    }
    public void JoinGame()
    {
        SceneManager.LoadScene("Main");
    }
}
