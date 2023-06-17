using Godot;


public class PlayerMovementActions {

    public enum MovementActionsEnum : ushort {
        PlayerMoveForward,
        PlayerMoveBackward,
        PlayerMoveLeft,
        PlayerMoveRight,
        PlayerMoveUp,
        PlayerMoveDown,
        PlayerRotatePitchDown,
        PlayerRotatePitchUp,
        PlayerRotateRollLeft,
        PlayerRotateRollRight,
        PlayerRotateYawLeft,
        PlayerRotateYawRight,
        PlayerDisableFlightAssist
    }
}