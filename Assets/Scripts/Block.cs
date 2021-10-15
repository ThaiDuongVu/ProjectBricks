using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Block : MonoBehaviour
{
    public BlockType type;

    private Animator animator { get; set; }
    private static readonly int IsPressingAnimationTrigger = Animator.StringToHash("isPressing");
    private static readonly int ExplodeAnimationTrigger = Animator.StringToHash("explode");

    public Vector2Int GridPosition { get; set; }
    private Vector2 worldPosition;

    private const float RaycastDistance = 10f;
    private const float InterpolationRatio = 0.2f;

    private const float ExplosionDelay = 0.2f;

    private InputManager inputManager;

    /// <summary>
    /// Unity Event function.
    /// On current object enabled.
    /// </summary>
    private void OnEnable()
    {
        inputManager = new InputManager();

        // Handle press input
        inputManager.Game.Press.started += OnPressStarted;
        inputManager.Game.Press.canceled += OnPressCanceled;

        inputManager.Enable();
    }

    #region Input Methods

    /// <summary>
    /// On press input performed.
    /// </summary>
    /// <param name="context">Input context</param>
    private void OnPressStarted(InputAction.CallbackContext context)
    {
        animator.SetBool(IsPressingAnimationTrigger, true);
    }

    /// <summary>
    /// On press input canceled.
    /// </summary>
    /// <param name="context">Input context</param>
    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        animator.SetBool(IsPressingAnimationTrigger, false);
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
    /// Get component references.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        worldPosition = transform.position;
    }

    /// <summary>
    /// Unity Event function.
    /// Update once per frame.
    /// </summary>
    private void FixedUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, worldPosition, InterpolationRatio);
    }

    /// <summary>
    /// Move block at a direction on grid.
    /// </summary>
    /// <param name="direction">Direction to move</param>
    public void Move(Vector2 direction, Vector2Int maxGridPosition, Vector2Int minGridPosition, Vector2 gridBlockUnit, Block[,] occupiedGrid)
    {
        // Perform raycast to find new grid position and world position
        var hits = Physics2D.RaycastAll(transform.position, direction, RaycastDistance, LayerMask.GetMask("Blocks"));

        if (direction == Vector2.up) GridPosition = new Vector2Int(GridPosition.x, maxGridPosition.y - hits.Length);
        else if (direction == Vector2.down) GridPosition = new Vector2Int(GridPosition.x, minGridPosition.y + hits.Length);
        else if (direction == Vector2.left) GridPosition = new Vector2Int(minGridPosition.x + hits.Length, GridPosition.y);
        else if (direction == Vector2.right) GridPosition = new Vector2Int(maxGridPosition.x - hits.Length, GridPosition.y);

        worldPosition = GridPosition * gridBlockUnit;

        // Set new position in grid array
        occupiedGrid[GridPosition.x + 2, GridPosition.y + 2] = this;
    }

    /// <summary>
    /// Explode and destroy current block.
    /// </summary>
    public IEnumerator Explode()
    {
        yield return new WaitForSeconds(ExplosionDelay);
        animator.SetTrigger(ExplodeAnimationTrigger);

        yield return new WaitForSeconds(ExplosionDelay);
        CameraShaker.Instance.Shake(CameraShakeMode.Light);
        Destroy(gameObject);
    }
}
