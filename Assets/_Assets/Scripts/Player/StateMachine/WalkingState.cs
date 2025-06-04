using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkingState : IPlayerState
{
    private InputAction move;
    private Rigidbody rb;    
    private Vector3 moveDirection;

    public event Action OnAction;

    private float rotationSpeed = 10f; 
    private float moveSpeed = 5f;    

    public WalkingState(InputAction playerInput, Rigidbody rb)
    {
        this.move = playerInput;
        this.rb = rb;
    }

    public void EnterState(PlayerStateMachine player) { }

    public void UpdateState(PlayerStateMachine player)
    {
        moveDirection = new Vector3(move.ReadValue<Vector2>().x,0,move.ReadValue<Vector2>().y);        

        if (moveDirection.magnitude < 0.2f )
        {
            OnAction.Invoke();
        }
    }

    public void ExitState(PlayerStateMachine player) 
    {
        rb.linearVelocity = new Vector3(0f,rb.linearVelocity.y,0f);
    }

    public void FixedUpdateState(PlayerStateMachine player)
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);        
    }
}
