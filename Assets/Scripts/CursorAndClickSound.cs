using UnityEngine;

public class CursorAndClickSound : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("Click Sound Settings")]
    public AudioSource clickAudio;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (clickAudio != null && clickAudio.clip != null)
            {
                clickAudio.PlayOneShot(clickAudio.clip);
            }
        }
    }
}
