using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;       // La telecamera del giocatore
    public Camera portalCamera;     // La telecamera ausiliaria (es. Camera_Portal_Blue)

    [Header("Material Settings")]
    public Material portalMaterial; // Il materiale applicato alla superficie del portale (es. Mat_PortalScreen_Red)
    public string texturePropertyName = "_BaseMap"; // O "_MainTex" se usi la Built-in Pipeline

    private RenderTexture currentRenderTexture;

    void Start()
    {
        // Se non l'abbiamo assegnata, cerca la Main Camera in automatico
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Assicuriamoci che il materiale sia un'istanza separata (importante per non modificare l'asset globale)
        portalMaterial = new Material(portalMaterial);
        
        // Se c'è una mesh renderer su questo oggetto, assegniamogli il materiale istanziato
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = portalMaterial;
        }

        UpdateRenderTexture();
    }

    void Update()
    {
        // Controlliamo ad ogni frame se la risoluzione dello schermo è cambiata
        if (currentRenderTexture != null && 
           (currentRenderTexture.width != Screen.width || currentRenderTexture.height != Screen.height))
        {
            UpdateRenderTexture();
        }
    }

    void UpdateRenderTexture()
    {
        // 1. Rilascia e distruggi la vecchia Render Texture se esiste, per liberare memoria
        if (currentRenderTexture != null)
        {
            currentRenderTexture.Release();
            Destroy(currentRenderTexture);
        }

        // 2. Crea una nuova Render Texture identica alla risoluzione dello schermo
        currentRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        
        // 3. (Opzionale ma raccomandato) Disattiva l'anti-aliasing sulla texture per risparmiare performance, 
        // oppure impostalo per migliorare la qualità se il PC lo regge.
        currentRenderTexture.antiAliasing = 1; 
        currentRenderTexture.name = "Dynamic_RT_for_" + gameObject.name;

        // 4. Assegna la nuova texture come "Target" alla telecamera ausiliaria
        if (portalCamera != null)
        {
            portalCamera.targetTexture = currentRenderTexture;
            portalCamera.aspect = mainCamera.aspect;
        }

        // 5. Inserisci la nuova texture nel materiale che si trova fisicamente sul portale
        if (portalMaterial != null)
        {
            portalMaterial.SetTexture(texturePropertyName, currentRenderTexture);
        }
    }

    private void OnDestroy()
    {
        // Pulizia fondamentale quando distruggi l'oggetto o chiudi il gioco
        if (currentRenderTexture != null)
        {
            currentRenderTexture.Release();
            Destroy(currentRenderTexture);
        }
    }
}