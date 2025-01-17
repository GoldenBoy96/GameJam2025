using UnityEngine;

public class BaseProjectileController : MonoBehaviour
{
    [SerializeField] BaseCharacter character;

    protected void DemoShoot()
    {
        Debug.Log("Base shoot ");
    }
}
