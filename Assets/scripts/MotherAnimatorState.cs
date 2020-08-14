using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("greet"))
        {
            animator.SetBool("greet", false);
        }
    }

    //public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        
    //}
}
