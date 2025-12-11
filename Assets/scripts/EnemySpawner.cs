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
    public float volumeSonApparition = 1f;

    private float minuterie = 0f;
    private AudioSource audioSource;
    private bool jeuTermine = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        audioSource.ignoreListenerPause = true;
    }

    private void OnEnable()
    {
        // Subscribe to the game over event
        if (GestionnaireJeu.Instance != null)
            GestionnaireJeu.Instance.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (GestionnaireJeu.Instance != null)
            GestionnaireJeu.Instance.OnGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        jeuTermine = true;

        // Stop any currently playing spawn sound
        if (audioSource.isPlaying)
            audioSource.Stop();

        // Optionally, disable the spawner
        enabled = false;
    }

    private void Update()
    {
        if (jeuTermine) return;

        // Normal spawning logic
        minuterie += Time.deltaTime;

        if (minuterie >= intervalleApparition)
        {
            minuterie = 0f;
            ApparaitreEnnemi();
        }
    }

    private void ApparaitreEnnemi()
    {
        float randomX = Random.Range(-plageSpawnX, plageSpawnX);
        Vector3 posSpawn = new Vector3(randomX, transform.position.y, 0f);

        GameObject ennemi = Instantiate(prefabEnnemi, posSpawn, Quaternion.identity);

        DeplacementEnnemi deplacement = ennemi.AddComponent<DeplacementEnnemi>();
        deplacement.vitesse = vitesseEnnemi;
        deplacement.positionDestructionY = positionDestructionY;

        // Play spawn sound via AudioSource
        if (sonApparition != null)
            audioSource.PlayOneShot(sonApparition, Mathf.Clamp01(volumeSonApparition));
        else
            Debug.LogWarning("[GenerateurEnnemis] sonApparition non assigné !");
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
            Destroy(gameObject);
    }
}
