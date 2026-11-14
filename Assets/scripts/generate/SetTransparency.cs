using UnityEngine;

public class CubeTransparencyHotkey : MonoBehaviour
{
    public Material outlineMaterial;
    public Material transparentMaterial;
    bool isTransparent = true;

    public void ResetStateToTransparent()
    {
        isTransparent = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            ToggleTransparency();
    }

    void ToggleTransparency()
    {
        GameObject main = GameObject.Find("MAIN_CUBE");
        if (main == null) return;

        Renderer rend = main.GetComponent<Renderer>();

        if (!isTransparent)
        {
            // átváltás átlátszóra
            rend.material = transparentMaterial;
        }
        else
        {
          // átváltás nem átlátszóra
            rend.material = outlineMaterial;
        }

        isTransparent = !isTransparent;
    }
}