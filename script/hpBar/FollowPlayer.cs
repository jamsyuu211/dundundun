using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public Vector3 offset; // 플레이어와의 오프셋

    void Update()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position + offset;
            if (IsValidPosition(newPosition))
            {
                transform.position = newPosition;
            }
        }
    }

    bool IsValidPosition(Vector3 position)
    {
        return !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z) &&
               Mathf.Abs(position.x) < 10000f && Mathf.Abs(position.y) < 10000f && Mathf.Abs(position.z) < 10000f;
    }

}
