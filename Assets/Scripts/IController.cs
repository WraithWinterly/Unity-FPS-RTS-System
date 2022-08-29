using Unity.VisualScripting;

public interface IController
{
    public bool IsGrounded();
    public bool IsCrouching();
    public bool IsSprinting();
    public bool IsMoving();
    public bool IsRising();
    public bool IsFalling();
}