using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct Upgrade
{
    public int rarity;
    public UnityEvent effect;
    public GameObject button;
}

[Serializable]
public struct UpgradeType
{
    public string name;
    public Upgrade[] upgrades; // MUST be in the order of { +, *, ^ }
}

public class UpgradeStructs : MonoBehaviour
{
    public UpgradeType[] upgrades;
}
