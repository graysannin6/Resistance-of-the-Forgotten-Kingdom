using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : Singleton<PlayerHealthManager>
{
    [SerializeField] private int maxHealth = 8;
    [SerializeField] private float knockbackAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;


    private int currentHealth;
    private bool canTakeDamage = true;
    private float lerpTimer;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    private string barRestoreColorHex = "#C9FF9E";
    private Color restoredColor;

    private KnockBack knockBack;
    private Flash flash;

    protected override void Awake()
    {
        base.Awake();
        knockBack = GetComponent<KnockBack>();
        flash = GetComponent<Flash>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        ShooterEnemy shooterEnemy = collision.gameObject.GetComponent<ShooterEnemy>();
        Boss boss = collision.gameObject.GetComponent<Boss>();
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.otherCollider is CapsuleCollider2D)
        {
            TakeDamage(1, collision.transform);
            Debug.Log("Player took damage");

        }
    }

    public void HealPlayer()
    {
        currentHealth += 1;
    }

    public void TakeDamage(int damage, Transform hitTransform)
    {
        if (!canTakeDamage)
        {
            return;
        }
        ScreenShakeManager.Instance.ShakeScreen();
        knockBack.Knockback(hitTransform, knockbackAmount);
        StartCoroutine(flash.FlashWhite());
        canTakeDamage = false;
        currentHealth -= damage;
        lerpTimer = 0;
        StartCoroutine(InvincibilityFrames());
    }

    private IEnumerator InvincibilityFrames()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    public void UpdateHealthUI()
    {
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float hFraction = (float)currentHealth / maxHealth;

        if (fillBack > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.black;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }
        else if (fillFront < hFraction)
        {
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;

            if (ColorUtility.TryParseHtmlString(barRestoreColorHex, out restoredColor))
            {
                backHealthBar.color = restoredColor;
            }
            else
            {
                Debug.Log("Color not parsed");
            }

            float percentComplete = lerpTimer / chipSpeed;
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, hFraction, percentComplete);
        }
    }
}

