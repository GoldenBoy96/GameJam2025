
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance;

    public static SystemManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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
