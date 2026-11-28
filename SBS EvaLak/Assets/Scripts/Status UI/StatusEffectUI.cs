using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image fillImage; // Image Type must be set to Filled
    [SerializeField] private float moveSpeed = 10f;

    private StatusEffect effect;
    private Vector3 targetPosition;

    // Called by the manager when the icon is created
    public void Init(StatusEffect effect, Vector3 position)
    {
        this.effect = effect;
        transform.position = position;
        targetPosition = position;
    }

    // Icons glide to their new slot when one above them expires
    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
    }

    private void Update()
    {
        if (effect != null && effect.MaxTurns > 0)
            fillImage.fillAmount = (float)effect.turnsLeft / effect.MaxTurns;

        float t = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, t);
    }
}