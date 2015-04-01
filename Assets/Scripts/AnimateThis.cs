using UnityEngine;
using System.Collections;

public class AnimateThis : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator.SetInteger("pose", 1);
    }
}