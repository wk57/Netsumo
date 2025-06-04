using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Dependences")]
    [SerializeField] private InputMappingModel mapingModel;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NetworkAnimator animator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private List<MeshRenderer> playerMesh;

    private bool IsTransitioning;
    private bool InAction;

    private IPlayerState CurrentState;    
    private InputAction move, push;    

    void Awake()
    {        
        move = mapingModel.GetAction(InputMappingModel.PlayerActions.WALK);
        push = mapingModel.GetAction(InputMappingModel.PlayerActions.PUSH);
    }

    void Start()
    {
        CurrentState = new IdleState();
        CurrentState.EnterState(this);
    }

    public void StateMachineUpdate()
    {
        CurrentState?.UpdateState(this);
        StatesHandling();
    }

    public void StateMachineFixedUpdate()
    {        
        CurrentState?.FixedUpdateState(this);
    }

    void StatesHandling()
    {
        if (!IsTransitioning && !InAction && move.ReadValue<Vector2>() != Vector2.zero)
        {            
            InAction = true;            
            TransitionToState(new WalkingState(move,rb));
        }
        else if (!IsTransitioning && push.WasPerformedThisFrame())
        {
            InAction = true;
            TransitionToState(new PushingState(animator));
        }
        else if(!IsTransitioning && !InAction && move.ReadValue<Vector2>() == Vector2.zero)
        { 
            TransitionToState(new IdleState());
        }
    }

    public void SetBeingPushState(Vector3 pushDirection)
    {
        InAction = true;
        TransitionToState(new BeingPushState(pushDirection, rb, animator));
    }

    void ActionFinished()
    {        
        InAction = false;
    }

    private void OnEnable()
    {
        move.Enable();
        push.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        push.Disable();        
    }

    public void TransitionToState(IPlayerState newState)
    {
        IsTransitioning = true;
        CurrentState?.ExitState(this);
        CurrentState.OnAction -= ActionFinished;
        CurrentState = newState;
        CurrentState.OnAction += ActionFinished;
        CurrentState.EnterState(this);
        IsTransitioning = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward, 0.5f);
    }

    public void PreparePlayerPerRound(bool canMove)
    {
        playerInput.enabled = canMove;
        
    }

    public void SetPlayerAlive(bool isAlive)
    {
        foreach (MeshRenderer obj in playerMesh)
        {
            obj.enabled = isAlive;
            rb.linearVelocity = Vector3.zero;
        }
    }

    public Rigidbody GetPlayerRigidbody => rb;
}
