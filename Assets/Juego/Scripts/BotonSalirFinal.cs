using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonSalirFinal : MonoBehaviour
{
    public Sprite imagen;
    public TMP_FontAsset fuente;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Preparar()
    {
        SceneManager.sceneLoaded -= AlCargar;
        SceneManager.sceneLoaded += AlCargar;
    }

    static void AlCargar(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != "Final" || GameObject.Find("SALIR") != null)
            return;

        Camera camara = Camera.main;
        Vector3 posicion = Vector3.up * 1.5f;
        Quaternion rotacion = Quaternion.identity;
        if (camara != null)
        {
            Vector3 adelante = camara.transform.forward;
            adelante.y = 0f;
            if (adelante.sqrMagnitude < 0.01f)
                adelante = Vector3.forward;
            adelante.Normalize();
            posicion = camara.transform.position + adelante * 2.6f + camara.transform.right * 1.1f;
            rotacion = Quaternion.LookRotation(posicion - camara.transform.position);
        }

        Crear(posicion, rotacion);
    }

    public static GameObject Crear(Vector3 posicion, Quaternion rotacion)
    {
        if (GameObject.Find("SALIR") != null)
            return GameObject.Find("SALIR");

        GameObject plantilla = Resources.Load<GameObject>("BotonSalirFinal");
        BotonSalirFinal datos = plantilla != null ? plantilla.GetComponent<BotonSalirFinal>() : null;

        GameObject boton = new GameObject("SALIR");
        boton.tag = "Interactable";

        RectTransform recto = boton.AddComponent<RectTransform>();
        recto.sizeDelta = new Vector2(167.4f, 42.8f);
        recto.localScale = Vector3.one * 0.007f;
        recto.position = posicion;
        recto.rotation = rotacion;

        Canvas lienzo = boton.AddComponent<Canvas>();
        lienzo.renderMode = RenderMode.WorldSpace;
        boton.AddComponent<CanvasScaler>();

        Image dibujo = boton.AddComponent<Image>();
        if (datos != null)
            dibujo.sprite = datos.imagen;
        dibujo.preserveAspect = true;
        dibujo.raycastTarget = false;

        boton.AddComponent<Button>();
        BoxCollider caja = boton.AddComponent<BoxCollider>();
        caja.size = new Vector3(167.4f, 42.8f, 20f);
        boton.AddComponent<UiElementXR>();
        boton.AddComponent<SalirAplicacion>();

        GameObject textoGo = new GameObject("Texto");
        textoGo.transform.SetParent(boton.transform, false);
        RectTransform rt = textoGo.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        TextMeshProUGUI texto = textoGo.AddComponent<TextMeshProUGUI>();
        texto.text = "SALIR";
        texto.alignment = TextAlignmentOptions.Center;
        texto.fontSize = 28f;
        texto.color = Color.white;
        texto.raycastTarget = false;
        if (datos != null && datos.fuente != null)
            texto.font = datos.fuente;

        return boton;
    }
}
