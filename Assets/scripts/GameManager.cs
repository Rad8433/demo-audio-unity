using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GestionnaireJeu : MonoBehaviour
{
    public static GestionnaireJeu Instance;

    [Header("Gameplay")]
    public int viesMax = 3;
    private int viesActuelles;

    [Header("Interface Utilisateur (UI)")]
    public TMP_Text texteVies;          // Affiche le nombre de vies
    public GameObject panneauGameOver;  // Panneau affiché à la fin du jeu

    [Header("Audio - Effets")]
    public AudioClip sonDegat;          // Son joué à chaque perte d'une vie
    public float volumeSonDegat = 1f;   // Volume pour le son de dégât (0..1)
    public AudioClip sonMort;           // Son lorsqu’on perd la dernière vie
    public AudioClip sonGameOver;       // Son lorsque le panneau apparaît

    [Header("Audio - Musique de fond")]
    public AudioSource sourceMusiqueTheme; // Musique de thème (séparée du joueur)

    // Source d'effets audio (utilisée pour PlayOneShot)
    private AudioSource sourceAudio;
    private bool jeuTermine = false;

    private void Awake()
    {
        // Création du singleton (accès global)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Récupérer l'AudioSource sur ce GameObject (ou laisser l'éditeur l'assigner)
        sourceAudio = GetComponent<AudioSource>();
        if (sourceAudio == null)
        {
            // fallback : on ajoute une AudioSource si nécessaire (facilite le test en éditeur)
            sourceAudio = gameObject.AddComponent<AudioSource>();
            sourceAudio.playOnAwake = false;
        }
    }

    private void Start()
    {
        viesActuelles = viesMax;
        MettreAJourVies();

        if (panneauGameOver != null)
            panneauGameOver.SetActive(false);

        // S’assurer que le temps est normal au début
        Time.timeScale = 1f;
    }

    // Appelée quand le joueur subit un dégât (par ex. collision)
    public void SubirDegat()
    {
        if (jeuTermine) return;

        // Jouer le son de dégât immédiatement (même si on va aussi déclencher GameOver ensuite)
        if (sourceAudio != null && sonDegat != null)
        {
            sourceAudio.PlayOneShot(sonDegat, Mathf.Clamp01(volumeSonDegat));
        }

        viesActuelles--;
        MettreAJourVies();

        if (viesActuelles <= 0)
        {
            StartCoroutine(SequenceGameOver());
        }
    }

    void MettreAJourVies()
    {
        if (texteVies != null)
            texteVies.text = "Vies : " + viesActuelles;
    }

    private System.Collections.IEnumerator SequenceGameOver()
    {
        jeuTermine = true;

        // Désactiver le mouvement du joueur immédiatement
        PlayerController joueur = FindObjectOfType<PlayerController>();
        if (joueur != null)
            joueur.mouvementAutorise = false;

        // Arrêter la musique de thème si elle existe
        if (sourceMusiqueTheme != null)
            sourceMusiqueTheme.Stop();

        // Jouer le son de mort immédiatement (dernière vie)
        if (sourceAudio != null && sonMort != null)
            sourceAudio.PlayOneShot(sonMort);

        // Attendre 1,5 seconde avant d’afficher le panneau
        yield return new WaitForSeconds(1.5f);

        // Afficher le panneau Game Over
        if (panneauGameOver != null)
            panneauGameOver.SetActive(true);

        // Jouer le son du Game Over (panel)
        if (sourceAudio != null && sonGameOver != null)
            sourceAudio.PlayOneShot(sonGameOver);

        // Geler le jeu après l'affichage du panneau
        Time.timeScale = 0f;
    }

    private void Update()
    {
        // Réinitialiser la scène en appuyant sur R
        if (jeuTermine && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Méthode publique utilitaire pour jouer un clip si besoin ailleurs
    public void JouerSon(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sourceAudio != null)
            sourceAudio.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
