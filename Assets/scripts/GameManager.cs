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
    public TMP_Text texteVies;
    public GameObject panneauGameOver;

    [Header("Audio - Effets")]
    public AudioClip sonDegat;
    public float volumeSonDegat = 1f;
    public AudioClip sonMort;

    [Header("Audio - Alerte (1 vie restante)")]
    public AudioSource sourceAlerte; // 🔔 ALERT AUDIO SOURCE (Clip set in Inspector)

    [Header("Audio - Musique Game Over")]
    public AudioSource sourceMusiqueGameOver;

    [Header("Audio - Musique de fond")]
    public AudioSource sourceMusiqueTheme;

    private AudioSource sourceAudio; // SFX source
    private bool jeuTermine = false;

    private bool alerteDejaJouee = false; // Prevent multiple alerts

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        sourceAudio = GetComponent<AudioSource>();
        if (sourceAudio == null)
        {
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

        Time.timeScale = 1f;

        if (sourceMusiqueTheme != null)
            sourceMusiqueTheme.Play();
    }

    public void SubirDegat()
    {
        if (jeuTermine) return;

        // Son de dégât
        if (sourceAudio != null && sonDegat != null)
            sourceAudio.PlayOneShot(sonDegat, Mathf.Clamp01(volumeSonDegat));

        viesActuelles--;
        MettreAJourVies();

        // 🔔 ALERT SOUND when 1 life left
        if (viesActuelles == 1 && !alerteDejaJouee)
        {
            if (sourceAlerte != null)
                sourceAlerte.Play();   // ✔ plays full AudioSource

            alerteDejaJouee = true;
        }

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

        PlayerController joueur = FindObjectOfType<PlayerController>();
        if (joueur != null)
            joueur.mouvementAutorise = false;

        if (sourceMusiqueTheme != null)
            sourceMusiqueTheme.Stop();

        if (sourceAudio != null && sonMort != null)
            sourceAudio.PlayOneShot(sonMort);

        // Stop alert sound when game ends
        if (sourceAlerte != null)
            sourceAlerte.Stop();

        yield return new WaitForSeconds(1.5f);

        if (panneauGameOver != null)
            panneauGameOver.SetActive(true);

        if (sourceMusiqueGameOver != null)
        {
            sourceMusiqueGameOver.loop = false;
            sourceMusiqueGameOver.Play();
        }

        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (jeuTermine && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void JouerSon(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sourceAudio != null)
            sourceAudio.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
