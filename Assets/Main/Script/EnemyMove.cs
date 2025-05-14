using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyMove : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private float speed = 2f;
    private float[] scaleSpeed = { 1, 0.8f, 0.6f, 0.4f, 0.2f };
    private Animator animator;
    private Enemy enemy;

    private GameObject detectionZone;
    private Collider2D detectionCollider;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 3f;

    void Start()
    {
        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();

        int scale = enemy.ReturnScale();
        speed *= scaleSpeed[scale];

        // Tạo detection zone độc lập
        detectionZone = new GameObject("DetectionZone");
        detectionZone.transform.position = transform.position;

        CircleCollider2D circle = detectionZone.AddComponent<CircleCollider2D>();
        circle.isTrigger = true;
        circle.radius = detectionRadius;

        detectionCollider = circle;
    }

    void Update()
    {
        if (enemy.IsDead()) Destroy(detectionZone);
        if (detectionZone != null)
            detectionZone.transform.position = transform.position;

        bool isInRange = false;
        if (playerTransform != null && detectionCollider != null)
        {
            isInRange = detectionCollider.bounds.Contains(playerTransform.position);
        }

        animator.SetBool("isRunning", isInRange);

        if (isInRange)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            float x = Math.Abs(transform.localScale.x);
            float y = transform.localScale.y;
            float z = transform.localScale.z;

            if (direction.x > 0)
                transform.localScale = new Vector3(x, y, z);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-x, y, z);

            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }
}
