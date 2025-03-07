using System.Linq;
using UnityEngine;

public partial class Player
{
    public bool IsGrounded()
    {
        bool result = Physics2D.OverlapBox(_detectGroundTrm.position + (Vector3.down * (_detectGroundSize.y * .5f)),
            _detectGroundSize, 0, _whatIsGround);
        return result;
    }

    public bool IsFalling()
    {
        return RigidBodyCompo.velocity.y < -.2f && !IsGrounded();
    }

    public bool IsWallDetected()
    {
        if (IsGrounded()) return false;
        // bool result = Physics2D.Raycast(_detectWallTrm.position, Vector2.left * FacingDirection,
        //     _detectWallDistance, _whatIsWall);
        return RigidBodyCompo.IsTouching(new ContactFilter2D()
            { layerMask = _whatIsGround, useLayerMask = true, useTriggers = false });

        int cnt = Physics2D.Raycast(_detectWallTrm.position, Vector2.left * FacingDirection,
            new ContactFilter2D() { layerMask = _whatIsGround, useLayerMask = true, useTriggers = false }, _wallhits,
            _detectWallDistance);
        if (cnt > 0)
        {
            if (_wallhits.First().transform.CompareTag(_ignoreWalltag))
                return false;
            return true;
        }

        return false;
    }

    public bool IsCeilingDetected()
    {
        return Physics2D.Raycast(_detectCeilingTrm.position, Vector2.up,
            _detectCeilingDistance, _whatIsGround);
    }
}