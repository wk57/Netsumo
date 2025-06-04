using System;
using System.Collections;
using Unity.Netcode.Components;
using UnityEngine;


public class BeingPushState : IPlayerState
{
    public event Action OnAction;

    private Vector3 pushDirection;
    private Rigidbody rb;
    private NetworkAnimator animator;
    private Coroutine coroutine;
    private float pushForce = 5f;

    public BeingPushState(Vector3 pushDirection, Rigidbody rb, NetworkAnimator animator)
    {
        this.animator = animator;
        this.pushDirection = pushDirection;
        this.rb = rb;
        
    }

    public void EnterState(PlayerStateMachine player)
    {
        animator.SetTrigger("BEINGPUSHED");

        coroutine = player.StartCoroutine(StateDuration());
    }

    public void ExitState(PlayerStateMachine player)
    {
        if(coroutine != null)
        {
            player.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void FixedUpdateState(PlayerStateMachine player) { }
    public void UpdateState(PlayerStateMachine player) { }
    
    private IEnumerator StateDuration()
    {
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
        OnAction.Invoke();
    }
}

