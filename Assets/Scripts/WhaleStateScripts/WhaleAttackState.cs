using UnityEngine;

public class WhaleAttackState : WhaleBaseState
{
    // Game manager
    private GameManager gameManagerScript;

    // Mouse
    private Camera mainCamera;
    private Vector3 mousePosition;

    // Whale attack read only params
    private readonly float whaleAttackRotateSpeed = 20f;
    private readonly int attackTime = 30;

    // Whale attack params
    private float whaleAttackSpeed = 1f;
    private float whaleStepSlice = 10f;
    private int attackTimeCounter = 0;

    public bool whaleAttack = false;

    // Whale attacking positions
    private float attackStepX;
    private float attackStepY;
    private float destinationRangeDelta = 1f;
    private float destinationXDelta = 0f;
    private float destinationYDelta = 0f;

    // Whale attacking speed const
    private const float maxAttackSpeed = 22f;
    private const float minAttackSpeed = 18f;

    public override void EnterState(WhaleStateManager whale)
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();
        nextStepPosition = Vector3.zero;
        nextPosition = Vector3.zero;
        prevPosition = Vector3.zero;
        mainCamera = Camera.main;
    }

    public override void UpdateState(WhaleStateManager whale)
    {
        whale.whaleRotateSpeed = whaleAttackRotateSpeed;
        whale.whaleSpeed = whaleAttackSpeed;

        gameManagerScript.whalesAttacking = whaleAttack;

        if (whaleAttack)
        {
            whaleStepSlice = 1;
            whaleAttackSpeed = UtilFunctions.GetRandomDoubleInRange(minAttackSpeed, maxAttackSpeed);
            attackTimeCounter++;
            bool stopAttacking = attackTimeCounter >= attackTime;
            if (stopAttacking)
            {
                attackTimeCounter = 0;
                whaleAttack = false;
                gameManagerScript.whalesAttacking = false;
            }
        }
        else
        {
            whaleStepSlice = 10;
            whaleAttackSpeed = 1;

            mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 currentPosition = whale.transform.position;
            Vector3 destination = new Vector3(mousePosition.x + destinationXDelta, mousePosition.y + destinationYDelta, 0);
            Vector3 nextStep = UtilFunctions.GetNextStepByDestinationPoint2D(currentPosition, destination, whaleStepSlice);

            attackStepX = nextStep.x;
            attackStepY = nextStep.y;
        }
        nextStepPosition = new Vector3(attackStepX, attackStepY, 0);
    }

    public override void OnCollisionEnter2D(WhaleStateManager whale, Collision2D collision)
    {

    }

    public override void OnTriggerEnter2D(WhaleStateManager whale, Collider2D collision)
    {
        bool validEnemyTagName = collision.gameObject.tag == "MeteorBody" || collision.gameObject.tag == "AlienBody";

        if (validEnemyTagName && gameManagerScript.whalesAttacking)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.AttackByPlayer(whale.damagePoints);
        }
    }

    public override void LeftMouseButtonClicked()
    {
        if (!whaleAttack)
        {
        whaleAttack = true;
        gameManagerScript.whalesAttacking = true;
        destinationXDelta = UtilFunctions.GetRandomDoubleInRange(-destinationRangeDelta, destinationRangeDelta);
        destinationYDelta = UtilFunctions.GetRandomDoubleInRange(-destinationRangeDelta, destinationRangeDelta);
        }
    }
}
