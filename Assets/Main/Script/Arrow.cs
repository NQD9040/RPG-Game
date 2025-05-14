using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    private float direction = 1f;
    float arrowDamage = 20f;
    private float critRate = 0f;
    private float critDamage = 0f;
    private PlayerController playerController;
    [SerializeField] float dmgMultiple = 1.2f;
    public void SetDirection(float dir)
    {
        direction = dir;
    }

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Destroy(gameObject, lifetime);
        critRate = CritValue.Instance.critRate;
        critDamage = CritValue.Instance.critDamage;
    }

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        float dmgBonus = playerController.isBoost ? (playerController.maxHP * playerController.hpPercentageBoost)/100 : 0;
        if (dmgBonus >= playerController.baseATK * 10)
        {
            dmgBonus = playerController.baseATK * 10;
        }
        float temp = playerController.baseATK + dmgBonus;
        temp = Random.Range(temp * 0.9f, temp * 1.1f) * dmgMultiple;
        arrowDamage = Mathf.Round(temp * 100) / 100f;
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnHitByPlayer(arrowDamage, critRate, critDamage);
                
            }
            Destroy(gameObject);
        }
    }
}
