using UnityEngine;

public partial class Player
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_detectGroundTrm)
        {
            _detectGroundTrm = transform.Find("DetectGround");
        }

        if (!VisualCompo)
            VisualCompo = transform.Find("Visual");
        if (!_detectWallTrm)
            _detectWallTrm = transform.Find("DetectWall");
        if (!_detectCeilingTrm)
            _detectCeilingTrm = transform.Find("DetectCeiling");
        Vector2 pos = _detectWallTrm.localPosition;
        float xPos = Mathf.Abs(pos.x);
        pos.x = xPos * -FacingDirection;
        _detectWallTrm.localPosition = pos;
        Gizmos.color = Color.red;
        if (_detectGroundTrm)
            Gizmos.DrawWireCube(_detectGroundTrm.position + (Vector3.down * _detectGroundSize.y * .5f),
                _detectGroundSize);
        if (_detectWallTrm)
            Gizmos.DrawRay(_detectWallTrm.position, Vector2.left * FacingDirection * _detectWallDistance);
        if (_detectCeilingTrm)
            Gizmos.DrawRay(_detectCeilingTrm.position, Vector2.up * _detectCeilingDistance);
        Gizmos.color = Color.white;
    }

#endif
}