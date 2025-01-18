using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSelectionCSS : MonoBehaviour
    {
    public List<CharacterCardData> charactersDetails = new List<CharacterCardData>();
    [SerializeField] GameObject charCellPrefab;

    private void Start()
    {
        foreach (CharacterCardData card in charactersDetails)
        {
            SpawnCharacterCell(card);
        }
    }

    private void SpawnCharacterCell(CharacterCardData cellData)
    {
        GameObject charCell = Instantiate(charCellPrefab, transform);
        Image artwork = charCell.transform.Find(MenuConstant.CharCellArtWork).GetComponent<Image>();
        TextMeshProUGUI name = charCell.transform.Find(MenuConstant.CharCellName).GetComponentInChildren<TextMeshProUGUI>();

        artwork.sprite = cellData.characterSprite;
        name.text = cellData.characterName;

        artwork.GetComponent<RectTransform>().pivot = uiPivot(artwork.sprite);
        artwork.GetComponent<RectTransform>().sizeDelta *= cellData.zoom;
    }

    public Vector2 uiPivot(Sprite sprite)
    {
        Vector2 pixelSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 pixelPivot = sprite.pivot;
        return new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);
    }

}
