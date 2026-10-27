using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UpgradeManager : NetworkBehaviour
{
    public UpgradeStructs upgrades;

    [Header("Controls")]
    public InputActionReference purchaseTopUpgrade;
    public InputActionReference purchaseBottomUpgrade;
    public InputActionReference purchaseHeal;

    [Header("Panel")]
    public RectTransform upgradePanel;
    [SerializeField] TMP_Text upgradeCountDisplay;
    [SerializeField] float panelMoveSpeed;
    private bool showingUpgrades;
    private GameObject[] shownUpgrades = new GameObject[2];

    [Header("EXP System")]
    public float experience;
    public float requiredExperience;
    [SerializeField] float requiredMultiplier;
    public int upgradeCount;

    [Header("Script References")]
    [SerializeField] UpgradeSpawner spawner;
    [SerializeField] PlayerHealth health;

    [Header("Debug")]
    public InputActionReference gainExp;

    private void OnEnable()
    {
        purchaseTopUpgrade.action.Enable();
        purchaseBottomUpgrade.action.Enable();
        purchaseHeal.action.Enable();

        purchaseTopUpgrade.action.performed += PurchaseTopUpgrade;
        purchaseBottomUpgrade.action.performed += PurchaseBottomUpgrade;
        purchaseHeal.action.performed += PurchaseHeal;

        gainExp.action.Enable();
        gainExp.action.performed += GainExpDebug;
    }

    private void OnDisable()
    {
        if (!IsLocalPlayer) return;

        purchaseTopUpgrade.action.Disable();
        purchaseBottomUpgrade.action.Disable();
        purchaseHeal.action.Disable();

        purchaseTopUpgrade.action.performed -= PurchaseTopUpgrade;
        purchaseBottomUpgrade.action.performed -= PurchaseBottomUpgrade;
        purchaseHeal.action.performed -= PurchaseHeal;
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;

        if (experience >= requiredExperience)
        {
            experience -= requiredExperience;
            requiredExperience *= requiredMultiplier;
            upgradeCount++;
        }

        if (upgradeCount > 0)
        {
            upgradePanel.anchoredPosition = Vector3.Lerp(upgradePanel.anchoredPosition, new Vector3(-10, 0, 0), Time.deltaTime * panelMoveSpeed);
            if (!showingUpgrades)
            {
                showingUpgrades = true;
                ShowUpgrades();
            }
        }
        else
        {
            upgradePanel.anchoredPosition = Vector3.Lerp(upgradePanel.anchoredPosition, new Vector3(394, 0, 0), Time.deltaTime * panelMoveSpeed);
        }

        upgradeCountDisplay.text = "Upgrades Left\n" + upgradeCount;
    }

    private void ShowUpgrades()
    {
        for (int i = 0; i < 2; i++)
        {
            int selectedType = Random.Range(0, upgrades.upgrades.Length - 1);
            shownUpgrades[i] = spawner.SpawnUpgrade(upgrades.upgrades[selectedType], i);
        }
    }

    private void GainExpDebug(InputAction.CallbackContext context)
    {
        GainExp(1);
    }

    public void GainExp(float gain = 1)
    {
        experience += gain;
    }

    private void PurchaseTopUpgrade(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || upgradeCount <= 0) return;
    }

    private void PurchaseBottomUpgrade(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || upgradeCount <= 0) return;
    }

    private void PurchaseHeal(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer || upgradeCount <= 0) return;
        DoHeal();
    }

    public void DoHeal()
    {
        if (health.health >= health.maxHealth) return;

        health.health += health.maxHealth / 10;
        if (health.health > health.maxHealth) health.health = health.maxHealth;
        CommonUpgradeCleanup();
    }


    // -- Upgrade Effects -- //

    private void CommonUpgradeCleanup()
    {
        Destroy(shownUpgrades[0]);
        Destroy(shownUpgrades[1]);
        upgradeCount--;
        showingUpgrades = false;
    }

    // Max Health
    public void MaxHealthAdditive()
    {
        float beforeHealth = health.maxHealth;
        health.maxHealth += 3;
        health.health += health.maxHealth - beforeHealth;
        CommonUpgradeCleanup();
    }
    public void MaxHealthMultiplicative()
    {
        float beforeHealth = health.maxHealth;
        health.maxHealth *= 1.25f;
        health.health += health.maxHealth - beforeHealth;
        CommonUpgradeCleanup();
    }
    public void MaxHealthExponential()
    {
        float beforeHealth = health.maxHealth;
        health.maxHealth = Mathf.Pow(health.maxHealth, 1.1f);
        health.health += health.maxHealth - beforeHealth;
        CommonUpgradeCleanup();
    }
}
