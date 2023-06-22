using Godot;

public static class InputActionStr{


    /*-------------------------------------------------------------------------
    ---------------------------------------------------------------------------
     SECTIONINFO: Both the client and the server need to respond to these
    ---------------------------------------------------------------------------
    -------------------------------------------------------------------------*/

    /*-------------------------------------------------------------------------
     SUBSECTIONINFO: Basic movement actions
    -------------------------------------------------------------------------*/

    public static readonly StringName PlayerMoveForward = "PlayerMoveForward"; 
    public static readonly StringName PlayerMoveBackward = "PlayerMoveBackward"; 
    public static readonly StringName PlayerMoveLeft = "PlayerMoveLeft"; 
    public static readonly StringName PlayerMoveRight = "PlayerMoveRight"; 
    public static readonly StringName PlayerMoveUp = "PlayerMoveUp"; 
    public static readonly StringName PlayerMoveDown = "PlayerMoveDown"; 
    public static readonly StringName PlayerRotatePitchDown = "PlayerRotatePitchDown"; 
    public static readonly StringName PlayerRotatePitchUp = "PlayerRotatePitchUp"; 
    public static readonly StringName PlayerRotateRollLeft = "PlayerRotateRollLeft"; 
    public static readonly StringName PlayerRotateRollRight = "PlayerRotateRollRight"; 
    public static readonly StringName PlayerRotateYawLeft = "PlayerRotateYawLeft"; 
    public static readonly StringName PlayerRotateYawRight = "PlayerRotateYawRight"; 

    /*-------------------------------------------------------------------------
     SUBSECTIONINFO: *Advanced* movement actions
    -------------------------------------------------------------------------*/
    public static readonly StringName PlayerDisableFlightAssist = "PlayerDisableFlightAssist";
    public static readonly StringName PlayerUseWeapons = "PlayerUseWeapons";  




    /*-------------------------------------------------------------------------
    ---------------------------------------------------------------------------
     SECTIONINFO: Only the client needs to care
    ---------------------------------------------------------------------------
    -------------------------------------------------------------------------*/


    public static readonly StringName PlayerResetMouseAccumulator = "PlayerResetMouseAccumulator"; 
    
    public static readonly StringName PlayerResetThrottle = "PlayerResetThrottle"; 
}