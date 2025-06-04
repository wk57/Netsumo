using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerState 
{
    public event Action OnAction;
    
    void EnterState(PlayerStateMachine player);
    void UpdateState(PlayerStateMachine player);
    void ExitState(PlayerStateMachine player);
    //void OnTriggerEnter(PlayerStateMachine player, Collider other);

    void FixedUpdateState(PlayerStateMachine player);


}
