using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{

    public Transform linkedPortal;
    public Camera playerCamera;
    public Camera portalCamera;
    public MeshRenderer portalRenderer;

    public RenderTexture renderTexture;
    private Material portalMaterial;

    private MeshRenderer linkedPortalRenderer;
    private Material linkedPortalMaterial;
    private Texture2D fallbackGreyTexture;

    void Awake()
    {
        portalMaterial = portalRenderer.material;

        portalCamera.enabled = false;

        renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        portalCamera.targetTexture = renderTexture;
        portalMaterial.SetTexture("_MainTex", renderTexture);

        // Recupera il renderer del portale collegato
        if (linkedPortal != null)
        {
            linkedPortalRenderer = linkedPortal.GetComponent<MeshRenderer>();
            if (linkedPortalRenderer != null)
            {
                linkedPortalMaterial = linkedPortalRenderer.material;
            }
        }

        // Genera una texture 1x1 grigia a runtime
        fallbackGreyTexture = new Texture2D(1, 1);
        fallbackGreyTexture.SetPixel(0, 0, Color.gray);
        fallbackGreyTexture.Apply();
    }

    void LateUpdate()
    {
        // Position and Rotation (LocalToWorld)
        Matrix4x4 m = linkedPortal.localToWorldMatrix 
                        * Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0))
                        * transform.worldToLocalMatrix
                        * playerCamera.transform.localToWorldMatrix;

        portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        portalCamera.fieldOfView = playerCamera.fieldOfView;
        portalCamera.aspect = playerCamera.aspect;
        portalCamera.nearClipPlane = playerCamera.nearClipPlane;
        portalCamera.farClipPlane = playerCamera.farClipPlane;

        //SetObliquePlane();

        // 1. Sostituisce la texture del portale d'uscita con il grigio
        if (linkedPortalMaterial != null)
        {
            linkedPortalMaterial.SetTexture("_MainTex", fallbackGreyTexture);
        }

        // 2. Esegue il rendering della visuale interna del portale
        portalCamera.Render();

        // 3. Ripristina la RenderTexture originale sul portale collegato
        PortalBehaviour linkedScript = linkedPortal.GetComponent<PortalBehaviour>();
        if (linkedPortalMaterial != null && linkedScript != null)
        {
            linkedPortalMaterial.SetTexture("_MainTex", linkedScript.renderTexture);
        }
    }

    private void SetObliquePlane()
    {
        // Il piano del portale nello spazio globale del portale d'uscita
        Transform destinationTransform = linkedPortal.transform;
        
        // Direzione normale del portale (in avanti rispetto al quad)
        Vector3 planeNormal = destinationTransform.forward;
        Vector3 planePosition = destinationTransform.position;

        // Convertiamo il piano nello spazio vista (Camera Space) della telecamera del portale
        Vector3 cameraSpacePos = portalCamera.worldToCameraMatrix.MultiplyPoint(planePosition);
        Vector3 cameraSpaceNormal = portalCamera.worldToCameraMatrix.MultiplyVector(planeNormal).normalized;

        // Equazione del piano: dot(normal, pos) + d = 0 => d = -dot(normal, pos)
        float distance = -Vector3.Dot(cameraSpaceNormal, cameraSpacePos);
        Vector4 clipPlane = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, distance);

        // Calcola la matrice di proiezione modificata con il piano obliquo
        portalCamera.projectionMatrix = portalCamera.CalculateObliqueMatrix(clipPlane);
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    /* private void SetNearClipPlane()
    {
        Transform clipPlane = linkedPortal;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, linkedPortal.position - portalCamera.transform.position));

        Vector3 camSpacePos = portalCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCamera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal);
        Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

        portalCamera.projectionMatrix = playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    } */
}
