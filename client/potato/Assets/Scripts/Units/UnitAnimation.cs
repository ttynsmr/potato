using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : StateMachineBehaviour
{
    public GameObject down;
    public GameObject up;
    public GameObject left;
    public bool flipLeft = false;
    public GameObject right;
    public bool flipRight = true;

    private GameObject currentView;
    private Potato.UnitDirection currentDirection;

    public GameObject GetView(Potato.UnitDirection direction)
    {
        switch (direction)
        {
            case Potato.UnitDirection.Down:
                return down;
            case Potato.UnitDirection.Left:
                return left;
            case Potato.UnitDirection.Right:
                return right;
            case Potato.UnitDirection.Up:
                return up;
            default:
                return down;
        }
    }

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentView != null)
        {
            Destroy(currentView);
        }

        var direction = (Potato.UnitDirection)animator.GetInteger("Direction");

        var view = GetView(direction);
        currentView = Instantiate(view, animator.transform);
        currentView.transform.localScale = direction switch
        {
            Potato.UnitDirection.Right => new Vector3(-1, 1, 1),
            Potato.UnitDirection.Left => Vector3.one,
            Potato.UnitDirection.Up => Vector3.one,
            Potato.UnitDirection.Down => Vector3.one,
            _ => Vector3.one
        };
        currentDirection = direction;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var direction = (Potato.UnitDirection)animator.GetInteger("Direction");

        if (currentDirection != direction)
        {
            if (currentView != null)
            {
                Destroy(currentView);
            }

            var view = GetView(direction);
            currentView = Instantiate(view, animator.transform);
            currentView.transform.localScale = direction switch
            {
                Potato.UnitDirection.Right => new Vector3(-1, 1, 1),
                Potato.UnitDirection.Left => Vector3.one,
                Potato.UnitDirection.Up => Vector3.one,
                Potato.UnitDirection.Down => Vector3.one,
                _ => Vector3.one
            };
            currentDirection = direction;
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentView != null)
        {
            Destroy(currentView.gameObject);
        }
    }

    //OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }

    //OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that sets up animation IK (inverse kinematics)
    }
}
