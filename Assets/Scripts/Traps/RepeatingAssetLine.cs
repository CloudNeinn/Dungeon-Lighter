using System.Collections.Generic;
using UnityEngine;

public class RepeatingAssetLine : MonoBehaviour
{
    public GameObject assetPrefab; // your "*" prefab
    public List<Transform> points; // List of points to connect
    public float spacing = 1f;     // Distance between each instance

    private List<GameObject> _spawnedAssets = new List<GameObject>();

    void Start()
    {
        DrawAssets();
    }

    void DrawAssets()
    {
        ClearAssets();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Transform start = points[i];
            Transform end = points[i + 1];

            Vector3 direction = (end.position - start.position).normalized;
            float distance = Vector3.Distance(start.position, end.position);
            int count = Mathf.FloorToInt(distance / spacing);

            for (int j = 1; j < count; j++)
            {
                Vector3 position = start.position + direction * spacing * j;
                GameObject asset = Instantiate(assetPrefab, position, Quaternion.LookRotation(direction));
                _spawnedAssets.Add(asset);
            }
        }
    }

    void ClearAssets()
    {
        foreach (GameObject obj in _spawnedAssets)
        {
            Destroy(obj);
        }
        _spawnedAssets.Clear();
    }
}