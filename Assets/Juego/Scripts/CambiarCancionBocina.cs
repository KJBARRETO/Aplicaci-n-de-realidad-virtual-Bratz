using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarCancionBocina : MonoBehaviour
{
    public AudioClip[] canciones;
    AudioSource fuente;
    int indice;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Preparar()
    {
        SceneManager.sceneLoaded -= AlCargar;
        SceneManager.sceneLoaded += AlCargar;
    }

    static void AlCargar(Scene escena, LoadSceneMode modo)
    {
        GameObject bocina = GameObject.Find("Disco_Speaker (3)");
        if (bocina == null)
            return;

        AudioSource[] fuentes = Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < fuentes.Length; i++)
        {
            if (fuentes[i].gameObject == bocina)
                continue;
            if (!fuentes[i].gameObject.name.StartsWith("Disco_Speaker"))
                continue;
            fuentes[i].Stop();
            fuentes[i].playOnAwake = false;
            fuentes[i].clip = null;
        }

        if (bocina.GetComponent<CambiarCancionBocina>() == null)
            bocina.AddComponent<CambiarCancionBocina>();
    }

    void Start()
    {
        if (canciones == null || canciones.Length == 0)
        {
            GameObject plantilla = Resources.Load<GameObject>("BocinaCanciones");
            if (plantilla != null)
            {
                CambiarCancionBocina datos = plantilla.GetComponent<CambiarCancionBocina>();
                if (datos != null)
                    canciones = datos.canciones;
            }
        }

        fuente = GetComponent<AudioSource>();
        if (fuente == null)
            fuente = gameObject.AddComponent<AudioSource>();

        gameObject.tag = "Interactable";
        AsegurarCaja();

        fuente.loop = true;
        fuente.playOnAwake = true;
        fuente.spatialBlend = 1f;
        fuente.minDistance = 2f;
        fuente.maxDistance = 80f;
        fuente.volume = 0.7f;

        if (canciones != null && canciones.Length > 0 && canciones[0] != null)
        {
            indice = 0;
            fuente.clip = canciones[0];
        }

        if (fuente.clip != null)
            fuente.Play();
    }

    public void OnPointerClickXR()
    {
        if (fuente == null || canciones == null || canciones.Length < 2)
            return;

        indice = (indice + 1) % canciones.Length;
        if (canciones[indice] == null)
            return;

        fuente.clip = canciones[indice];
        fuente.Play();
    }

    void AsegurarCaja()
    {
        BoxCollider caja = GetComponent<BoxCollider>();
        if (caja == null)
            caja = gameObject.AddComponent<BoxCollider>();

        MeshFilter filtro = GetComponent<MeshFilter>();
        if (filtro == null || filtro.sharedMesh == null)
            return;

        caja.center = filtro.sharedMesh.bounds.center;
        caja.size = filtro.sharedMesh.bounds.size;
    }
}
