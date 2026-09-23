using UnityEngine;

public class Script_PortalCamera : MonoBehaviour
{
    [Header("Riferimenti Telecamere")]
    public Transform playerCamera;       // La Main Camera del giocatore
    public Camera portalCamera;          // La telecamera ausiliaria (quella che renderizza la texture)

    [Header("Riferimenti Portali")]
    public Transform inPortal;           // Il portale in cui il giocatore sta guardando (es. Portale Rosso)
    public Transform outPortal;          // Il portale da cui esce la vista (es. Portale Blu)

    void LateUpdate()
    {
        UpdateCamera();
        SetNearClipPlane();
    }

    void UpdateCamera()
    {

        /* Matrix4x4 playerMatrix = Matrix4x4.TRS(playerCamera.transform.position, playerCamera.transform.rotation, Vector3.one);
        Matrix4x4 inMatrix = Matrix4x4.TRS(inPortal.position, inPortal.rotation, Vector3.one);
        Matrix4x4 outMatrix = Matrix4x4.TRS(outPortal.position, outPortal.rotation, Vector3.one);

        
        Matrix4x4 relativeMatrix = inMatrix.inverse * playerMatrix;
        Matrix4x4 flipMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one); */
        Matrix4x4 m = outPortal.localToWorldMatrix 
                        * Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0))
                        * transform.worldToLocalMatrix
                        * playerCamera.transform.localToWorldMatrix;

        // Matrix4x4 finalMatrix = outMatrix * flipMatrix * relativeMatrix;

        portalCamera.transform.position = m.GetColumn(3); 
        portalCamera.transform.rotation = m.rotation;
    }

    void SetNearClipPlane()
    {
        // Questo passaggio è cruciale per la perfezione dell'engine:
        // Taglia via tutto ciò che si trova *dietro* al portale di uscita,
        // altrimenti la telecamera ausiliaria inquadrerebbe i muri dietro di esso.
        
        Transform clipPlane = outPortal;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, outPortal.position - portalCamera.transform.position));

        Vector3 camSpacePos = portalCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCamera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal);

        Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

        // Applica la matrice di proiezione obliqua alla telecamera ausiliaria
        portalCamera.projectionMatrix = portalCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }
}