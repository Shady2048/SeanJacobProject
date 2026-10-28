using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbarController : MonoBehaviour
{
    [Header("Enemy Healthbar")]
    public Slider healthbar;

    [Header("Healthbar Text")]
    public TextMeshProUGUI healthbarText;

    public void UpdateHealthbar(int currentHP, int maxHP)
    {
        healthbar.maxValue = maxHP;
        healthbar.value = currentHP;
        healthbarText.text = currentHP + "/" + maxHP;
    }

}
