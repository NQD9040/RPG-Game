using UnityEngine;

public class CritValue : MonoBehaviour
{
    public static CritValue Instance { get; private set; }

    [Header("Crit Settings")]
    public float critRate = 25f;
    public float critDamage = 100f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
