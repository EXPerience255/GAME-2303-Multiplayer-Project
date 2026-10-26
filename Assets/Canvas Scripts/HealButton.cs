using UnityEngine;

public class HealButton : MonoBehaviour
{
    [SerializeField] UpgradeManager upgrades;
    [SerializeField] PlayerHealth health;

    public void Heal()
    {
        health.health += health.maxHealth / 10;
        if (health.health > health.maxHealth) health.health = health.maxHealth;
        upgrades.upgradeCount--;
    }
}
