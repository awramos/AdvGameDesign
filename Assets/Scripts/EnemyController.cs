using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour 
{
    [SerializeField] private Transform selfTrans;
    [SerializeField] private Animator animator;
    private Transform playerTrans;
    private float rotationSpeed;
    private float approachRadius;
    private float attackRadius;
    public float moveSpeed;

    void Start()
    {
        SetTarget(GameManager.instance.player.playerTransform);
        rotationSpeed = 5f;
        approachRadius = 13f;
        attackRadius = 3f;
    }

    void Update()
    {
        MoveStuff();
        FaceTarget();
    }

    public void SetTarget(Transform t)
    {
        playerTrans = t;
    }
    private void MoveStuff()
    {
        //attacking
        if(Mathf.Abs(Vector3.Distance(selfTrans.position, playerTrans.position)) < attackRadius)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Attack");
        }
        //approaching
        else if(Mathf.Abs(Vector3.Distance (selfTrans.position, playerTrans.position)) < approachRadius)
        {
            //Vector3 towards = 
            selfTrans.position += (playerTrans.position - selfTrans.position) * moveSpeed * Time.deltaTime;
            animator.SetFloat("Speed", 1f);
            animator.ResetTrigger("Attack");
        }
        //idle
        else if (Mathf.Abs(Vector3.Distance(selfTrans.position, playerTrans.position)) > approachRadius)
        {
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Attack");
        }
    }
    private void FaceTarget()
    {
        if(playerTrans == null)
        {
            selfTrans.rotation = Quaternion.Lerp(selfTrans.rotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 towardsPlayer = playerTrans.position - selfTrans.position;
            Quaternion targetRotation = Quaternion.LookRotation(towardsPlayer);
            selfTrans.rotation = Quaternion.Lerp(selfTrans.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
