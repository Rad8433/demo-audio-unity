using UnityEngine;

public class GenerateurEnnemis : MonoBehaviour
{
    [Header("Paramètres d'apparition")]
    public GameObject prefabEnnemi;     // Prefab de l’ennemi à instancier
    public float intervalleApparition = 1.5f;
    public float plageSpawnX = 8f;      // Largeur de la zone où les ennemis peuvent apparaître

    [Header("Paramètres des ennemis")]
    public float vitesseEnnemi = 3f;
    public float positionDestructionY = -6f;

    private float minuterie = 0f;

    private void Update()
    {
        // Faire avancer la minuterie
        minuterie += Time.deltaTime;

        // Si le temps écoulé dépasse l’intervalle, on fait apparaître un ennemi
        if (minuterie >= intervalleApparition)
        {
            minuterie = 0f;
            ApparaitreEnnemi();
        }
    }

    void ApparaitreEnnemi()
    {
        // Choisir une position aléatoire en X
        float randomX = Random.Range(-plageSpawnX, plageSpawnX);
        Vector3 posSpawn = new Vector3(randomX, transform.position.y, 0f);

        // Instancier un ennemi
        GameObject ennemi = Instantiate(prefabEnnemi, posSpawn, Quaternion.identity);

        // Ajouter un script de déplacement
        DeplacementEnnemi deplacement = ennemi.AddComponent<DeplacementEnnemi>();
        deplacement.vitesse = vitesseEnnemi;
        deplacement.positionDestructionY = positionDestructionY;
    }
}

public class DeplacementEnnemi : MonoBehaviour
{
    public float vitesse = 3f;
    public float positionDestructionY = -6f;

    private void Update()
    {
        // Déplacement vers le bas
        transform.position += Vector3.down * vitesse * Time.deltaTime;

        // Détruire l’objet s’il sort de l’écran
        if (transform.position.y <= positionDestructionY)
        {
            Destroy(gameObject);
        }
    }
}
