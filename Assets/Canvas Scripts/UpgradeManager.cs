using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeManager : NetworkBehaviour
{
    [SerializeField] UpgradeStructs upgrades;

    [Header("Controls")]
    public InputActionReference purchaseTopUpgrade;
    public InputActionReference purchaseBottomUpgrade;
    public InputActionReference purchaseHeal;

    [Header("Panel")]
    public RectTransform upgradePanel;
    [SerializeField] HealButton heal;
    [SerializeField] float panelMoveSpeed;

    [Header("EXP System")]
    public float experience;
    public float requiredExperience;
    [SerializeField] float requiredMultiplier;
    public int upgradeCount;

    private void OnEnable()
    {
        purchaseTopUpgrade.action.Enable();
        purchaseBottomUpgrade.action.Enable();
        purchaseHeal.action.Enable();

        purchaseTopUpgrade.action.performed += PurchaseTopUpgrade;
        purchaseBottomUpgrade.action.performed += PurchaseBottomUpgrade;
        purchaseHeal.action.performed += PurchaseHeal;
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
        }
        else
        {
            upgradePanel.anchoredPosition = Vector3.Lerp(upgradePanel.anchoredPosition, new Vector3(394, 0, 0), Time.deltaTime * panelMoveSpeed);
        }
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
        heal.Heal();
    }
}
