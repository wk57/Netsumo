using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNetworkHandler : NetworkBehaviour
{
    [Header("Dependencies")]
    [SerializeField] PlayerStateMachine playerStateMachine;    

    [Header("Parameters")]
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotationSmoothTime = 0.1f;

    private Vector3 networkPositionVelocity;
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        if (IsOwner)
        {
            playerStateMachine.GetPlayerRigidbody.isKinematic = false;
        }
    }

    private void Update()
    {
        NetworkVariablesHandling();
        if (!IsOwner)
            return;

        playerStateMachine.StateMachineUpdate();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        playerStateMachine.StateMachineFixedUpdate();
    }

    [Rpc(SendTo.Owner)]
    public void SetBeingPushStateRpc(Vector3 pushDirection)
    {
        playerStateMachine.SetBeingPushState(pushDirection);
    }

    private void NetworkVariablesHandling()
    {
        if (IsOwner)
        {
            networkPosition.Value = transform.position;
            networkRotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, networkPosition.Value, ref networkPositionVelocity, networkPositionSmoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation.Value, networkRotationSmoothTime);
        }
    }

    [Rpc(SendTo.Owner)]
    public void SetSpawnPositionRpc(Vector3 position)
    {        
        if (IsOwner)
        {
            transform.position = position;
        }
    }

    [Rpc(SendTo.Owner)]
    public void SetPlayerInputRpc(bool isEnable)
    {
        playerStateMachine.PreparePlayerPerRound(isEnable);
    }

    [Rpc(SendTo.Owner)]
    public void SetPlayerAliveRpc(bool isAlive)
    {
        playerStateMachine.SetPlayerAlive(isAlive);
    }


}
