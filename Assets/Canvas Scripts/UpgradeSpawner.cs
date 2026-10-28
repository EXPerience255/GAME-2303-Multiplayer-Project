using UnityEngine;

public class UpgradeSpawner : MonoBehaviour
{
    [SerializeField] float multiplierChance;
    [SerializeField] float exponentChance;

    public GameObject SpawnUpgrade(UpgradeType upgrades, int index)
    {
        float raritySelector = Random.Range(0f, 1f);
        Upgrade upgrade;

        Debug.Log(index + ", " + raritySelector);
        if (raritySelector > 1 - exponentChance) upgrade = upgrades.upgrades[2];
        else if (raritySelector > 1 - exponentChance - multiplierChance) upgrade = upgrades.upgrades[1];
        else upgrade = upgrades.upgrades[0];

        RectTransform upgradeTransform = Instantiate
        (
            upgrade.button, 
            new Vector3(0, 0, 0), 
            Quaternion.identity, 
            gameObject.GetComponent<RectTransform>()
        )
        .GetComponent<RectTransform>();

        switch (index)
        {
            case 0:
                upgradeTransform.anchoredPosition = new Vector3(0, 256, 0);
                break;
            case 1:
                upgradeTransform.anchoredPosition = new Vector3(0, -128, 0);
                break;
            default:
                Debug.LogError("UpgradeSpawner.cs, SpawnUpgrade(): Index was " + index + ", which is out of bounds! Index should be 0 or 1.");
                break;
        }

        return upgradeTransform.gameObject;
    }
}
