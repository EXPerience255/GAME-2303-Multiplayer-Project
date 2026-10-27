using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudCarrier : NetworkBehaviour
{
    [SerializeField] float dialSmoothSpeed;

    [Header("Class References")]
    [SerializeField] PlayerHealth health;
    [SerializeField] UpgradeManager experience;

    [Header("Object References")]
    [SerializeField] Image experienceDial;
    [SerializeField] Image healthDial;
    [SerializeField] TMP_Text healthDisplay;
    RectTransform hudCarrier;

    private void Awake()
    {
        hudCarrier = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (IsLocalPlayer) hudCarrier.anchoredPosition = Vector3.Lerp(hudCarrier.anchoredPosition, new Vector3(16, 16, 0), Time.deltaTime * dialSmoothSpeed);
        else return;

        experienceDial.fillAmount = Mathf.Lerp(experienceDial.fillAmount, experience.experience / experience.requiredExperience, Time.deltaTime * dialSmoothSpeed);
        healthDial.fillAmount = Mathf.Lerp(healthDial.fillAmount, health.health / health.maxHealth, Time.deltaTime * dialSmoothSpeed);

        healthDisplay.text = Mathf.Round(health.health) + "\n———\n" + Mathf.Round(health.maxHealth);
    }
}
