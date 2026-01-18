using UnityEngine;
using UnityEngine.VFX;

public class ClickVFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;

    public void Play(Vector2 screenPosition)
    {
        Vector3 pos = new Vector3(screenPosition.x, screenPosition.y, 0f);
        vfx.SetVector3("ClickPosition", pos);
        vfx.Play();
    }
}
