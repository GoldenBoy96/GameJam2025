using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseLevelController : MonoBehaviour
{
    private static BaseLevelController instance;

    public static BaseLevelController Instance { get => instance; }

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


    protected virtual void SetupLevel()
    {
    }


    public virtual void StartGame()
    {
    }

    public virtual void StopGame(PlayerPosition deadPlayer)
    {
    }

    public virtual void Restart()
    {
        //reload scene
    }

    public virtual void ReturnToMainMenu()
    {
        //load scene menu
    }

    #region State Machine
    public virtual void SwitchToState(LevelState incomingState)
    {
        
    }
    #endregion
}
