using System.Collections;
using UnityEngine;

public class CardHover : MonoBehaviour
{
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float hoverHeight;

    private Vector3 startPosition;

    [SerializeField] private float delay = 0;
    [SerializeField] private bool canMove = false;

    private void Start()
    {
        startPosition = transform.position;
        canMove = true;
    }

    private void Update()
    {
        if (canMove)
        {
            float newY = startPosition.y + Mathf.Sin((Time.time + delay) * hoverSpeed) * hoverHeight;

            transform.position = new Vector3(startPosition.x, newY, hoverSpeed);
        }
    }
}
