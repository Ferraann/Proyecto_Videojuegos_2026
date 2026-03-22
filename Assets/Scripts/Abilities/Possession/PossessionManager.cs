// TERMINADO V2

using UnityEngine;
using System.Collections.Generic;

namespace Possession
{
    public class PossessionManager : MonoBehaviour
    {
        [SerializeField] private PossessionConfig  config;
        [SerializeField] private Transform         playerTransform;
        [SerializeField] private OutlineController outlineController;
        [SerializeField] private Camara            camara;

        private InputHandler       inputHandler;
        private PossessionState    currentState       = PossessionState.Free;
        public  PossessionState    CurrentState       => currentState;
        private IPossessable       currentTarget;
        private List<IPossessable> nearbyPossessables = new List<IPossessable>();
        private float              scanRefreshTimer;

        // -------------------------------------------------- Unity

        private void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            inputHandler.OnPossessionKeyPressed += HandlePossessionInput;
            inputHandler.OnCancelKeyPressed     += CancelScanning;
        }

        private void OnDestroy()
        {
            inputHandler.OnPossessionKeyPressed -= HandlePossessionInput;
            inputHandler.OnCancelKeyPressed     -= CancelScanning;
        }

        private void Update()
        {
            if (currentState != PossessionState.Scanning) return;

            scanRefreshTimer += Time.deltaTime;
            if (scanRefreshTimer >= 0.1f)
            {
                scanRefreshTimer = 0f;
                List<IPossessable> newNearby = FindAllNearby();

                bool listChanged = newNearby.Count != nearbyPossessables.Count;
                nearbyPossessables = newNearby;

                if (nearbyPossessables.Count == 0)
                {
                    outlineController.HideOutlines();
                    currentTarget = null;
                    return;
                }

                if (listChanged)
                {
                    IPossessable nearest = FindNearestFrom(nearbyPossessables);
                    currentTarget = nearest;
                    outlineController.ShowOutlines(nearbyPossessables, currentTarget);
                    return;
                }
            }

            if (nearbyPossessables.Count == 0) return;

            IPossessable newNearest = FindNearestFrom(nearbyPossessables);

            if (newNearest == null || newNearest == currentTarget) return;

            currentTarget = newNearest;
            outlineController.ShowOutlines(nearbyPossessables, currentTarget);

            Debug.Log($"[Possession] Nuevo objetivo más cercano: {((MonoBehaviour)currentTarget).gameObject.name}");
        }
        // -------------------------------------------------- Input

        private void HandlePossessionInput()
        {
            switch (currentState)
            {
                case PossessionState.Free:
                    EnterScanning();
                    break;

                case PossessionState.Scanning:
                    TryPossess();
                    break;

                case PossessionState.Possessing:
                    Depossess();
                    break;
            }
        }

        // -------------------------------------------------- Estados

        private void CancelScanning()
        {
            if (currentState != PossessionState.Scanning) return;

            outlineController.HideOutlines();
            currentTarget = null;
            currentState  = PossessionState.Free;

            Debug.Log("[Possession] Escaneo cancelado.");
        }

        private void EnterScanning()
        {
            nearbyPossessables = FindAllNearby();
            currentTarget      = FindNearestFrom(nearbyPossessables);
            currentState       = PossessionState.Scanning;

            if (nearbyPossessables.Count > 0)
                outlineController.ShowOutlines(nearbyPossessables, currentTarget);

            Debug.Log($"[Possession] Escaneando.");
        }

        private void TryPossess()
        {
            if (currentTarget == null) return;

            currentState = PossessionState.Possessing;
            outlineController.HideOutlines();
            playerTransform.gameObject.SetActive(false);
            camara.SetTarget(currentTarget.Transform);

            float speed = config.GetSpeedForWeight(currentTarget.WeightClass);
            currentTarget.OnPossess(speed);

            Debug.Log($"[Possession] Poseyendo con velocidad: {speed}");
        }

        private void Depossess()
        {
            if (currentTarget == null) return;

            currentTarget.OnDepossess();
            playerTransform.gameObject.SetActive(true);
            camara.SetTarget(playerTransform);

            Vector3 spawnPosition = SpawnFinder.FindFreePosition(
                currentTarget.Transform.position,
                config.spawnSearchRadius
            );
            playerTransform.position = spawnPosition;

            currentTarget = null;
            currentState  = PossessionState.Free;

            Debug.Log("[Possession] Jugador liberado.");
        }

        // -------------------------------------------------- Detección

        private List<IPossessable> FindAllNearby()
        {
            Collider[] hits = Physics.OverlapSphere(
                playerTransform.position,
                config.detectionRadius
            );

            List<IPossessable> result = new List<IPossessable>();

            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out PossessableObject candidate))
                    result.Add(candidate);
            }

            return result;
        }

        private IPossessable FindNearestFrom(List<IPossessable> possessables)
        {
            IPossessable nearest  = null;
            float        bestDist = float.MaxValue;

            foreach (IPossessable p in possessables)
            {
                float dist = Vector3.Distance(playerTransform.position, p.Transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest  = p;
                }
            }

            return nearest;
        }
    }
}