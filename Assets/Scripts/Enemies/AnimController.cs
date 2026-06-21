using UnityEngine;

public class AnimatiiInamic : MonoBehaviour
{
    [Header("Componente necesare")]
    public Animator animator;
    public UrmarireInamic scriptMiscare; // Avem nevoie de el ca să-l oprim la moarte

    [Header("Status")]
    public EnemyHealth enemyHealth;
    public bool esteMort;
    public float viataCurenta = 0f;
    public float distantaDeAtac = 1.5f; // Asigură-te că e la fel ca în scriptul de mișcare!

    private Transform player;

    void Start()
    {
        // Caută player-ul automat
        GameObject playerGasit = GameObject.FindGameObjectWithTag("Player");
        if (playerGasit != null)
        {
            player = playerGasit.transform;
        }

        // Dacă ai uitat să le tragi în Inspector, scriptul le ia singur
        if (animator == null) animator = GetComponent<Animator>();
        if (scriptMiscare == null) scriptMiscare = GetComponent<UrmarireInamic>();
    }

    void Update()
    {
        esteMort = enemyHealth.generalHealth.isDead;
        viataCurenta = enemyHealth.generalHealth.currentHealth;
        // Dacă e mort sau n-a găsit jucătorul, oprim complet logica
        if (esteMort || player == null) return;

        // Verificăm dacă viața a ajuns la zero
        if (viataCurenta <= 0)
        {
            Mori();
            return;
        }

        // Măsurăm distanța pentru a ști ce animație redăm
        float distanta = Vector3.Distance(transform.position, player.position);

        if (distanta > distantaDeAtac)
        {
            // Dacă e departe, pornim Run și oprim Attack
            animator.SetBool("run", true);
            animator.SetBool("attack", false);
        }
        else
        {
            // Dacă a ajuns lângă tine, se oprește și atacă
            animator.SetBool("run", false);
            animator.SetBool("attack", true);
        }
    }

    // O funcție publică pe care o poți apela din alte scripturi când îl lovești
    public void PrimesteDamage(float damage)
    {
        if (esteMort) return;
        viataCurenta -= damage;
    }

    private void Mori()
    {
        esteMort = true;

        // Declanșăm animația de moarte (folosim Trigger pentru că se întâmplă o singură dată)
        animator.SetTrigger("die");

        // Oprim scriptul de urmărire ca să nu se mai miște spre tine după ce a murit
        if (scriptMiscare != null)
        {
            scriptMiscare.enabled = false;
        }

        // Opțional: îi dezactivăm coliziunea ca jucătorul să poată trece peste cadavru
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}