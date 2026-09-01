using UnityEngine;
using System.Collections.Generic;

public class MiniMapEnemyMarkers : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform miniMapMask;
    [SerializeField] private GameObject enemyMarkerPrefab;

    [Header("MiniMap")]
    [SerializeField] private float mapWorldSize = 50f;

    private Dictionary<Transform, RectTransform> enemyMarkers
        = new Dictionary<Transform, RectTransform>();

    private float enemyChecktimer = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            GameObject marker = Instantiate(enemyMarkerPrefab, miniMapMask);

            enemyMarkers.Add(
                enemy.transform,
                marker.GetComponent<RectTransform>()
                );
        }
    }

    private void Update()
    {        
        enemyChecktimer += Time.deltaTime;

        if (enemyChecktimer >= 1f)
        {
            FindNewEnemies();
            enemyChecktimer = 0f;
        }

        foreach (var pair in enemyMarkers)
        {
            Transform enemy = pair.Key;
            RectTransform marker = pair.Value;

            if (enemy == null)
                continue;

            Vector3 worldOffset = enemy.position - player.position;
            Vector3 offset = Quaternion.Euler(0, -player.eulerAngles.y, 0) * worldOffset;

            float x = offset.x / mapWorldSize * miniMapMask.rect.width;
            float y = offset.z / mapWorldSize * miniMapMask.rect.height;

            marker.anchoredPosition = new Vector2(x, y);
        }
    }

    private void FindNewEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Debug.Log("Minimap found enemies: " + enemies.Length);

        foreach (GameObject enemy in enemies)
        {
            if (!enemyMarkers.ContainsKey(enemy.transform))
            {
                GameObject marker = Instantiate(enemyMarkerPrefab, miniMapMask);

                enemyMarkers.Add(
                    enemy.transform,
                    marker.GetComponent<RectTransform>()
                );
            }
        }
    }
}
