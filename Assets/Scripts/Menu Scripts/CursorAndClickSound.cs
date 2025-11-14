using UnityEngine;

public class CursorAndClickSound : MonoBehaviour
{
    [Header("Cursor Settings")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("Click Sound Settings")]
    public AudioSource clickAudio;
    public GameObject[] effectPrefabs;
    private int effectIndex = 0;

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

            if (effectPrefabs != null && effectPrefabs.Length > 0)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = Camera.main.nearClipPlane + 0.5f;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0f;

                GameObject effect = Instantiate(effectPrefabs[effectIndex], worldPosition, Quaternion.identity);
                Destroy(effect, 3f);

                effectIndex++;
                if (effectIndex >= effectPrefabs.Length)
                    effectIndex = 0;
            }
        }
    }
}
