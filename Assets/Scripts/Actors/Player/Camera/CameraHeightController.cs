using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeightController : Actor
{
    public Transform cameraParent;
    public float standingHeight = 0.5f;
    public float crouchHeight = -0.3f;
    public float smoothSpeed = 5f;

    private float targetHeight;

    private void Start()
    {
        targetHeight = standingHeight;
    }

    protected override void ActorUpdate()
    {
        Vector3 currentPos = cameraParent.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, targetHeight, currentPos.z);

        cameraParent.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * smoothSpeed);
    }

    public void PostureChange(Posture posture)
    {
        if (posture == Posture.Crouching)
        {
            targetHeight = crouchHeight;
        }
        else
        {
            targetHeight = standingHeight;

        }
    }
}
