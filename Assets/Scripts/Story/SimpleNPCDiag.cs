using UnityEngine;
using TMPro; // Avem nevoie de asta pentru textul frumos

public class SimpleNPC : MonoBehaviour
{
    [Header("UI Elementele")]
    public GameObject fereastraDialog; // Panel-ul (fundalul) textului
    public TMP_Text textDialog;        // Componenta de text in sine

    [Header("Ce spune NPC-ul?")]
    [TextArea(3, 10)] // Face casuta de text mai mare in Inspector
    public string mesajNPC = "Salutare, călătorule!";
    public GameObject enemy;

    private bool jucatorulEsteAproape = false;

    void Start()
    {
        // Ascundem fereastra la inceputul jocului
        if (fereastraDialog != null)
        {
            fereastraDialog.SetActive(false);
        }
        enemy.SetActive(false);
    }

    void Update()
    {
        // Daca jucatorul e in zona si apasa pe E
        if (jucatorulEsteAproape && Input.GetKeyDown(KeyCode.E))
        {
            // Verificam daca fereastra este deja deschisa sau nu
            bool esteDeschisa = fereastraDialog.activeSelf;

            // O deschidem daca era inchisa, o inchidem daca era deschisa
            fereastraDialog.SetActive(!esteDeschisa);

            // Daca abia am deschis-o, setam textul
            if (!esteDeschisa)
            {
                textDialog.text = mesajNPC;
                enemy.SetActive(true);
            }
        }
    }

    // Cand jucatorul intra in zona (trigger-ul) NPC-ului
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jucatorulEsteAproape = true;
        }
    }

    // Cand jucatorul iese din zona NPC-ului
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jucatorulEsteAproape = false;
            fereastraDialog.SetActive(false); // Inchidem dialogul automat daca pleaca
        }
    }
}