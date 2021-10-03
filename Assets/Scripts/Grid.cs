using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grid : MonoBehaviour
{
    private static readonly Vector2Int gridSize = new Vector2Int(5, 5);
    private Block[,] occupiedGrid = new Block[gridSize.x, gridSize.y];
    private static readonly Vector2Int minGridPosition = new Vector2Int(-2, -2);
    private static readonly Vector2Int maxGridPosition = new Vector2Int(2, 2);
    private static readonly Vector2 gridBlockUnit = new Vector2(0.75f, 1f);
    private List<Block> blocks = new List<Block>();

    [SerializeField] private Block[] blockPrefabs;
    private static readonly Vector2Int initBlockNumbersRange = new Vector2Int(2, 4);

    private float swipeThreshold = 10f;
    private bool canSwipe = true;

    private const float SpawnDelay = 0.2f;

    private const int Match3Score = 50;
    private const int Match4Score = 100;
    private const int Match5Score = 150;
    private const int Grid4Score = 150;

    private int scoreAdded;
    private const int ScoreTier1 = 100;
    private const int ScoreTier2 = 150;
    private const int ScoreTier3 = 250;
    private string[] messageSet1 = new string[] { "Nice!", "Good!", "Well done!" };
    private string[] messageSet2 = new string[] { "Great!", "Awesome!", "Fantastic!" };
    private string[] messageSet3 = new string[] { "Incredible!", "Unbelievable!" };

    private InputManager inputManager;

    /// <summary>
    /// Unity Event function.
    /// On current object enabled.
    /// </summary>
    private void OnEnable()
    {
        inputManager = new InputManager();

        // Handle direction input
        inputManager.Game.Direction.performed += OnDirectionPerformed;
        inputManager.Game.KeyDirection.performed += OnKeyDirectionPerformed;
        // Test game over functionality
        inputManager.Debug.Test.performed += (InputAction.CallbackContext context) => { PlayerPrefs.DeleteAll(); };

        inputManager.Enable();
    }

    #region Input Methods

    /// <summary>
    /// On direction input performed.
    /// </summary>
    /// <param name="context">Input context</param>
    private void OnDirectionPerformed(InputAction.CallbackContext context)
    {
        if (GameController.Instance.State != GameState.Started) return;
        if (!canSwipe || context.ReadValue<Vector2>().magnitude < swipeThreshold) return;

        SwipeBlocks(context.ReadValue<Vector2>().normalized);
    }

    /// <summary>
    /// On direction input performed with keyboard.
    /// </summary>
    /// <param name="context">Input context</param>
    private void OnKeyDirectionPerformed(InputAction.CallbackContext context)
    {
        if (GameController.Instance.State != GameState.Started) return;
        if (!canSwipe) return;

        SwipeBlocks(context.ReadValue<Vector2>().normalized);
    }

    #endregion

    /// <summary>
    /// Unity Event function.
    /// On current object disabled.
    /// </summary>
    private void OnDisable()
    {
        inputManager.Disable();
    }

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        if (PlayerPrefs.GetInt("LastGameOver", 0) == 0)
        {
            // Spawn initial blocks on grid
            for (int x = 0; x < gridSize.x; x++) for (int y = 0; y < gridSize.y; y++) occupiedGrid[x, y] = null;
            for (int i = 0; i < Random.Range(initBlockNumbersRange.x, initBlockNumbersRange.y); i++) StartCoroutine(SpawnRandomBlock());

            GameController.Instance.Score = 0;
            SaveGridData();
        }
        else
        {
            // Spawn blocks from save data
            LoadGridData();
        }

        PlayerPrefs.SetInt("LastGameOver", 1);
    }

    /// <summary>
    /// Spawn a new block at a random position on the grid.
    /// </summary>
    private IEnumerator SpawnRandomBlock()
    {
        yield return new WaitForSeconds(SpawnDelay);

        var gridSpawnPosition = new Vector2Int(Random.Range(minGridPosition.x, maxGridPosition.x),
                                               Random.Range(minGridPosition.y, maxGridPosition.y));

        if (!occupiedGrid[gridSpawnPosition.x + 2, gridSpawnPosition.y + 2])
        {
            var spawnPosition = gridSpawnPosition * gridBlockUnit;
            var block = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)], spawnPosition, Quaternion.identity);
            block.GridPosition = gridSpawnPosition;
            blocks.Add(block);
            occupiedGrid[gridSpawnPosition.x + 2, gridSpawnPosition.y + 2] = block;
        }

        SaveGridData();
        CheckGrid();
        if (GridFull()) GameController.Instance.GameOver();
        
        canSwipe = true;
    }

    /// <summary>
    /// Spawn a new block at a defined position on the grid.
    /// </summary>
    private IEnumerator SpawnDefinedBlock(int x, int y, int type)
    {
        yield return new WaitForSeconds(SpawnDelay);

        var gridSpawnPosition = new Vector2Int(x - 2, y - 2);
        var spawnPosition = gridSpawnPosition * gridBlockUnit;
        var block = Instantiate(blockPrefabs[type], spawnPosition, Quaternion.identity);
        block.GridPosition = gridSpawnPosition;
        blocks.Add(block);
        occupiedGrid[gridSpawnPosition.x + 2, gridSpawnPosition.y + 2] = block;

        SaveGridData();
        CheckGrid();
        if (GridFull()) GameController.Instance.GameOver();

        canSwipe = true;
    }

    /// <summary>
    /// Swipe all blocks on grid.
    /// </summary>
    /// <param name="value">Direction to swipe</param>
    private void SwipeBlocks(Vector2 value)
    {
        Vector2 direction = Vector2.zero;

        if (value.x > Mathf.Sqrt(0.5f)) direction = Vector2.right;
        else if (value.x < -Mathf.Sqrt(0.5f)) direction = Vector2.left;

        if (value.y > Mathf.Sqrt(0.5f)) direction = Vector2.up;
        else if (value.y < -Mathf.Sqrt(0.5f)) direction = Vector2.down;

        for (int x = 0; x < gridSize.x; x++) for (int y = 0; y < gridSize.y; y++) occupiedGrid[x, y] = null;
        foreach (var block in blocks) block.Move(direction, maxGridPosition, minGridPosition, gridBlockUnit, occupiedGrid);
        canSwipe = false;

        StartCoroutine(SpawnRandomBlock());
    }

    /// <summary>
    /// Check to see if all spots on the grid is occupied by a block.
    /// </summary>
    /// <returns>Whether grid is full</returns>
    private bool GridFull()
    {
        foreach (var block in occupiedGrid)
            if (!block) return false;

        return true;
    }

    /// <summary>
    /// Check grid for adjacent blocks and make them explode.
    /// </summary>
    private void CheckGrid()
    {
        CheckMatch5();
        CheckGrid4();
        CheckMatch4();
        CheckMatch3();

        // Send feedback and reset added score
        if (scoreAdded >= ScoreTier1 && scoreAdded < ScoreTier2)
            GameController.Instance.SendUIMessage(messageSet1[Random.Range(0, messageSet1.Length)]);
        else if (scoreAdded >= ScoreTier2 && scoreAdded < ScoreTier3)
            GameController.Instance.SendUIMessage(messageSet2[Random.Range(0, messageSet2.Length)]);
        else if (scoreAdded >= ScoreTier3)
            GameController.Instance.SendUIMessage(messageSet3[Random.Range(0, messageSet3.Length)]);
        scoreAdded = 0;

        // Remove every blocks from list and add them back in
        blocks.Clear();
        foreach (var block in occupiedGrid)
        {
            if (!block) continue;
            blocks.Add(block);
        }
    }

    /// <summary>
    /// Check if 3 blocks on a straight line (horizontally or vertically) are of the same color.
    /// </summary>
    private void CheckMatch3()
    {
        // Check match-3 horizontally
        for (int x = 0; x < occupiedGrid.GetLength(0) - 2; x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1); y++)
            {
                if (!occupiedGrid[x, y] || !occupiedGrid[x + 1, y] || !occupiedGrid[x + 2, y]) continue;

                BlockType[] types = new BlockType[3]
                    { occupiedGrid[x, y].type, occupiedGrid[x + 1, y].type, occupiedGrid[x + 2, y].type };
                List<BlockType> typeOccurences = new List<BlockType>();

                foreach (var type in types)
                    if (type != BlockType.All && !typeOccurences.Contains(type)) typeOccurences.Add(type);

                if (typeOccurences.Count <= 1)
                {
                    StartCoroutine(occupiedGrid[x, y].Explode());
                    StartCoroutine(occupiedGrid[x + 1, y].Explode());
                    StartCoroutine(occupiedGrid[x + 2, y].Explode());

                    occupiedGrid[x, y] = null;
                    occupiedGrid[x + 1, y] = null;
                    occupiedGrid[x + 2, y] = null;

                    GameController.Instance.Score += Match3Score;
                    scoreAdded += Match3Score;
                }
            }
        }
        // Check match-3 vertically
        for (int x = 0; x < occupiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1) - 2; y++)
            {
                if (!occupiedGrid[x, y] || !occupiedGrid[x, y + 1] || !occupiedGrid[x, y + 2]) continue;

                BlockType[] types = new BlockType[3]
                    { occupiedGrid[x, y].type, occupiedGrid[x, y + 1].type, occupiedGrid[x, y + 2].type };
                List<BlockType> typeOccurences = new List<BlockType>();

                foreach (var type in types)
                    if (type != BlockType.All && !typeOccurences.Contains(type)) typeOccurences.Add(type);

                if (typeOccurences.Count <= 1)
                {
                    StartCoroutine(occupiedGrid[x, y].Explode());
                    StartCoroutine(occupiedGrid[x, y + 1].Explode());
                    StartCoroutine(occupiedGrid[x, y + 2].Explode());

                    occupiedGrid[x, y] = null;
                    occupiedGrid[x, y + 1] = null;
                    occupiedGrid[x, y + 2] = null;

                    GameController.Instance.Score += Match3Score;
                    scoreAdded += Match3Score;
                }
            }
        }
    }

    /// <summary>
    /// Check if 4 blocks on a straight line (horizontally or vertically) are of the same color.
    /// </summary>
    private void CheckMatch4()
    {
        // Check match-4 horizontally
        for (int x = 0; x < occupiedGrid.GetLength(0) - 3; x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1); y++)
            {
                if (!occupiedGrid[x, y] || !occupiedGrid[x + 1, y] || !occupiedGrid[x + 2, y] || !occupiedGrid[x + 3, y]) continue;

                BlockType[] types = new BlockType[4]
                    { occupiedGrid[x, y].type, occupiedGrid[x + 1, y].type, occupiedGrid[x + 2, y].type, occupiedGrid[x + 3, y].type };
                List<BlockType> typeOccurences = new List<BlockType>();

                foreach (var type in types)
                    if (type != BlockType.All && !typeOccurences.Contains(type)) typeOccurences.Add(type);

                if (typeOccurences.Count <= 1)
                {
                    StartCoroutine(occupiedGrid[x, y].Explode());
                    StartCoroutine(occupiedGrid[x + 1, y].Explode());
                    StartCoroutine(occupiedGrid[x + 2, y].Explode());
                    StartCoroutine(occupiedGrid[x + 3, y].Explode());

                    occupiedGrid[x, y] = null;
                    occupiedGrid[x + 1, y] = null;
                    occupiedGrid[x + 2, y] = null;
                    occupiedGrid[x + 3, y] = null;

                    GameController.Instance.Score += Match4Score;
                    scoreAdded += Match4Score;
                }
            }
        }
        // Check match-4 vertically
        for (int x = 0; x < occupiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1) - 3; y++)
            {
                if (!occupiedGrid[x, y] || !occupiedGrid[x, y + 1] || !occupiedGrid[x, y + 2] || !occupiedGrid[x, y + 3]) continue;

                BlockType[] types = new BlockType[4]
                    { occupiedGrid[x, y].type, occupiedGrid[x, y + 1].type, occupiedGrid[x, y + 2].type, occupiedGrid[x, y + 3].type };
                List<BlockType> typeOccurences = new List<BlockType>();

                foreach (var type in types)
                    if (type != BlockType.All && !typeOccurences.Contains(type)) typeOccurences.Add(type);

                if (typeOccurences.Count <= 1)
                {
                    StartCoroutine(occupiedGrid[x, y].Explode());
                    StartCoroutine(occupiedGrid[x, y + 1].Explode());
                    StartCoroutine(occupiedGrid[x, y + 2].Explode());
                    StartCoroutine(occupiedGrid[x, y + 3].Explode());

                    occupiedGrid[x, y] = null;
                    occupiedGrid[x, y + 1] = null;
                    occupiedGrid[x, y + 2] = null;
                    occupiedGrid[x, y + 3] = null;

                    GameController.Instance.Score += Match4Score;
                    scoreAdded += Match4Score;
                }
            }
        }
    }

    /// <summary>
    /// Check if 5 blocks on a straight line (horizontally or vertically) are of the same color.
    /// </summary>
    private void CheckMatch5()
    {
        // Check match-4 horizontally
        for (int x = 0; x < occupiedGrid.GetLength(0) - 4; x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1); y++)
            {
                if (!occupiedGrid[x, y] || !occupiedGrid[x + 1, y] || !occupiedGrid[x + 2, y] || !occupiedGrid[x + 3, y] || !occupiedGrid[x + 4, y])
                    continue;

                BlockType[] types = new BlockType[5]
                    { occupiedGrid[x, y].type, occupiedGrid[x + 1, y].type, occupiedGrid[x + 2, y].type, occupiedGrid[x + 3, y].type, occupiedGrid[x + 4, y].type };
                List<BlockType> typeOccurences = new List<BlockType>();

                foreach (var type in types)
                    if (type != BlockType.All && !typeOccurences.Contains(type)) typeOccurences.Add(type);

                if (typeOccurences.Count <= 1)
                {
                    StartCoroutine(occupiedGrid[x, y].Explode());
                    StartCoroutine(occupiedGrid[x + 1, y].Explode());
                    StartCoroutine(occupiedGrid[x + 2, y].Explode());
                    StartCoroutine(occupiedGrid[x + 3, y].Explode());
                    StartCoroutine(occupiedGrid[x + 4, y].Explode());

                    occupiedGrid[x, y] = null;
                    occupiedGrid[x + 1, y] = null;
                    occupiedGrid[x + 2, y] = null;
                    occupiedGrid[x + 3, y] = null;
                    occupiedGrid[x + 4, y] = null;

                    GameController.Instance.Score += Match5Score;
                    scoreAdded += Match5Score;
                }
            }
        }
        // Check match-4 vertically
        for (int x = 0; x < occupiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1) - 4; y++)
            {
                if (!occupiedGrid[x, y] || !occupiedGrid[x, y + 1] || !occupiedGrid[x, y + 2] || !occupiedGrid[x, y + 3] || !occupiedGrid[x, y + 4])
                    continue;

                BlockType[] types = new BlockType[5]
                    { occupiedGrid[x, y].type, occupiedGrid[x, y + 1].type, occupiedGrid[x, y + 2].type, occupiedGrid[x, y + 3].type, occupiedGrid[x, y + 4].type };
                List<BlockType> typeOccurences = new List<BlockType>();

                foreach (var type in types)
                    if (type != BlockType.All && !typeOccurences.Contains(type)) typeOccurences.Add(type);

                if (typeOccurences.Count <= 1)
                {
                    StartCoroutine(occupiedGrid[x, y].Explode());
                    StartCoroutine(occupiedGrid[x, y + 1].Explode());
                    StartCoroutine(occupiedGrid[x, y + 2].Explode());
                    StartCoroutine(occupiedGrid[x, y + 3].Explode());
                    StartCoroutine(occupiedGrid[x, y + 4].Explode());

                    occupiedGrid[x, y] = null;
                    occupiedGrid[x, y + 1] = null;
                    occupiedGrid[x, y + 2] = null;
                    occupiedGrid[x, y + 3] = null;
                    occupiedGrid[x, y + 4] = null;

                    GameController.Instance.Score += Match5Score;
                    scoreAdded += Match5Score;
                }
            }
        }
    }

    /// <summary>
    /// Check if 4 blocks in a 2x2 square are all of the same color.
    /// </summary>
    private void CheckGrid4()
    {
        for (int x = 0; x < occupiedGrid.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1) - 1; y++)
            {
                if (!occupiedGrid[x, y] || !occupiedGrid[x + 1, y] || !occupiedGrid[x, y + 1] || !occupiedGrid[x + 1, y + 1]) continue;

                BlockType[] types = new BlockType[4]
                    { occupiedGrid[x, y].type, occupiedGrid[x + 1, y].type, occupiedGrid[x, y + 1].type, occupiedGrid[x + 1, y + 1].type };
                List<BlockType> typeOccurences = new List<BlockType>();

                foreach (var type in types)
                    if (type != BlockType.All && !typeOccurences.Contains(type)) typeOccurences.Add(type);

                if (typeOccurences.Count <= 1)
                {
                    StartCoroutine(occupiedGrid[x, y].Explode());
                    StartCoroutine(occupiedGrid[x + 1, y].Explode());
                    StartCoroutine(occupiedGrid[x, y + 1].Explode());
                    StartCoroutine(occupiedGrid[x + 1, y + 1].Explode());

                    occupiedGrid[x, y] = null;
                    occupiedGrid[x + 1, y] = null;
                    occupiedGrid[x, y + 1] = null;
                    occupiedGrid[x + 1, y + 1] = null;

                    GameController.Instance.Score += Grid4Score;
                    scoreAdded += Grid4Score;
                }
            }
        }
    }

    /// <summary>
    /// Save all blocks type information of current grid to player preferences.
    /// TODO: Consider using encoded binary files instead.
    /// </summary>
    private void SaveGridData()
    {
        for (int x = 0; x < occupiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1); y++)
            {
                if (!occupiedGrid[x, y]) PlayerPrefs.SetInt("Grid" + x.ToString() + y.ToString(), ((int)BlockType.None));
                else PlayerPrefs.SetInt("Grid" + x.ToString() + y.ToString(), ((int)occupiedGrid[x, y].type));
            }
        }

        PlayerPrefs.SetInt("CurrentScore", GameController.Instance.Score);
    }

    /// <summary>
    /// Load all blocks type from player preferences and spawn.
    /// </summary>
    private void LoadGridData()
    {
        for (int x = 0; x < occupiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < occupiedGrid.GetLength(1); y++)
            {
                if (PlayerPrefs.GetInt("Grid" + x.ToString() + y.ToString(), ((int)BlockType.None)) == ((int)BlockType.None))
                {
                    occupiedGrid[x, y] = null;
                    continue;
                }

                StartCoroutine(SpawnDefinedBlock(x, y, (PlayerPrefs.GetInt("Grid" + x.ToString() + y.ToString(), ((int)BlockType.None)))));
            }
        }

        GameController.Instance.Score = PlayerPrefs.GetInt("CurrentScore", 0);
    }
}
