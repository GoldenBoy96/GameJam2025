using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSelectionCSS : MonoBehaviour
{
    public List<CharacterCardData> charactersDetails = new List<CharacterCardData>();
    [SerializeField] GameObject charCellPrefab;

    public Vector2 slotArtworkSize;
    public Transform playerSlotsContainer;

    [Space]
    [Header("Current Confirmed Character")]
    public CharacterCardData confirmedCharacter;
    public CharacterCardData selectedChar;

    public int index1;
    public int index2;
    [SerializeField] bool keyDown;
    [SerializeField] bool keyDown2;
    [SerializeField] int maxIndex;
    private void Start()
    {
        foreach (CharacterCardData card in charactersDetails)
        {
            SpawnCharacterCell(card);
            maxIndex = charactersDetails.Count - 1;
        }
    }

    private void Update()
    {
        GetP1Input();
        GetP2Input();
        if (charactersDetails[index1] != null)
        {
            ShowCharacterInSlot(0, charactersDetails[index1]);
            selectedChar = charactersDetails[index1];
        }
        if (charactersDetails[index1] != null)
        {
            ShowCharacterInSlot(1, charactersDetails[index2]);
            selectedChar = charactersDetails[index2];
        }

    }
    private void GetP1Input()
    {
        //if (Input.GetAxis("Horizontal") != 0)
        if (Input.GetKeyDown(InputConstants.PLAYER_1_LEFT) || Input.GetKeyDown(InputConstants.PLAYER_1_RIGHT))
        {
            if (!keyDown)
            {
                if (Input.GetKeyDown(InputConstants.PLAYER_1_LEFT))
                {
                    if (index1 < maxIndex)
                    {
                        index1++;
                    }
                    else
                    {
                        index1 = 0;
                    }
                }
                else if (Input.GetKeyDown(InputConstants.PLAYER_1_RIGHT))
                {

                    if (index1 > 0)
                    {
                        index1--;
                    }
                    else
                    {
                        index1 = maxIndex;
                    }
                }
                keyDown = true;
            }
        }
        else
        {
            keyDown = false;
        }


    }    private void GetP2Input()
    {
        //if (Input.GetAxis("Horizontal") != 0)
        if (Input.GetKeyDown(InputConstants.PLAYER_2_LEFT) || Input.GetKeyDown(InputConstants.PLAYER_2_RIGHT))
        {
            if (!keyDown2)
            {
                if (Input.GetKeyDown(InputConstants.PLAYER_2_LEFT))
                {
                    if (index2 < maxIndex)
                    {
                        index2++;
                    }
                    else
                    {
                        index2 = 0;
                    }
                }
                else if (Input.GetKeyDown(InputConstants.PLAYER_2_RIGHT))
                {

                    if (index2 > 0)
                    {
                        index2--;
                    }
                    else
                    {
                        index2 = maxIndex;
                    }
                }
                keyDown2 = true;
            }
        }
        else
        {
            keyDown2 = false;
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
        artwork.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }

    public Vector2 uiPivot(Sprite sprite)
    {
        Vector2 pixelSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 pixelPivot = sprite.pivot;
        return new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);
    }

    public void ShowCharacterInSlot(int player, CharacterCardData character)
    {
        bool nullChar = (character == null);

        Color alpha = nullChar ? Color.clear : Color.white;
        //Sprite artwork = nullChar ? null : character.characterSprite;
        Sprite artwork = character.characterSprite;

        string name = nullChar ? string.Empty : character.characterName;
        string playernickname = "Player " + (player + 1).ToString();
        string playernumber = "P" + (player + 1).ToString();

        Transform slot = playerSlotsContainer.GetChild(player);
        Image artworkSrc = slot.transform.Find(MenuConstant.CharCellArtWork).GetComponent<Image>();
        artworkSrc.sprite = artwork;
        //Transform slot = playerSlotsContainer.GetC

        Transform slotArtwork = slot.Find(MenuConstant.CharCellArtWork);
        //Transform slotIcon = slot.Find("icon");

        //Sequence s = DOTween.Sequence();
        //s.Append(slotArtwork.DOLocalMoveX(-300, .05f).SetEase(Ease.OutCubic));
        //s.AppendCallback(() => slotArtwork.GetComponent<Image>().sprite = artwork);
        //s.AppendCallback(() => slotArtwork.GetComponent<Image>().color = alpha);
        //s.Append(slotArtwork.DOLocalMoveX(300, 0));
        //s.Append(slotArtwork.DOLocalMoveX(0, .05f).SetEase(Ease.OutCubic));

        if (nullChar)
        {
            //slotIcon.GetComponent<Image>().DOFade(0, 0);
        }
        else
        {
            //slotIcon.GetComponent<Image>().sprite = character.characterIcon;
            //slotIcon.GetComponent<Image>().DOFade(.3f, 0);
        }

        if (artwork != null)
        {
            slotArtwork.GetComponent<RectTransform>().pivot = uiPivot(artwork);
            //slotArtwork.GetComponent<RectTransform>().sizeDelta = slotArtworkSize;
            //slotArtwork.GetComponent<RectTransform>().sizeDelta *= character.zoom;
            //artworkSrc.GetComponent<RectTransform>().sizeDelta = artwork.s.sizeDelta * slotArtworkSize;
            slotArtwork.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        }
        //slot.Find("name").GetComponent<TextMeshProUGUI>().text = name;
        //slot.Find("player").GetComponentInChildren<TextMeshProUGUI>().text = playernickname;
        //slot.Find("iconAndPx").GetComponentInChildren<TextMeshProUGUI>().text = playernumber;
    }

    public void ConfirmCharacter(int player, CharacterCardData character)
    {
        if (confirmedCharacter == null)
        {
            confirmedCharacter = character;
            playerSlotsContainer.GetChild(player).DOPunchPosition(Vector3.down * 3, .3f, 10, 1);
        }
    }
}
