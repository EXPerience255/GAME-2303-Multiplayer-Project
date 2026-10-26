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
    [SerializeField] float panelMoveSpeed;

    [Header("EXP System")]
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
        if (upgradeCount > 0)
        {
            upgradePanel.anchoredPosition = Vector3.Lerp(upgradePanel.anchoredPosition, new Vector3(-10, 0, 0), Time.deltaTime * panelMoveSpeed);
        }
        else
        {
            upgradePanel.anchoredPosition = Vector3.Lerp(upgradePanel.anchoredPosition, new Vector3(394, 0, 0), Time.deltaTime * panelMoveSpeed);
        }
    }

    private void PurchaseTopUpgrade(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;

        upgradeCount++;
    }

    private void PurchaseBottomUpgrade(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;

        upgradeCount--;
    }

    private void PurchaseHeal(InputAction.CallbackContext context)
    {
        if (!IsLocalPlayer) return;

        upgradeCount = 0;
    }
}
