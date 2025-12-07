using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Déplacement")]
    public float vitesseDeplacement = 5f;

    [Header("Audio")]
    public AudioClip sonDeplacement;
    public AudioClip sonCoup;

    [HideInInspector]
    public bool mouvementAutorise = true;   // <- AJOUT ICI

    private Rigidbody2D corpsRigide2D;
    private AudioSource sourceAudio;
    private Vector2 directionMouvement;

    private void Awake()
    {
        corpsRigide2D = GetComponent<Rigidbody2D>();
        sourceAudio = GetComponent<AudioSource>();

        sourceAudio.loop = true;
        sourceAudio.playOnAwake = false;
    }

    private void Update()
    {
        if (!mouvementAutorise)
        {
            directionMouvement = Vector2.zero;
            GererSonDeplacement();
            return;
        }

        float mouvementX = Input.GetAxisRaw("Horizontal");
        float mouvementY = Input.GetAxisRaw("Vertical");

        directionMouvement = new Vector2(mouvementX, mouvementY).normalized;

        GererSonDeplacement();
    }

    private void FixedUpdate()
    {
        if (mouvementAutorise)
            corpsRigide2D.velocity = directionMouvement * vitesseDeplacement;
        else
            corpsRigide2D.velocity = Vector2.zero;
    }

    void GererSonDeplacement()
    {
        bool enDeplacement = directionMouvement.sqrMagnitude > 0.01f;

        if (enDeplacement)
        {
            if (!sourceAudio.isPlaying && sonDeplacement != null)
            {
                sourceAudio.clip = sonDeplacement;
                sourceAudio.Play();
            }
        }
        else
        {
            if (sourceAudio.isPlaying && sourceAudio.clip == sonDeplacement)
            {
                sourceAudio.Stop();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (sonCoup != null)
                sourceAudio.PlayOneShot(sonCoup);

            if (GestionnaireJeu.Instance != null)
                GestionnaireJeu.Instance.SubirDegat();

            Destroy(collision.gameObject);
        }
    }
}
