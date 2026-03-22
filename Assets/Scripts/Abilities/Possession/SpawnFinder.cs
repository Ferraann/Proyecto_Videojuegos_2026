using UnityEngine;

namespace Possession
{
    public static class SpawnFinder
    {
        private const int MaxAttempts = 20;

        /// <summary>
        /// Busca el punto libre más cercano al origen dentro del radio dado.
        /// Si no encuentra ninguno, devuelve el origen como fallback.
        /// </summary>
        public static Vector3 FindFreePosition(Vector3 origin, float searchRadius, float playerHeight = 1.8f)
        {
            float playerHalfHeight = playerHeight / 2f;
            float playerRadius     = 0.4f;

            for (int i = 0; i < MaxAttempts; i++)
            {
                float angle  = i * (360f / MaxAttempts) * Mathf.Deg2Rad;
                float dist   = searchRadius * ((i + 1f) / MaxAttempts);

                Vector3 candidate = origin + new Vector3(
                    Mathf.Cos(angle) * dist,
                    0f,
                    Mathf.Sin(angle) * dist
                );

                if (IsPositionFree(candidate, playerHalfHeight, playerRadius))
                {
                    Debug.Log($"[SpawnFinder] Posición libre encontrada en: {candidate}");
                    return candidate;
                }
            }

            Debug.LogWarning("[SpawnFinder] No se encontró posición libre, usando origen como fallback.");
            return origin;
        }

        private static bool IsPositionFree(Vector3 center, float halfHeight, float radius)
        {
            // Comprueba si una cápsula del tamaño del jugador cabe en esa posición
            Vector3 bottom = center + Vector3.up * radius;
            Vector3 top    = center + Vector3.up * (halfHeight * 2f - radius);

            return !Physics.CheckCapsule(bottom, top, radius);
        }
    }
}
