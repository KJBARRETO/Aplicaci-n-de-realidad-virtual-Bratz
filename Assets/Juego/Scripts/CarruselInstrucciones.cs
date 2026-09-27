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
        objeto.layer = 0;

        Vector3 escala = objeto.transform.localScale;
        if (escala.x < 0f)
        {
            objeto.transform.localScale = new Vector3(Mathf.Abs(escala.x), Mathf.Abs(escala.y), Mathf.Abs(escala.z));
            objeto.transform.Rotate(0f, 180f, 0f);
        }

        RectTransform rect = objeto.GetComponent<RectTransform>();
        BoxCollider caja = objeto.GetComponent<BoxCollider>();
        if (caja == null)
            caja = objeto.AddComponent<BoxCollider>();
        Vector2 tamano = rect.rect.size;
        if (tamano.x < 1f || tamano.y < 1f)
            tamano = rect.sizeDelta;
        caja.size = new Vector3(Mathf.Abs(tamano.x), Mathf.Abs(tamano.y), 40f);
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

