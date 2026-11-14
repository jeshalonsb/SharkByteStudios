using System.Collections.Generic;
using UnityEngine;

// Base for anything that lives in the scene and triggers once per turn
public abstract class StatusEffect : MonoBehaviour
{
    public int turnsLeft = 3;

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
    }

    // Called by Deck once per turn
    public void Tick()
    {
        OnTick();

        turnsLeft--;
        if (turnsLeft <= 0)
            Debug.Log("Status effect expired");
            Destroy(gameObject);
    }

    protected abstract void OnTick();
}