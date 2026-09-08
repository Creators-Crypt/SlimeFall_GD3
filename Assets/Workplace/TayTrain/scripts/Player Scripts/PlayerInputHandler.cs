using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool DodgePressed { get; private set;  }
    public bool TeleportPressed { get; private set; }
    public bool ConcentratePressed { get; private set; }

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
        
    }
    public void OnJump(InputValue value)
    {
        if(value.isPressed)
        {
            JumpPressed = true;
        }
    }
    public void UseJump()
    {
        JumpPressed = false;
    }
    public void OnSprint(InputValue value)
    {
        SprintHeld = value.isPressed;
        Debug.Log("SprintHeld: " + SprintHeld);
    }
    public void OnDodge(InputValue value)
    {
        if(value.isPressed)
        {
            DodgePressed = true;
        }
    }
    public void UseDodge()
    {
        DodgePressed = false;
    }
    public void OnTeleport(InputValue value)
    {
        if(value.isPressed)
        {
            TeleportPressed = true;
        }
    }
    public void UseTeleport()
    {
        TeleportPressed = false;
    }
    public void OnConcentrate(InputValue value) 
    {
        if(value.isPressed)
        {
            ConcentratePressed = true;
        }
    }
    public void UseConcentrate()
    {
        ConcentratePressed = false;
    }
}
