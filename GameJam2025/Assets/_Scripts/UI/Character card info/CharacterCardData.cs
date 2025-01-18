using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Character", menuName = "Character Cell")]
public class CharacterCardData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
    public float zoom = 1;
}
