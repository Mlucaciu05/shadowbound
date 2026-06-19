using UnityEngine;

public class BossHealthBarUI : MonoBehaviour
{
    public HealthBarUI healthBar;
    public GameObject root;
    public BossController currentBoss;

    void Awake()
    {
        if (root == null)
        {
            root = gameObject;
        }

        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBarUI>();
        }

        root.SetActive(false);
    }

    public void Show(BossController boss)
    {
        currentBoss = boss;

        if (healthBar != null && boss != null)
        {
            healthBar.Bind(boss.health);
        }

        if (root != null)
        {
            root.SetActive(boss != null);
        }
    }

    public void Hide()
    {
        currentBoss = null;

        if (root != null)
        {
            root.SetActive(false);
        }
    }
}
