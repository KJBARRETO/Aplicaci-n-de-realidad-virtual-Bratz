using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarruselInstrucciones : MonoBehaviour
{
    public GameObject texto1;
    public GameObject texto2;
    public GameObject texto3;
    public GameObject texto4;

    int indice;
    bool listo;
    GameObject[] paginas;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Crear()
    {
        ConfigurarSiExiste("TextoTutorial");
        ConfigurarSiExiste("marco");
    }

    public void Configurar(Transform raiz)
    {
        if (listo)
            return;

        texto1 = Buscar(raiz, "Texto (1)");
        texto2 = Buscar(raiz, "Texto (2)");
        texto3 = Buscar(raiz, "Texto (3)");
        texto4 = BuscarInstrucciones(raiz);
        if (texto4 == null)
            texto4 = Buscar(raiz, "Texto");

        if (texto1 == texto4) texto1 = null;
        if (texto2 == texto4) texto2 = null;
        if (texto3 == texto4) texto3 = null;

        var lista = new System.Collections.Generic.List<GameObject>();
        if (texto4 != null) lista.Add(texto4);
        if (texto1 != null) lista.Add(texto1);
        if (texto2 != null) lista.Add(texto2);
        if (texto3 != null) lista.Add(texto3);
        AgregarSiExiste(lista, raiz, "Pista 1");
        AgregarSiExiste(lista, raiz, "Pista 2");
        AgregarSiExiste(lista, raiz, "Pista 3");
        paginas = lista.ToArray();

        Button[] flechas = raiz.GetComponentsInChildren<Button>(true);
        Button izquierda = null;
        Button derecha = null;
        float xIzquierda = float.MaxValue;
        float xDerecha = float.MinValue;
        foreach (Button flecha in flechas)
        {
            if (flecha.GetComponent<CambioEscena>() != null)
                continue;
            float x = flecha.GetComponent<RectTransform>().anchoredPosition.x;
            if (x < xIzquierda)
            {
                xIzquierda = x;
                izquierda = flecha;
            }
            if (x > xDerecha)
            {
                xDerecha = x;
                derecha = flecha;
            }
        }

        PrepararFlecha(izquierda, Anterior);
        if (derecha != izquierda)
            PrepararFlecha(derecha, Siguiente);

        indice = 0;
        Mostrar();
        listo = true;
    }

    static GameObject BuscarInstrucciones(Transform raiz)
    {
        TextMeshProUGUI[] textos = raiz.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI texto in textos)
        {
            if (texto.GetComponentInParent<Button>() != null)
                continue;
            string limpio = texto.text.Replace("\r", "").Trim();
            if (string.Equals(limpio, "INSTRUCCIONES", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(limpio, "PISTAS", System.StringComparison.OrdinalIgnoreCase))
                return texto.gameObject;
        }
        return null;
    }
    static void ConfigurarSiExiste(string nombre)
    {
        GameObject raiz = GameObject.Find(nombre);
        if (raiz == null)
            return;

        CarruselInstrucciones carrusel = raiz.GetComponent<CarruselInstrucciones>();
        if (carrusel == null)
            carrusel = raiz.GetComponentInChildren<CarruselInstrucciones>(true);
        if (carrusel == null)
            carrusel = raiz.AddComponent<CarruselInstrucciones>();
        carrusel.Configurar(raiz.transform);
    }

    static void AgregarSiExiste(System.Collections.Generic.List<GameObject> lista, Transform raiz, string nombre)
    {
        GameObject pagina = Buscar(raiz, nombre);
        if (pagina != null)
            lista.Add(pagina);
    }

    static GameObject Buscar(Transform raiz, string nombre)
    {
        foreach (Transform hijo in raiz.GetComponentsInChildren<Transform>(true))
        {
            if (hijo.name == nombre && hijo.GetComponentInParent<Button>() == null)
                return hijo.gameObject;
        }
        return null;
    }

    static void PrepararFlecha(Button flecha, UnityEngine.Events.UnityAction accion)
    {
        if (flecha == null)
            return;

        GameObject objeto = flecha.gameObject;
        objeto.tag = "Interactable";

        RectTransform rect = objeto.GetComponent<RectTransform>();
        BoxCollider caja = objeto.GetComponent<BoxCollider>();
        if (caja == null)
            caja = objeto.AddComponent<BoxCollider>();
        Vector2 tamano = rect.rect.size;
        if (tamano.x < 1f || tamano.y < 1f)
            tamano = rect.sizeDelta;
        caja.size = new Vector3(Mathf.Abs(tamano.x), Mathf.Abs(tamano.y), 20f);
        caja.center = Vector3.zero;

        if (objeto.GetComponent<UiElementXR>() == null)
            objeto.AddComponent<UiElementXR>();

        flecha.onClick.AddListener(accion);
    }

    public void Siguiente()
    {
        if (paginas == null || paginas.Length == 0)
            return;
        indice = (indice + 1) % paginas.Length;
        Mostrar();
    }

    public void Anterior()
    {
        if (paginas == null || paginas.Length == 0)
            return;
        indice = (indice + paginas.Length - 1) % paginas.Length;
        Mostrar();
    }

    void Mostrar()
    {
        if (paginas == null || paginas.Length == 0)
            return;

        GameObject activo = paginas[indice];
        for (int i = 0; i < paginas.Length; i++)
        {
            if (paginas[i] != null && paginas[i] != activo)
                paginas[i].SetActive(false);
        }
        if (activo != null)
            activo.SetActive(true);

        TextMeshProUGUI[] todos = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI texto in todos)
        {
            if (texto.GetComponentInParent<Button>() != null)
                continue;
            if (activo != null && (texto.gameObject == activo || texto.transform.IsChildOf(activo.transform) || activo.transform.IsChildOf(texto.transform)))
                continue;
            bool esPagina = false;
            foreach (GameObject pagina in paginas)
            {
                if (pagina != null && (texto.gameObject == pagina || texto.transform.IsChildOf(pagina.transform)))
                {
                    esPagina = true;
                    break;
                }
            }
            if (!esPagina && texto.gameObject.activeSelf)
                texto.gameObject.SetActive(false);
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
static class ArmarPistasEnMarco
{
    static ArmarPistasEnMarco()
    {
        UnityEditor.EditorApplication.delayCall += Armar;
    }

    static void Armar()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        GameObject marco = GameObject.Find("marco");
        if (marco == null || marco.transform.Find("LienzoPistas") != null)
            return;

        SpriteRenderer sprite = marco.GetComponent<SpriteRenderer>();
        if (sprite == null || sprite.sprite == null)
            return;

        UnityEditor.Undo.RegisterFullObjectHierarchyUndo(marco, "Pistas en el marco");

        Vector2 tamano = sprite.sprite.bounds.size;
        GameObject lienzo = new GameObject("LienzoPistas", typeof(RectTransform));
        lienzo.layer = marco.layer;
        lienzo.transform.SetParent(marco.transform, false);

        RectTransform rect = lienzo.GetComponent<RectTransform>();
        rect.localRotation = Quaternion.Euler(0f, 180f, 0f);
        rect.localPosition = new Vector3(0f, 0f, 0.15f);
        rect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = tamano / 0.01f;

        Canvas canvas = lienzo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 20;
        UnityEngine.UI.CanvasScaler escala = lienzo.AddComponent<UnityEngine.UI.CanvasScaler>();
        escala.dynamicPixelsPerUnit = 10f;
        lienzo.AddComponent<CarruselInstrucciones>();

        float ancho = rect.sizeDelta.x;
        float alto = rect.sizeDelta.y;
        CrearTexto(lienzo.transform);
        CrearImagen(lienzo.transform, "Pista 1", "Assets/Juego/Images/Pista 1.png", false);
        CrearImagen(lienzo.transform, "Pista 2", "Assets/Juego/Images/Pista 2.png", false);
        CrearImagen(lienzo.transform, "Pista 3", "Assets/Juego/Images/pista 3.png", false);
        CrearFlecha(lienzo.transform, "Flecha Izquierda", new Vector2(-ancho * 0.36f, 0f), new Vector2(ancho * 0.12f, alto * 0.16f), true);
        CrearFlecha(lienzo.transform, "Flecha Derecha", new Vector2(ancho * 0.36f, 0f), new Vector2(ancho * 0.12f, alto * 0.16f), false);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(marco.scene);
    }

    static void CrearTexto(Transform padre)
    {
        GameObject objeto = new GameObject("PISTAS", typeof(RectTransform));
        objeto.layer = padre.gameObject.layer;
        objeto.transform.SetParent(padre, false);
        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.22f, 0.7f);
        rect.anchorMax = new Vector2(0.78f, 0.88f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        objeto.AddComponent<CanvasRenderer>();
        TextMeshProUGUI texto = objeto.AddComponent<TextMeshProUGUI>();
        texto.text = "PISTAS";
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = new Color(0.35f, 0.08f, 0.28f, 1f);
        texto.enableAutoSizing = true;
        texto.fontSizeMin = 28f;
        texto.fontSizeMax = 96f;
        texto.raycastTarget = false;
        texto.font = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/Marimpa SDF.asset");
    }

    static void CrearImagen(Transform padre, string nombre, string ruta, bool visible)
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform));
        objeto.layer = padre.gameObject.layer;
        objeto.transform.SetParent(padre, false);
        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0.16f);
        rect.anchorMax = new Vector2(0.8f, 0.68f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        objeto.AddComponent<CanvasRenderer>();
        UnityEngine.UI.Image imagen = objeto.AddComponent<UnityEngine.UI.Image>();
        imagen.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;
        objeto.SetActive(visible);
    }

    static void CrearFlecha(Transform padre, string nombre, Vector2 posicion, Vector2 tamano, bool voltear)
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform));
        objeto.layer = padre.gameObject.layer;
        objeto.transform.SetParent(padre, false);
        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = tamano;
        rect.localRotation = Quaternion.identity;
        rect.localScale = voltear ? new Vector3(-1f, 1f, 1f) : Vector3.one;

        objeto.AddComponent<CanvasRenderer>();
        UnityEngine.UI.Image imagen = objeto.AddComponent<UnityEngine.UI.Image>();
        imagen.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Juego/Images/FLECHA.png");
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;
        UnityEngine.UI.Button boton = objeto.AddComponent<UnityEngine.UI.Button>();
        boton.targetGraphic = imagen;
    }
}
#endif
