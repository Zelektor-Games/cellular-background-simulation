using System.Collections;
using UnityEngine;

public class CellSpawner : MonoBehaviour
{
    public static CellSpawner Instanse;
    public static int CellsCount = 0;
    
    [Header("Links")]
    [SerializeField] private Cell _cellPrefab;
    
    [Header("Spawn Delay")]
    public float minDelay;
    public float maxDelay;

    [Header("Spawn Positions")]
    public Vector2 xPositions;
    public Vector2 yPositions;

    private void Awake()
    {
        Instanse = this;
        CellsCount = 0;
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            float newX = Random.Range(xPositions.x, xPositions.y);
            float newY = Random.Range(yPositions.x, yPositions.y);

            Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            SpawnNewCell(new Vector2(newX, newY), newColor);
        }
    }

    public void SpawnNewCell(Vector2 position, Color color)
    {
        Cell cell = Instantiate(_cellPrefab, position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        cell.sr.color = color;
    }
}
