using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Portal otherPortal;

    [SerializeField]
    private Renderer outlineRenderer;

    [SerializeField]
    private Color portalColour;

    [SerializeField]
    private LayerMask placementMask;

    private Material material;
    private new Renderer renderer;
    //private new BoxCollider collider;

    private void Awake()
    {
        //collider = GetComponent<BoxCollider>();
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    private void Start()
    {
        //PlacePortal(wallCollider, transform.position, transform.rotation);
        SetColour(portalColour);
    }

    public Portal GetOtherPortal()
    {
        return otherPortal;
    }

    public Color GetColour()
    {
        return portalColour;
    }

    public void SetColour(Color colour)
    {
        material.SetColor("_Colour", colour);
        outlineRenderer.material.SetColor("_Colour", colour);
    }

    public void SetMaskID(int id)
    {
        material.SetInt("_MaskID", id);
    }

    public void SetTexture(string str, Texture tex)
    {
        material.SetTexture(str, tex);
    }

    public bool IsRendererVisible()
    {
        return renderer.isVisible;
    }
    void OnBecameVisible()
    {
        enabled = true;
    }
    void OnBecameInvisible()
    {
        enabled = false;
    }
}
