using System.Collections.Generic;
using UnityEngine;

// Base for anything that lives in the scene and triggers once per turn
public abstract class StatusEffect : MonoBehaviour
{
    [SerializeField] private StatusEffectUI uiPrefab; // Icon shown while this effect is active

    public int turnsLeft = 3;

    public int MaxTurns { get; private set; } // Duration it started with, used for the UI fill
    public Healthbar Target { get; private set; }
    public StatusEffectUI UIPrefab => uiPrefab;

    private static readonly List<StatusEffect> active = new List<StatusEffect>();

    // Copy, so effects can be destroyed while Deck loops over them
    public static List<StatusEffect> GetAll()
    {
        return new List<StatusEffect>(active);
    }

    protected virtual void OnEnable()
    {
        active.Add(this);
    }

    protected virtual void OnDisable()
    {
        active.Remove(this);

        // Removes the icon, null check because the manager may already be gone when a scene unloads
        if (StatusEffectUIManager.Instance != null)
            StatusEffectUIManager.Instance.Remove(this);
    }

    // Subclasses call this first in their Init, it sets the target and duration and creates the icon
    protected void Begin(Healthbar target, int turns)
    {
        Target = target;
        turnsLeft = turns;
        MaxTurns = turns;

        if (StatusEffectUIManager.Instance != null)
            StatusEffectUIManager.Instance.Add(this);
    }

    // Called by Deck once per turn
    public void Tick()
    {
        OnTick();

        turnsLeft--;
        if (turnsLeft <= 0)
        {
            Debug.Log("Status effect expired");
            Destroy(gameObject);
        }
    }

    protected abstract void OnTick();
}