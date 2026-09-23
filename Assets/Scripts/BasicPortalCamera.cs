using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPortalCamera : MonoBehaviour
{
    [SerializeField]
    private Portal portal0;
    [SerializeField]
    private Portal portal1;

    [SerializeField]
    private int recursionLimit = 7;

    [SerializeField]
    private Camera portalCamera;

    private Camera mainCamera;

    // Ogni portale ha la propria texture dedicata
    private RenderTexture texturePortal0;
    private RenderTexture texturePortal1;

    private Texture2D fallbackColorTexture0;
    private Texture2D fallbackColorTexture1;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        // Disattiviamo l'auto-render della camera per controllarla solo via codice
        portalCamera.enabled = false;

        // Creiamo le due texture con risoluzione dello schermo
        texturePortal0 = new RenderTexture(Screen.width, Screen.height, 24);
        texturePortal1 = new RenderTexture(Screen.width, Screen.height, 24);

        fallbackColorTexture0 = new Texture2D(1, 1);
        fallbackColorTexture1 = new Texture2D(1, 1);
        fallbackColorTexture0.SetPixel(0, 0, portal0.GetColour());
        fallbackColorTexture1.SetPixel(0, 0, portal1.GetColour());
        fallbackColorTexture0.Apply();
        fallbackColorTexture1.Apply();
    }

    private void Start()
    {
        // Assegniamo a ciascun portale la texture in cui scriveremo la sua vista
        // (Assicurati che Portal.cs abbia un metodo per impostare la texture o il materiale)
        portal0.SetTexture("_BaseMap", texturePortal0);
        portal1.SetTexture("_BaseMap", texturePortal1);
    }

    private void LateUpdate()
    {
        // Se il portale 0 è a vista, la camera va dietro al portale 1 e scrive in texturePortal0
        if (portal0.IsRendererVisible())
        {
            portalCamera.targetTexture = texturePortal0;
            for (int i = recursionLimit - 1; i >= 0; --i)
            {
                if (i == recursionLimit - 1)
                {
                    portal0.SetTexture("_BaseMap", fallbackColorTexture0);
                }
                else
                {
                    // Nei livelli successivi ripristina la texture calcolata
                    portal0.SetTexture("_BaseMap", texturePortal0);
                }
                RenderCamera(portal0, portal1, i);
            }
        }

        // Se il portale 1 è a vista, la camera va dietro al portale 0 e scrive in texturePortal1
        if (portal1.IsRendererVisible())
        {
            portalCamera.targetTexture = texturePortal1;
            for (int i = recursionLimit - 1; i >= 0; --i)
            {
                if (i == recursionLimit - 1)
                {
                    portal1.SetTexture("_BaseMap", fallbackColorTexture1);
                }
                else
                {
                    // Nei livelli successivi ripristina la texture calcolata
                    portal1.SetTexture("_BaseMap", texturePortal1);
                }
                RenderCamera(portal1, portal0, i);
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iteration)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for(int i = 0; i <= iteration; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = 
                Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        portalCamera.Render();
        
        /* Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        // Posizionamento e rotazione della camera
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        portalCamera.transform.position = outTransform.TransformPoint(relativePos);

        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        portalCamera.transform.rotation = outTransform.rotation * relativeRot;

        // Reset della matrice prima di calcolare l'obliqua
        portalCamera.ResetProjectionMatrix();

        // Near-clip plane obliquo per non vedere oggetti dietro il portale d'uscita
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Esegue il rendering della visuale
        portalCamera.Render(); */
    }

    private void OnDestroy()
    {
        // Buona pratica: rilasciare le RenderTexture dalla memoria GPU
        if (texturePortal0 != null) texturePortal0.Release();
        if (texturePortal1 != null) texturePortal1.Release();
    }
}