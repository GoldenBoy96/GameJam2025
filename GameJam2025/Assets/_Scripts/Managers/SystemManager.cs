using UnityEngine;

public class SystemManager : MonoBehaviour
{
   private static SystemManager instance;

    public static SystemManager Instance { get => instance;}

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public void GoToMainMenu()
    {
        //TODO
    }
}
