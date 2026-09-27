using UnityEngine;

public class MostrarBotonAlColocar : MonoBehaviour
{
    public PlateBehaviour mesa;
    public GameObject boton;
    public PlateBehaviour[] puntos;
    public GameObject[] ocultar;

    static readonly Vector3[] puestosViejos =
    {
        new Vector3(76.19f, -10.819f, -91.52f),
        new Vector3(82.2f, -10.819f, -88.2f),
        new Vector3(70.2f, -10.819f, -88.2f)
    };

    static readonly Vector3[] lugaresEnElla =
    {
        new Vector3(-0.45f, 0.15f, 0.28f),
        new Vector3(0f, 0.35f, 0.28f),
        new Vector3(0.45f, 0.15f, 0.28f)
    };

    Transform preucupada;

    void Start()
    {
        if (puntos == null || puntos.Length == 0)
            return;

        QuitarDuplicados();
        GameObject ella = GameObject.Find("Preucupada");
        if (ella == null)
            return;

        preucupada = ella.transform;
        if (boton != null)
        {
            TMPro.TextMeshProUGUI texto = boton.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            if (texto != null)
                texto.text = "FELICITACIONES";
        }
        for (int i = 0; i < puntos.Length; i++)
        {
            PlateBehaviour punto = puntos[i];
            if (punto == null)
                continue;

            Transform lugar = punto.transform;
            lugar.SetParent(preucupada, false);
            lugar.localRotation = Quaternion.identity;
            lugar.localScale = Vector3.one;
            lugar.localPosition = new Vector3(0f, 0f, 0.35f);

            if (lugar.childCount > 0)
                lugar.GetChild(0).localPosition = i < lugaresEnElla.Length ? lugaresEnElla[i] : Vector3.zero;

            BoxCollider caja = punto.GetComponent<BoxCollider>();
            if (caja != null)
            {
                caja.enabled = true;
                caja.isTrigger = false;
                caja.size = new Vector3(6f, 16f, 2.5f);
                caja.center = Vector3.zero;
            }
        }

        PlateBehaviour[] placas = FindObjectsByType<PlateBehaviour>(FindObjectsSortMode.None);
        foreach (PlateBehaviour placa in placas)
        {
            if (placa == null || EsPunto(placa))
                continue;
            BoxCollider caja = placa.GetComponent<BoxCollider>();
            if (caja != null)
                caja.enabled = false;
        }
    }

    void Update()
    {
        if (puntos != null && puntos.Length > 0)
        {
            int listos = 0;
            foreach (PlateBehaviour punto in puntos)
            {
                if (punto == null)
                    continue;

                bool ocupado = punto.heldObject != null;
                BoxCollider caja = punto.GetComponent<BoxCollider>();
                if (caja != null)
                    caja.enabled = !ocupado;
                if (!ocupado)
                    continue;

                listos++;
                GameObject objeto = punto.heldObject;
                if (preucupada != null && objeto.transform.parent != preucupada)
                    objeto.transform.SetParent(preucupada, true);

                Collider[] golpes = objeto.GetComponentsInChildren<Collider>(true);
                foreach (Collider golpe in golpes)
                    golpe.enabled = false;
            }

            bool completa = listos >= puntos.Length;
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

    bool EsPunto(PlateBehaviour placa)
    {
        foreach (PlateBehaviour punto in puntos)
        {
            if (punto == placa)
                return true;
        }
        return false;
    }

    static void QuitarDuplicados()
    {
        GrabObject[] objetos = FindObjectsByType<GrabObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        string[] familias = { "Rubor", "Sombras", "Labial" };
        foreach (string familia in familias)
        {
            int cantidad = 0;
            GrabObject elegido = null;
            foreach (GrabObject objeto in objetos)
            {
                if (!EsFamilia(objeto.gameObject.name, familia))
                    continue;
                cantidad++;
                if (elegido == null || (EsPuestoViejo(elegido.transform.position) && !EsPuestoViejo(objeto.transform.position)))
                    elegido = objeto;
            }

            if (cantidad < 2 || elegido == null)
                continue;

            foreach (GrabObject objeto in objetos)
            {
                if (objeto != elegido && EsFamilia(objeto.gameObject.name, familia))
                    objeto.gameObject.SetActive(false);
            }
        }
    }

    static bool EsFamilia(string nombre, string familia)
    {
        return nombre == familia || nombre.StartsWith(familia + " ") || nombre.StartsWith(familia + "(");
    }

    static bool EsPuestoViejo(Vector3 posicion)
    {
        foreach (Vector3 puesto in puestosViejos)
        {
            if (Vector3.Distance(posicion, puesto) < 1.5f)
                return true;
        }
        return false;
    }
}
