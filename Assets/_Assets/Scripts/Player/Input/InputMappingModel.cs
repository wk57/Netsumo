
using UnityEngine;
using UnityEngine.InputSystem;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "Mapping Model", menuName = "MVC/Input Mapping Model")]
public class InputMappingModel : ScriptableObject
{
    public enum PlayerActions 
    {
        WALK, 
        PUSH 
    }

    [SerializedDictionary("Player Actions", "Input Action Reference")]
    public SerializedDictionary<PlayerActions, InputActionReference> ActionsAvailable;
    

    public InputActionReference GetAction(PlayerActions playerActions)
    {
        return ActionsAvailable[playerActions];
    }
}
