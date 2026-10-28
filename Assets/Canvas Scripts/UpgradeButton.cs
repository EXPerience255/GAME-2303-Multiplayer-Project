using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    private UpgradeManager upgradeManager;
    public string upgradeName;
    public int upgradeRarity;

    private void Awake()
    {
        upgradeManager = GetComponentInParent<UpgradeSpawner>().GetComponentInParent<UpgradeManager>();
    }

    public void PerformPurchase()
    {
        foreach (UpgradeType group in upgradeManager.upgrades.upgrades)
        {
            if (group.name == upgradeName)
            {
                foreach (Upgrade upgrade in group.upgrades)
                {
                    if (upgrade.rarity == upgradeRarity)
                    {
                        upgrade.effect.Invoke();
                        return;
                    }
                }
            }
        }
    }
}
