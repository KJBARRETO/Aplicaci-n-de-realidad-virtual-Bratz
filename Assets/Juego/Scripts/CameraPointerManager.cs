using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointerManager : MonoBehaviour
{
     public static CameraPointerManager Instance;

    [SerializeField] private GameObject pointer;
    [SerializeField] private float maxDistancePointer = 4.5f;
    [Range (0,1)] 
    [SerializeField] private float disPointerObject = 0.95f;


    private const float _maxDistance = 1000;
    private static readonly RaycastHit[] _hits = new RaycastHit[64];
    private GameObject _gazedAtObject = null;

    private readonly string interactableTag = "Interactable";
    private float scaleSize = 0.025f;
    [HideInInspector] 
    public Vector3 hitPoint;

    private void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

        }
    }

    private void Start ()
    {
        GazeManager.Instance.OnGazeSelection += GazeSelection;
    }

    private void GazeSelection()
    {
        _gazedAtObject?.SendMessage("OnPointerClickXR", null, SendMessageOptions.DontRequireReceiver);
    }

   public void Update()
   {
        GameObject objetivo = null;
        Vector3 punto = transform.position + transform.forward;

        if (BuscarBotonEnPantalla(out GameObject boton, out Vector3 puntoBoton))
        {
            objetivo = boton;
            punto = puntoBoton;
        }
        else if (BuscarObjetivo(out RaycastHit hit))
        {
            objetivo = hit.transform.gameObject;
            punto = hit.point;
        }

        if (objetivo != null && objetivo.CompareTag(interactableTag))
        {
            hitPoint = punto;
            if (_gazedAtObject != objetivo)
            {
                _gazedAtObject?.SendMessage("OnPointerExitXR", null, SendMessageOptions.DontRequireReceiver);
                _gazedAtObject = objetivo;
                if (GazeManager.Instance != null)
                    GazeManager.Instance.StartGazeSelection();
                PonerCirculoDelante();
                _gazedAtObject.SendMessage("OnPointerEnterXR", null, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                PonerCirculoDelante();
            }
            PointerOnGaze(punto);
        }
        else
        {
            _gazedAtObject?.SendMessage("OnPointerExitXR", null, SendMessageOptions.DontRequireReceiver);
            _gazedAtObject = null;
            PointerOutGaze();
        }

        if (Google.XR.Cardboard.Api.IsTriggerPressed)
        {
            _gazedAtObject?.SendMessage("OnPointerClickXR", null, SendMessageOptions.DontRequireReceiver);
        }

          }

        private void PonerCirculoDelante()
        {
            if (pointer == null)
                return;
            Canvas lienzo = pointer.GetComponentInChildren<Canvas>(true);
            if (lienzo == null)
                return;
            lienzo.overrideSorting = true;
            lienzo.sortingOrder = 500;
        }

        private bool BuscarBotonEnPantalla(out GameObject boton, out Vector3 punto)
        {
            boton = null;
            punto = Vector3.zero;
            Camera camara = GetComponent<Camera>();
            if (camara == null)
                return false;

            Vector2 centro = camara.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));
            float mejor = float.MaxValue;
            UiElementXR[] botones = FindObjectsByType<UiElementXR>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            for (int i = 0; i < botones.Length; i++)
            {
                UiElementXR elemento = botones[i];
                if (elemento == null || !elemento.isActiveAndEnabled)
                    continue;
                if (!elemento.CompareTag(interactableTag))
                    continue;

                RectTransform rect = elemento.transform as RectTransform;
                if (rect == null)
                    continue;

                Vector3[] esquinas = new Vector3[4];
                rect.GetWorldCorners(esquinas);
                if (!CentroDentro(camara, centro, esquinas, out float profundidad))
                    continue;
                if (profundidad >= mejor)
                    continue;

                mejor = profundidad;
                boton = elemento.gameObject;
                punto = rect.position;
            }

            return boton != null;
        }

        private static bool CentroDentro(Camera camara, Vector2 centro, Vector3[] esquinas, out float profundidad)
        {
            profundidad = 0f;
            Vector2[] pantalla = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                Vector3 proyectado = camara.WorldToScreenPoint(esquinas[i]);
                if (proyectado.z <= 0.01f)
                    return false;
                pantalla[i] = proyectado;
                profundidad += proyectado.z;
            }

            profundidad *= 0.25f;
            bool dentro = false;
            for (int i = 0, j = 3; i < 4; j = i++)
            {
                bool cruza = (pantalla[i].y > centro.y) != (pantalla[j].y > centro.y);
                if (!cruza)
                    continue;
                float x = (pantalla[j].x - pantalla[i].x) * (centro.y - pantalla[i].y) / (pantalla[j].y - pantalla[i].y) + pantalla[i].x;
                if (centro.x < x)
                    dentro = !dentro;
            }

            return dentro;
        }

        private bool BuscarObjetivo(out RaycastHit elegido)
        {
            int cantidad = Physics.RaycastNonAlloc(transform.position, transform.forward, _hits, _maxDistance);
            bool hayInteractable = false;
            float distancia = _maxDistance;
            elegido = default;

            for (int i = 0; i < cantidad; i++)
            {
                if (!_hits[i].transform.CompareTag(interactableTag))
                    continue;
                if (_hits[i].distance >= distancia)
                    continue;
                distancia = _hits[i].distance;
                elegido = _hits[i];
                hayInteractable = true;
            }

            if (hayInteractable)
                return true;

            bool hayAlgo = false;
            distancia = _maxDistance;
            for (int i = 0; i < cantidad; i++)
            {
                if (_hits[i].distance >= distancia)
                    continue;
                distancia = _hits[i].distance;
                elegido = _hits[i];
                hayAlgo = true;
            }

            return hayAlgo;
        }

        private void PointerOnGaze(Vector3 hitPoint)
        {
            float scaleFactor = scaleSize * Vector3.Distance(transform.position, hitPoint);
            pointer.transform.localScale = Vector3.one * scaleFactor;
            pointer.transform.parent.position = CalculatePointerPosition(transform.position, hitPoint, disPointerObject);
        }

        private void PointerOutGaze()
        {
            pointer.transform.localScale = Vector3.one * 0.035f;
            pointer.transform.parent.transform.localPosition = new Vector3(0, 0, maxDistancePointer);
            pointer.transform.parent.parent.transform.rotation = transform.rotation;
            GazeManager.Instance.CancelGazeSelection();
        }

        private Vector3 CalculatePointerPosition(Vector3 p0, Vector3 p1, float t)
        {
            float x = p0.x + t * (p1.x - p0.x);
            float y = p0.y + t * (p1.y - p0.y);
            float z = p0.z + t * (p1.z - p0.z);

            return new Vector3(x,y,z);
        }
}
