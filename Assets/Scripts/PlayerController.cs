using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isMoving;
    private Vector2 input;
    public float moveSpeed = 1f;
    public LayerMask solidObjectsLayer;

    // Update is called once per frame
    private void Update()
    {
        if (!isMoving)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (horizontalInput != 0 || verticalInput != 0)
            {
                horizontalInput = verticalInput == 0 ? horizontalInput : 0;
                Vector3 inputDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;
                Vector3 targetPosition = transform.position + inputDirection;

                if (isWalkable(targetPosition))
                {
                    StartCoroutine(Move(targetPosition));
                }
            }
        }
    }

    private bool isWalkable(Vector3 targetPosition)
    {
        if (Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }
        return true;
    }
    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
    }
}
