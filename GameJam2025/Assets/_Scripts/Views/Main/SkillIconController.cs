using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillIconController : MonoBehaviour
{
    [SerializeField] Image skillIconBackground;
    [SerializeField] Image skillCooldown;
    [SerializeField] Image delayCooldown;
    [SerializeField] string eventTrigger;

    private float skillCooldownTime;
    private float delayCooldownTime;
    private PlayerPosition playerPosition;

    bool isSetUp = false;

    private void Start()
    {
        skillCooldown.fillAmount = 0;
        delayCooldown.fillAmount = 0;
    }

    private void Update()
    {
        ApplyCooldown();
    }

    private void ApplyCooldown()
    {
        skillCooldown.fillAmount -= (60f / (skillCooldownTime * 100f)) * Time.deltaTime;
        delayCooldown.fillAmount -= (60f / (delayCooldownTime * 100f)) * Time.deltaTime;
    }


    public void SetUp(Sprite iconSprite, string eventTrigger, float skillCooldownTime, float delayCooldownTime, PlayerPosition playerPosition)
    {
        if (isSetUp) { return; }
        this.eventTrigger = eventTrigger;
        skillIconBackground.sprite = iconSprite;
        this.skillCooldownTime = skillCooldownTime;
        this.delayCooldownTime = delayCooldownTime;
        Observer.AddObserver(eventTrigger, (x) =>
        {
            //Debug.Log(eventTrigger);
            SetSkillCooldownValue(1);
            SetDelayCooldownValue(1);
        });

        this.playerPosition = playerPosition;
        switch (playerPosition)
        {
            case PlayerPosition.Left:
                Observer.AddObserver(ObserverConstants.PLAYER_1_DELAY, (x) =>
                {
                    SetDelayCooldownValue(1);
                });
                break;
            case PlayerPosition.Right:
                Observer.AddObserver(ObserverConstants.PLAYER_2_DELAY, (x) =>
                {
                    SetDelayCooldownValue(1);
                });
                break;
        }
        Observer.Notify(eventTrigger);
    }

    public void SetSkillCooldownValue(float percent)
    {
        skillCooldown.fillAmount = percent;
    }
    public void SetDelayCooldownValue(float percent)
    {
        delayCooldown.fillAmount = percent;
    }

}
