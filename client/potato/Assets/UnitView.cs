using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    public RuntimeAnimatorController animatorController;
    public Animator animator;

    private Potato.UnitDirection direction;

    public Potato.UnitDirection Direction
    {
        get => direction;
        set {
            direction = value;
            animator.SetInteger("Direction", (int)value);
        }
    }

    public bool Moving
    {
        get => animator.GetBool("Walk");
        set => animator.SetBool("Walk", value);
    }

    private void Start()
    {
        Moving = false;
    }
}
