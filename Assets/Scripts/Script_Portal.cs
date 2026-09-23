using UnityEngine;

public class Script_Portal : MonoBehaviour
{

    public Transform player;
    public Transform linkedPortal;
    public Transform selfPortal;
    public Camera playerCamera;
    public Camera linkedCamera;
    public Camera selfCamera;

public RenderTexture renderTexture;

    void LateUpdate()
    {
        UpdatePortalCamera();
    }

    void UpdatePortalCamera()
    {
        Matrix4x4 relativeMatrix =  linkedPortal.localToWorldMatrix 
                                    * Matrix4x4.Rotate(Quaternion.Euler(0f, 180f, 0f))
                                    * selfPortal.worldToLocalMatrix;

        //selfCamera.transform.position = relativeMatrix.MultiplyPoint3x4(playerCamera.transform.position);

        Vector3 newForward = relativeMatrix.MultiplyVector(player.forward);
        Vector3 newUp = relativeMatrix.MultiplyVector(player.up);

        //transform.rotation = Quaternion.LookRotation(newForward, newUp);

        selfCamera.transform.rotation = Quaternion.LookRotation(newForward, newUp);
        /* Vector3 localPosition = selfPortal.InverseTransformPoint(player.position);
        linkedCamera.transform.position = linkedPortal.TransformPoint(localPosition);

        Vector3 localDirection = selfPortal.InverseTransformDirection(player.forward);
        Vector3 localUp = selfPortal.InverseTransformDirection(player.up);
        
        // Orienta la Telecamera Secondaria partendo dal Portale B
         */

        /* Matrix4x4 rotation180 =
            Matrix4x4.Rotate(
                Quaternion.Euler(0f, 180f, 0f)
            );

        Matrix4x4 matrix =
            linkedPortal.localToWorldMatrix *
            rotation180 *
            transform.worldToLocalMatrix *
            playerCamera.transform.localToWorldMatrix;

        //portalCamera.transform.position = matrix.GetColumn(3); 
        portalCamera.transform.rotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.inverse.GetColumn(1)); */
    }
}
