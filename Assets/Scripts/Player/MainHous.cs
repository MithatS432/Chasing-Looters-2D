using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainHous : MonoBehaviour
{
    public Image houseHealhtBar;
    public float houseHealth = 5000f;
    private float maxHealth;

    public GameObject deathPanel;
    public GameObject restart;
    public GameObject quit;

    void Start()
    {
        maxHealth = houseHealth;
        UpdateUI();
    }

    private void Update()
    {
        if (houseHealth <= 0)
        {
            houseHealth = 0;
            UpdateUI();
            ShowDeathPanel();
        }
    }

    public void GetDamage(float damage)
    {
        houseHealth -= damage;
        if (houseHealth < 0) houseHealth = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (houseHealhtBar != null)
        {
            float fillValue = houseHealth / maxHealth;
            houseHealhtBar.fillAmount = fillValue;
        }
    }

    void ShowDeathPanel()
    {
        deathPanel.SetActive(true);
        restart.SetActive(true);
        quit.SetActive(true);
    }
}
