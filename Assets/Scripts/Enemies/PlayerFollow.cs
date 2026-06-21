using UnityEngine;

public class UrmarireInamic : MonoBehaviour
{
    [Header("Setări Mișcare")]
    public float viteza = 3f;
    public float distantaDeOprire = 1.5f;

    private Transform player;

    void Start()
    {
        GameObject playerGasit = GameObject.FindGameObjectWithTag("Player");

        if (playerGasit != null)
        {
            player = playerGasit.transform;
        }
        else
        {
            Debug.LogError("Inamicul nu știe pe cine să urmărească! Asigură-te că personajul tău are tag-ul 'Player'.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // SECRETUL NOU: Creăm două puncte imaginare la nivelul 0 (pe podea) pentru a măsura distanța corect
            Vector3 pozitieInamicPlat = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 pozitiePlayerPlat = new Vector3(player.position.x, 0, player.position.z);

            // Acum măsurăm distanța reală de pe pământ, fără înălțime
            float distantaReala = Vector3.Distance(pozitieInamicPlat, pozitiePlayerPlat);

            if (distantaReala > distantaDeOprire)
            {
                // E prea departe -> MERGE SPRE TINE
                Vector3 tintaLaNivelulPodelei = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.position = Vector3.MoveTowards(transform.position, tintaLaNivelulPodelei, viteza * Time.deltaTime);
                transform.LookAt(tintaLaNivelulPodelei);
            }
            else
            {
                // A ajuns la tine -> SE OPREȘTE
                Debug.Log("M-AM OPRIT! Distanța reală este: " + distantaReala);
            }
        }
    }
}