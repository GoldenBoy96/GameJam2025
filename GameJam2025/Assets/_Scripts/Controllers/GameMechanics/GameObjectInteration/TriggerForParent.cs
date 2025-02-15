using UnityEngine;

public class TriggerForParent : MonoBehaviour
{
    [SerializeField] BaseProjectileController masterController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (masterController == null) return;
        masterController.TriggerFromChildren(collision);
    }
}
