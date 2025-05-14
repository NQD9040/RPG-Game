using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private Enemy enemy;
    private bool isPlayerInHitbox = false; // Theo dõi trạng thái người chơi

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy script not found in parent GameObject!");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInHitbox = true; // Người chơi vào vùng hitbox
            if (enemy != null)
            {
                enemy.Attack(); // Kích hoạt tấn công
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInHitbox = false; // Người chơi rời vùng hitbox
        }
    }

    public bool IsPlayerInHitbox()
    {
        return isPlayerInHitbox; // Trả về trạng thái người chơi trong hitbox
    }
}