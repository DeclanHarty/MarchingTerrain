using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crawler : MonoBehaviour
{
    public float castDistance;
    public float castAngle;
    public float distanceTolerence;

    public float targetDistanceAwayFromWall;
    public float turningSpeed;

    public float moveFromWallSpeed;

    public LayerMask layerMask;

    public float moveSpeed;

    Vector3 topLeftCheckPosition;
    Vector3 bottomLeftCheckPosition;
    Vector3 topRightCheckPosition;
    Vector3 bottomRightCheckPosition;

    public void FixedUpdate()
    {
        Crawl(transform.right);
    }

    public void Crawl(Vector3 direction)
    {
        RaycastHit topLeftHit;
        RaycastHit bottomLeftHit;
        RaycastHit topRightHit;
        RaycastHit bottomRightHit;

        topLeftCheckPosition = Quaternion.Euler(0,0,castAngle/2) * -transform.up * targetDistanceAwayFromWall + transform.position + distanceTolerence * transform.up;
        bottomLeftCheckPosition = Quaternion.Euler(0,0,castAngle/2) * -transform.up * targetDistanceAwayFromWall + transform.position - distanceTolerence * transform.up;
        topRightCheckPosition = Quaternion.Euler(0,0,-castAngle/2) * -transform.up * targetDistanceAwayFromWall + transform.position + distanceTolerence * transform.up;
        bottomRightCheckPosition = Quaternion.Euler(0,0,-castAngle/2) * -transform.up * targetDistanceAwayFromWall + transform.position - distanceTolerence * transform.up;

        Physics.Raycast(topLeftCheckPosition + Vector3.forward, -transform.forward, out topLeftHit);
        Physics.Raycast(bottomLeftCheckPosition + Vector3.forward, -transform.forward, out bottomLeftHit);
        Physics.Raycast(topRightCheckPosition + Vector3.forward, -transform.forward, out topRightHit);
        Physics.Raycast(bottomRightCheckPosition + Vector3.forward, -transform.forward, out bottomRightHit);

        if(topLeftHit.collider && topRightHit.collider)
        {
            transform.position += moveFromWallSpeed * transform.up * Time.fixedDeltaTime;
        }else if(topLeftHit.collider)
        {
            transform.RotateAround(topRightCheckPosition, transform.forward, turningSpeed * Time.fixedDeltaTime);
        }else if(topRightHit.collider)
        {
            transform.RotateAround(topLeftCheckPosition, transform.forward, -turningSpeed * Time.fixedDeltaTime);
            
        }else if(bottomLeftHit.collider == null && bottomRightHit.collider == null)
        {
            transform.position += moveFromWallSpeed * -transform.up * Time.fixedDeltaTime;
        }else if(bottomLeftHit.collider == null)
        {
            transform.RotateAround(bottomRightCheckPosition, transform.forward, -turningSpeed * Time.fixedDeltaTime);
        }else if(bottomRightHit.collider == null)
        {
            transform.RotateAround(bottomLeftCheckPosition, transform.forward, turningSpeed * Time.fixedDeltaTime);
        }

        transform.position += moveSpeed * direction * Time.fixedDeltaTime;
    }


    public void OnDrawGizmos()
    {
        const float TARGET_SPHERE_RADIUS = .1f;

        // Top Left DrawLine
        Gizmos.DrawLine(topLeftCheckPosition + Vector3.forward, topLeftCheckPosition - Vector3.forward);

        // Bottom Left DrawLine
        Gizmos.DrawLine(bottomLeftCheckPosition + Vector3.forward, bottomLeftCheckPosition - Vector3.forward);

        //Top Right DrawLine
        Gizmos.DrawLine(topRightCheckPosition + Vector3.forward, topRightCheckPosition - Vector3.forward);

        //Bottom Right DrawLine
        Gizmos.DrawLine(bottomRightCheckPosition + Vector3.forward, bottomRightCheckPosition - Vector3.forward);

        Gizmos.color = Color.red;
        // Top Left Target Point
        Gizmos.DrawSphere(topLeftCheckPosition, TARGET_SPHERE_RADIUS);

        // Bottom Left Target Point
        Gizmos.DrawSphere(bottomLeftCheckPosition, TARGET_SPHERE_RADIUS);

        // Top Right Target Point
        Gizmos.DrawSphere(topRightCheckPosition, TARGET_SPHERE_RADIUS);

        // Bottom Right Target Point
        Gizmos.DrawSphere(bottomRightCheckPosition, TARGET_SPHERE_RADIUS);
    }
}
