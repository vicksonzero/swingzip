using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_swingSMB : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponent<BPlayer>();
        var grapple = player.grapple;

        Vector3 grappleDirection = grapple.transform.position - player.transform.position;
        float angle = Mathf.Atan2(grappleDirection.y, grappleDirection.x) * Mathf.Rad2Deg - 90;
        Debug.Log(angle);
        player.spriteRoot.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        float signedAngle = Vector3.SignedAngle(grappleDirection, player.velocity, Vector3.back);
        var localScale = player.spriteRoot.localScale;
        localScale.x = Mathf.Sign(signedAngle);
        player.spriteRoot.localScale = localScale;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponent<BPlayer>();
        var grapple = player.grapple;

        Vector3 grappleDirection = grapple.transform.position - player.transform.position;
        float angle = Mathf.Atan2(grappleDirection.y, grappleDirection.x) * Mathf.Rad2Deg - 90;
        player.spriteRoot.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        float signedAngle = Vector3.SignedAngle(grappleDirection, player.velocity, Vector3.back);
        var localScale = player.spriteRoot.localScale;
        localScale.x = Mathf.Sign(signedAngle);
        player.spriteRoot.localScale = localScale;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
