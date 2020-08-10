using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    private Animator animator;

    private int health = 3;

    public int Health { get => health; set => health = value; }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health > 0)
        {
            animator.SetTrigger("gotHit");
        }
        else
        {
            GetComponent<CharacterController>().enabled = false;
            animator.enabled = false;
        }
    }
}
