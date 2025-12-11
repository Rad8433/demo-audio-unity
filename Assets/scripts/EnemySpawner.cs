using UnityEngine;

public class GenerateurEnnemis : MonoBehaviour
{
    [Header("Paramètres d'apparition")]
    public GameObject prefabEnnemi;          // Prefab de l’ennemi à instancier
    public float intervalleApparition = 1.5f;
    public float plageSpawnX = 8f;           // Largeur de la zone où les ennemis peuvent apparaître

    [Header("Paramètres des ennemis")]
    public float vitesseEnnemi = 3f;
    public float positionDestructionY = -6f;

    [Header("Audio")]
    public AudioClip sonApparition;          // Son joué à chaque spawn
    [Range(0f, 1f)]
    public float volumeSonApparition = 1f;   // Volume utilisé pour PlayClipAtPoint

    private float minuterie = 0f;

    private void Update()
    {
        minuterie += Time.deltaTime;

        if (minuterie >= intervalleApparition)
        {
            minuterie = 0f;
            ApparaitreEnnemi();
        }
    }

    void ApparaitreEnnemi()
    {
        float randomX = Random.Range(-plageSpawnX, plageSpawnX);
        Vector3 posSpawn = new Vector3(randomX, transform.position.y, 0f);

        GameObject ennemi = Instantiate(prefabEnnemi, posSpawn, Quaternion.identity);

        DeplacementEnnemi deplacement = ennemi.AddComponent<DeplacementEnnemi>();
        deplacement.vitesse = vitesseEnnemi;
        deplacement.positionDestructionY = positionDestructionY;

        // Jouer le son d'apparition via PlayClipAtPoint (ne nécessite pas d'AudioSource attaché)
        if (sonApparition == null)
        {
            Debug.LogWarning("[GenerateurEnnemis] sonApparition non assigné !");
            return;
        }

        Vector3 listenPos = (Camera.main != null) ? Camera.main.transform.position : transform.position;
        AudioSource.PlayClipAtPoint(sonApparition, listenPos, Mathf.Clamp01(volumeSonApparition));
        Debug.Log("[GenerateurEnnemis] PlayClipAtPoint appelé pour le son d'apparition.");
    }
}

public class DeplacementEnnemi : MonoBehaviour
{
    public float vitesse = 3f;
    public float positionDestructionY = -6f;

    private void Update()
    {
        transform.position += Vector3.down * vitesse * Time.deltaTime;

        if (transform.position.y <= positionDestructionY)
        {
            Destroy(gameObject);
        }
    }
}
