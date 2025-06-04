using System;
using UnityEngine;
using System.Collections;
using Unity.Netcode.Components;

public class PushingState : IPlayerState
{
    public event Action OnAction;

    private NetworkAnimator animator;
    private Coroutine coroutine;

    public PushingState(NetworkAnimator animator)
    {        
        this.animator = animator;
    }

    public void EnterState(PlayerStateMachine player)
    {
        animator.SetTrigger("PUSH");

        coroutine = player.StartCoroutine(StateDuration(player));
    }

    public void UpdateState(PlayerStateMachine player) { }

    public void ExitState(PlayerStateMachine player) 
    {
        if (coroutine != null)
        {
            player.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void FixedUpdateState(PlayerStateMachine player) { }
    private IEnumerator StateDuration(PlayerStateMachine player)
    {
        Vector3 spherePosition = player.transform.position + player.transform.forward;
        Collider[] hitColliders = Physics.OverlapSphere(spherePosition, 0.5f, LayerMask.GetMask("Player"));

        if (hitColliders != null)
        {
            foreach(Collider hitCollider in hitColliders)
            {
                if(hitCollider.gameObject != player.gameObject)
                {
                    PlayerNetworkHandler playerNetworkHandler = hitCollider.GetComponent<PlayerNetworkHandler>();
                    playerNetworkHandler.SetBeingPushStateRpc(player.transform.forward);
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
        OnAction.Invoke();
    }
}
