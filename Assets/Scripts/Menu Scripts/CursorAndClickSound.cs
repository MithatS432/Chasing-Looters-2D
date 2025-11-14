using UnityEngine;

public class CursorAndClickSound : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("Click Sound Settings")]
    public AudioSource clickAudio;
    public GameObject purpleEffectPrefab;

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
            if (purpleEffectPrefab != null)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = Camera.main.nearClipPlane + 0.5f;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0f;
                GameObject purpleEffect = Instantiate(purpleEffectPrefab, worldPosition, Quaternion.identity);
                Destroy(purpleEffect, 3f);
            }
        }
    }
}
