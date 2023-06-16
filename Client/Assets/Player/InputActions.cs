using Godot;


public class InputActions {


    /*-------------------------------------------------------------------------
    ---------------------------------------------------------------------------
     SECTIONINFO: Both the client and server need to handle these 
    ---------------------------------------------------------------------------
    -------------------------------------------------------------------------*/

    /*-------------------------------------------------------------------------
     SUBSECTIONINFO: Basic movement actions
    -------------------------------------------------------------------------*/
    public static readonly StringName PlayerMoveForward = "player_move_forward"; 
    public static readonly StringName PlayerMoveBackward = "player_move_backward"; 
    public static readonly StringName PlayerMoveLeft = "player_move_left"; 
    public static readonly StringName PlayerMoveRight = "player_move_right"; 
    public static readonly StringName PlayerMoveUp = "player_move_up"; 
    public static readonly StringName PlayerMoveDown = "player_move_down"; 
    public static readonly StringName PlayerRotatePitchDown = "player_rotate_pitch_down"; 
    public static readonly StringName PlayerRotatePitchUp = "player_rotate_pitch_up"; 
    public static readonly StringName PlayerRotateRollLeft = "player_rotate_roll_left"; 
    public static readonly StringName PlayerRotateRollRight = "player_rotate_roll_right"; 
    public static readonly StringName PlayerRotateYawLeft = "player_rotate_yaw_left"; 
    public static readonly StringName PlayerRotateYawRight = "player_rotate_yaw_right"; 

    /*-------------------------------------------------------------------------
     SUBSECTIONINFO: *Advanced* movement actions
    -------------------------------------------------------------------------*/
    public static readonly StringName PlayerDisableFlightAssist = "player_disable_flightassist"; 


    public static readonly StringName PlayerResetMouseAccumulator = "player_reset_mouse_accumulator"; 
    
    public static readonly StringName PlayerResetThrottle = "player_reset_throttle"; 


}