using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("2D Movement")]
    public float moveSpeed = 7f;
    public Vector2 minBounds = new Vector2(-8f, -4.5f);
    public Vector2 maxBounds = new Vector2(8f, 4.5f);

    [Header("Weapons (0 = Single Laser, 1 = Triple Spread)")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int currentWeapon = 0;
    public float fireRate = 0.25f;

    private float nextFireTime = 0f;
    private float powerUpTimer = 0f;

    void Update()
    {
        MovePlayer();
        HandleShooting();
        HandlePowerUpTimer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0f).normalized * moveSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + movement;

        // Clamp within screen boundaries
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        transform.position = newPos;
    }

    void HandleShooting()
    {
        // Press Tab or Q to manually switch between the 2 weapon types
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentWeapon = (currentWeapon == 0) ? 1 : 0;
        }

        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireWeapon();
        }
    }

    void FireWeapon()
    {
        if (currentWeapon == 0)
        {
            // Weapon 1: Standard Straight Laser
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
        else if (currentWeapon == 1)
        {
            // Weapon 2: 3-Way Spread Shot
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, 15f));
            Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, -15f));
        }
    }

    void HandlePowerUpTimer()
    {
        if (powerUpTimer > 0)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                fireRate = 0.25f; // Reset fire rate when power-up expires
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.LoseLife();
        }
        else if (other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
            fireRate = 0.1f; // Rapid fire boost
            currentWeapon = 1; // Upgrade to spread shot
            powerUpTimer = 5f;
        }
    }
}
