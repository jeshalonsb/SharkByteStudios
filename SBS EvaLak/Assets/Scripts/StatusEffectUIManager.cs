using System.Collections.Generic;
using UnityEngine;

public class StatusEffectUIManager : MonoBehaviour
{
    public static StatusEffectUIManager Instance { get; private set; }

    [SerializeField] private Transform playerOrigin;
    [SerializeField] private Transform enemyOrigin;
    [SerializeField] private float spacing = 1f; // Each icon sits this far below the previous one

    private readonly List<StatusEffectUI> playerIcons = new List<StatusEffectUI>();
    private readonly List<StatusEffectUI> enemyIcons = new List<StatusEffectUI>();
    private readonly Dictionary<StatusEffect, StatusEffectUI> iconFor = new Dictionary<StatusEffect, StatusEffectUI>();

    private void Awake()
    {
        Instance = this;
    }

    // Called by StatusEffect when it starts
    public void Add(StatusEffect effect)
    {
        if (effect.UIPrefab == null || effect.Target == null) return;

        bool isPlayer = effect.Target.CompareTag("Player");
        Transform origin = isPlayer ? playerOrigin : enemyOrigin;
        List<StatusEffectUI> list = isPlayer ? playerIcons : enemyIcons;

        // New icons spawn directly in their slot at the bottom of the stack
        Vector3 position = SlotPosition(origin, list.Count);
        StatusEffectUI icon = Instantiate(effect.UIPrefab, position, Quaternion.identity);
        icon.Init(effect, position);

        list.Add(icon);
        iconFor[effect] = icon;
    }

    // Called by StatusEffect when it is destroyed, icons below it move up
    public void Remove(StatusEffect effect)
    {
        if (!iconFor.TryGetValue(effect, out StatusEffectUI icon)) return;

        iconFor.Remove(effect);
        playerIcons.Remove(icon);
        enemyIcons.Remove(icon);
        Destroy(icon.gameObject);

        Layout(playerIcons, playerOrigin);
        Layout(enemyIcons, enemyOrigin);
    }

    private void Layout(List<StatusEffectUI> list, Transform origin)
    {
        for (int i = 0; i < list.Count; i++)
            list[i].MoveTo(SlotPosition(origin, i));
    }

    private Vector3 SlotPosition(Transform origin, int index)
    {
        return origin.position + Vector3.down * spacing * index;
    }
}