using UnityEngine;
using UnityEngine.UI;

public class MostrarBotonAlColocar : MonoBehaviour
{
    public PlateBehaviour mesa;
    public GameObject boton;
    public PlateBehaviour[] puntos;
    public GameObject[] ocultar;

    void Update()
    {
        if (puntos != null && puntos.Length > 0)
        {
            bool completa = true;
            foreach (PlateBehaviour punto in puntos)
            {
                if (punto == null || punto.heldObject == null)
                {
                    completa = false;
                    break;
                }
            }

            if (ocultar != null)
            {
                foreach (GameObject objeto in ocultar)
                {
                    if (objeto != null && objeto.activeSelf == completa)
                        objeto.SetActive(!completa);
                }
            }

            if (boton != null && boton.activeSelf != completa)
                boton.SetActive(completa);
            return;
        }

        if (mesa == null || boton == null)
            return;

        bool visible = mesa.heldObject != null;
        if (boton.activeSelf != visible)
            boton.SetActive(visible);
    }
}

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
static class ArmarBotonFinal
{
    static ArmarBotonFinal()
    {
        UnityEditor.EditorApplication.delayCall += Armar;
    }

    static void Armar()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        UnityEngine.SceneManagement.Scene escena = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (escena.name != "Juego" || YaExiste("BotonFinal"))
            return;

        GameObject preucupada = GameObject.Find("Preucupada");
        GameObject celular = GameObject.Find("Celular Morado");
        PlateBehaviour rubor = Buscar("Punto Rubor");
        PlateBehaviour labial = Buscar("Punto Labial");
        PlateBehaviour sombras = Buscar("Punto Sombras");
        if (preucupada == null || celular == null || rubor == null || labial == null || sombras == null)
            return;

        Transform lugar = preucupada.transform;
        GameObject raiz = new GameObject("BotonFinal", typeof(RectTransform));
        UnityEditor.Undo.RegisterCreatedObjectUndo(raiz, "Boton final");
        raiz.transform.SetPositionAndRotation(lugar.position + lugar.forward * 0.4f, lugar.rotation * Quaternion.Euler(0f, 180f, 0f));
        raiz.transform.localScale = Vector3.one * 0.015f;
        raiz.SetActive(false);

        RectTransform rect = raiz.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(520f, 180f);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Canvas canvas = raiz.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 30;

        GameObject boton = new GameObject("Boton", typeof(RectTransform));
        boton.tag = "Interactable";
        boton.transform.SetParent(raiz.transform, false);
        RectTransform botonRect = boton.GetComponent<RectTransform>();
        botonRect.anchorMin = Vector2.zero;
        botonRect.anchorMax = Vector2.one;
        botonRect.offsetMin = Vector2.zero;
        botonRect.offsetMax = Vector2.zero;
        botonRect.localScale = Vector3.one;
        botonRect.localRotation = Quaternion.identity;

        boton.AddComponent<CanvasRenderer>();
        Image imagen = boton.AddComponent<Image>();
        imagen.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Juego/Images/Bottones.png");
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;

        Button pulsador = boton.AddComponent<Button>();
        pulsador.targetGraphic = imagen;
        CambioEscena cambio = boton.AddComponent<CambioEscena>();
        UnityEditor.Events.UnityEventTools.AddIntPersistentListener(pulsador.onClick, cambio.CambioEscene, 3);
        boton.AddComponent<UiElementXR>();

        BoxCollider caja = boton.AddComponent<BoxCollider>();
        caja.size = new Vector3(520f, 180f, 30f);
        caja.center = Vector3.zero;

        GameObject textoObjeto = new GameObject("Texto", typeof(RectTransform));
        textoObjeto.transform.SetParent(boton.transform, false);
        RectTransform textoRect = textoObjeto.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = Vector2.zero;
        textoRect.offsetMax = Vector2.zero;
        textoRect.localScale = Vector3.one;
        textoRect.localRotation = Quaternion.identity;
        textoObjeto.AddComponent<CanvasRenderer>();
        TMPro.TextMeshProUGUI texto = textoObjeto.AddComponent<TMPro.TextMeshProUGUI>();
        texto.text = "FINAL";
        texto.alignment = TMPro.TextAlignmentOptions.Center;
        texto.color = new Color(0.25f, 0.05f, 0.18f, 1f);
        texto.enableAutoSizing = true;
        texto.fontSizeMin = 28f;
        texto.fontSizeMax = 72f;
        texto.raycastTarget = false;
        texto.font = UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Fonts/Starborn SDF.asset");

        GameObject entrega = new GameObject("EntregaMaquillaje");
        UnityEditor.Undo.RegisterCreatedObjectUndo(entrega, "Boton final");
        MostrarBotonAlColocar script = entrega.AddComponent<MostrarBotonAlColocar>();
        UnityEditor.SerializedObject serie = new UnityEditor.SerializedObject(script);
        serie.FindProperty("boton").objectReferenceValue = raiz;
        UnityEditor.SerializedProperty listaPuntos = serie.FindProperty("puntos");
        listaPuntos.arraySize = 3;
        listaPuntos.GetArrayElementAtIndex(0).objectReferenceValue = rubor;
        listaPuntos.GetArrayElementAtIndex(1).objectReferenceValue = labial;
        listaPuntos.GetArrayElementAtIndex(2).objectReferenceValue = sombras;
        UnityEditor.SerializedProperty listaOcultar = serie.FindProperty("ocultar");
        listaOcultar.arraySize = 2;
        listaOcultar.GetArrayElementAtIndex(0).objectReferenceValue = preucupada;
        listaOcultar.GetArrayElementAtIndex(1).objectReferenceValue = celular;
        serie.ApplyModifiedPropertiesWithoutUndo();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(escena);
    }

    static bool YaExiste(string nombre)
    {
        Transform[] objetos = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform objeto in objetos)
        {
            if (objeto.name == nombre && objeto.gameObject.scene.IsValid())
                return true;
        }
        return false;
    }

    static PlateBehaviour Buscar(string nombre)
    {
        GameObject objeto = GameObject.Find(nombre);
        if (objeto == null)
            return null;
        return objeto.GetComponent<PlateBehaviour>();
    }
}
#endif
