using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class OptimizedGridSpawner : MonoBehaviourPunCallbacks
{
    public GameObject cubePrefab;
    public int gridSize = 6;
    public float tileSize = 1.0f;
    public float scaleDuration = 0.5f;
    public float baseMoveAmplitude = 0.2f;
    public float moveSpeed = 2f;
    public float changeInterval = 2f;
    public int maxRandomCubes = 3;
    public float exclusionRadius = 3f;

    private Dictionary<Vector3, GameObject> activeCubes = new Dictionary<Vector3, GameObject>();
    private GameObject[] movingCubes;
    private float nextChangeTime;
    private GameObject localPlayer;

    void Start()
    {
        FindLocalPlayer();
        if (localPlayer != null)
        {
            UpdateGrid();
            nextChangeTime = Time.time + changeInterval;
        }
    }

    void Update()
    {
        if (localPlayer != null)
        {
            UpdateGrid();
            HandleRandomCubeMotion();
        }
    }

    void FindLocalPlayer()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                localPlayer = player;
                break;
            }
        }
    }

    void UpdateGrid()
    {
        Vector3 playerGridPos = GetWorldGridPosition(localPlayer.transform.position);
        float halfGrid = (gridSize / 2) * tileSize;

        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int z = -gridSize / 2; z <= gridSize / 2; z++)
            {
                Vector3 spawnPos = new Vector3(playerGridPos.x + x * tileSize, 0, playerGridPos.z + z * tileSize);

                if (!activeCubes.ContainsKey(spawnPos))
                {
                    GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
                    activeCubes[spawnPos] = cube;
                    StartCoroutine(ScaleCube(cube, Vector3.zero, Vector3.one, scaleDuration));
                }
            }
        }

        // Clean up cubes outside the grid range
        List<Vector3> cubesToRemove = new List<Vector3>();
        foreach (var kvp in activeCubes)
        {
            if (Mathf.Abs(kvp.Key.x - playerGridPos.x) > halfGrid || Mathf.Abs(kvp.Key.z - playerGridPos.z) > halfGrid)
            {
                StartCoroutine(ScaleCube(kvp.Value, Vector3.one, Vector3.zero, scaleDuration));
                cubesToRemove.Add(kvp.Key);
            }
        }

        foreach (var pos in cubesToRemove)
        {
            activeCubes.Remove(pos);
        }
    }

    Vector3 GetWorldGridPosition(Vector3 playerPosition)
    {
        return new Vector3(Mathf.Floor(playerPosition.x / tileSize) * tileSize, 0, Mathf.Floor(playerPosition.z / tileSize) * tileSize);
    }

    IEnumerator ScaleCube(GameObject cube, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cube.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cube.transform.localScale = endScale;
        if (endScale == Vector3.zero) Destroy(cube);
    }

    void HandleRandomCubeMotion()
    {
        if (Time.time >= nextChangeTime)
        {
            if (movingCubes != null)
            {
                foreach (GameObject cube in movingCubes)
                {
                    if (cube != null) StopCoroutine(MoveCubeUpDown(cube));
                }
            }

            var edgeCubes = GetEdgeCubes();
            int cubesToSelect = Mathf.Min(maxRandomCubes, edgeCubes.Count);
            movingCubes = new GameObject[cubesToSelect];

            for (int i = 0; i < cubesToSelect; i++)
            {
                GameObject randomCube = edgeCubes[Random.Range(0, edgeCubes.Count)];
                movingCubes[i] = randomCube;
                StartCoroutine(MoveCubeUpDown(randomCube));
            }

            nextChangeTime = Time.time + changeInterval;
        }
    }

    List<GameObject> GetEdgeCubes()
    {
        List<GameObject> edgeCubes = new List<GameObject>();
        foreach (var kvp in activeCubes)
        {
            float distanceFromPlayer = Vector3.Distance(kvp.Key, localPlayer.transform.position);
            if (distanceFromPlayer > exclusionRadius)
            {
                edgeCubes.Add(kvp.Value);
            }
        }
        return edgeCubes;
    }

    IEnumerator MoveCubeUpDown(GameObject cube)
    {
        Vector3 startPos = cube.transform.position;
        float amplitude = baseMoveAmplitude * (Vector3.Distance(startPos, localPlayer.transform.position) / gridSize);
        float elapsed = 0f;

        while (true)
        {
            float newY = Mathf.PingPong(elapsed * moveSpeed, amplitude * 2) - amplitude;
            cube.transform.position = new Vector3(cube.transform.position.x, startPos.y + newY, cube.transform.position.z);

            if (Mathf.Approximately(cube.transform.position.y, startPos.y)) break;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
