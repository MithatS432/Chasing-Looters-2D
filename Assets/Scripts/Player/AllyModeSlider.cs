using UnityEngine;
using UnityEngine.UI;

public class AllyModeSlider : MonoBehaviour
{
    public Slider allyModeSlider;
    public Player player;

    void Start()
    {
        allyModeSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        if (player != null)
        {
            player.SetAllyBehavior(value);
        }
    }
}
