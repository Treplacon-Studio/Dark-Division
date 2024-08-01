using System.Collections.Generic;

public class ActionsManager
{
    private static Dictionary<int, ActionsManager> _instances = new();

    public static ActionsManager GetInstance(int playerId)
    {
        if (!_instances.ContainsKey(playerId))
        {
            _instances[playerId] = new ActionsManager();
        }
        return _instances[playerId];
    }
    
    public ComponentHolder ComponentHolder;
    public Reloading Reloading;
    public Inspecting Inspecting;
    public Aiming Aiming;
    public Shooting Shooting;
    public Switching Switching;

    public Walking Walking;
    public Sprinting Sprinting;
    public Jumping Jumping;
    public Crouching Crouching;
    public ThrowEquipment ThrowEquipment;
}