using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Collider2D swordC2d;
    [SerializeField] private Animator playerAnimator;
    private bool IsAttacking;

    // Start is called before the first frame update
    void Start()
    {
        IsAttacking = false;
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            playerAnimator.SetTrigger("Sword_Attack");
        }
    }
    public bool isPlayerAttacking()
    {
        return IsAttacking;
    }

    void SwordAttack()
    {
        IsAttacking = true;
        swordC2d.enabled = true;
    }

    void NoAttack()
    {
        IsAttacking = false;
        swordC2d.enabled = false;
        playerAnimator.ResetTrigger("Sword_Attack");
    }
}
