using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] Animator animator;
    //[SerializeField] AnimatorFunction animatorFunction;
    [SerializeField] int thisIndex;
    void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool(MenuConstant.IsSelected, true);
            //if (Input.GetAxis("Submit") == 1)
            if (Input.GetKeyDown(InputConstants.PLAYER_1_CLOCKWISE))
            {
                animator.SetBool(MenuConstant.IsPressed, true);
            }
            else if (animator.GetBool(MenuConstant.IsPressed))
            {
                animator.SetBool(MenuConstant.IsPressed, false);
                //animatorFunctions.disableOnce=true; 

            }
        }
        else
        {
            animator.SetBool(MenuConstant.IsSelected, false);
        }
    }
}
