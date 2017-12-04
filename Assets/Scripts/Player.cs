using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Directions the player can face.
/// </summary>
public enum Direction
{
    Up = 0,
    Right = 270,
    Down = 180,
    Left = 90
}

/// <summary>
/// The Main Player Script.
/// Controls Movement, Trailing light, Collision.
/// </summary>
public class Player : MonoBehaviour
{

    #region PublicVariables

    /// <summary>
    /// The current player who has control of this player. IE: Player One, Player Two
    /// </summary>
    public int PossesingPlayer;

    /// <summary>
    /// How Fast the Player Will go.
    /// </summary>
    public float Speed;

    #endregion

    #region PrivateVariables

    /// <summary>
    /// Less time means more turns per second. .2 is a good number.
    /// </summary>
    [SerializeField] private float timeBeforeCanMove = .5f;

    /// <summary>
    /// Where the player is going at anytime.
    /// </summary>
    private Vector3 movementDirection;

    /// <summary>
    /// count real time. My Delta Time.
    /// </summary>
    private float deltaTime;

    /// <summary>
    /// used to reset postion when restarting the game.
    /// </summary>
    private Vector2 startingOffsetMin;

    /// <summary>
    /// used to reset postion when restarting the game.
    /// </summary>
    private Vector2 startingOffsetMax;

    /// <summary>
    /// The GameObject of the Trail behind the Player.
    /// </summary>
    private GameObject trailGameObject;

    /// <summary>
    /// The GameObject of the Trail behind the Player.
    /// </summary>
    private GameObject lastTrailGameObject;

    /// <summary>
    /// The RectTransform of the Trail behind the Player.
    /// </summary>
    private RectTransform trailTransform;

    /// <summary>
    /// The BoxCollider of the Trail behind the Player.
    /// </summary>
    private BoxCollider2D trailBoxCollider;

    /// <summary>
    /// width of the trail.
    /// </summary>
    private const int Width = 7;

    #endregion

    #region PublicMethods

    /// <summary>
    /// Intialize values.
    /// </summary>
    public void Start()
    {
        Time.timeScale = 1;

        movementDirection = new Vector3(0, PossesingPlayer == 1 ? Speed : -Speed, 0);
        transform.rotation = Quaternion.Euler(0, 0, PossesingPlayer == 1 ? (int)Direction.Up : (int)Direction.Down);
        
        var rectTransform = GetComponent<RectTransform>();
        startingOffsetMin = rectTransform.offsetMin;
        startingOffsetMax = rectTransform.offsetMax;

        CreateTrail(Quaternion.Euler(0, 0, 0));
    }

    // Update is called once per frame
    public void Update()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime > timeBeforeCanMove)
        {
            var moveInput = PossesingPlayer == 1
                ? new Vector3(Input.GetAxis("Horizontal1"), Input.GetAxis("Vertical1"), 0)
                : new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0);

            if ((moveInput.y > 0 || moveInput.y < 0) && (movementDirection.x > 0 || movementDirection.x < 0))
            {
                CreateTrail(Quaternion.Euler(0, 0, 90));
            }
            else if ((moveInput.x > 0 || moveInput.x < 0) && (movementDirection.y > 0 || movementDirection.y < 0))
            {
                CreateTrail(Quaternion.Euler(0, 0, 0));
            }

            SetMovementDirection(moveInput);
        }

        IncreaseSpeed(Time.deltaTime);
        SetTrail();

        transform.position += movementDirection * Time.deltaTime;
    }

    /// <summary>
    /// Allows the player to collide with his own trails again.
    /// </summary>
    /// <returns>Couroutine</returns>
    private IEnumerator SetTagAndLayer()
    {
        yield return new WaitForSeconds(timeBeforeCanMove - .1f);
        if (lastTrailGameObject != null)
        {
            lastTrailGameObject.tag = "Untagged";
            lastTrailGameObject.layer = 8;
        }
    }

    /// <summary>
    /// Creates a trail at the bottom of the bike.
    /// </summary>
    private void CreateTrail(Quaternion rotation)
    {
        var height = 4.5f;

        if (trailTransform)
        {
            trailTransform.SetParent(GameObject.Find("Trails").transform);
        }
        //Only place the collider for the last trail after a grace period
        if (trailGameObject)
        {
            lastTrailGameObject = trailGameObject;
            StartCoroutine(SetTagAndLayer());
        }

        trailGameObject = new GameObject("Trail");
        trailGameObject.SetActive(true);
        trailGameObject.tag = PossesingPlayer == 1 ? "PlayerOne" : "PlayerTwo";
        trailGameObject.layer = PossesingPlayer == 1 ? 9 : 10;

        trailTransform = trailGameObject.AddComponent<RectTransform>();

        trailTransform.SetParent(transform); 
        trailTransform.SetAsFirstSibling();
        trailTransform.anchorMax = new Vector2(.5f, .5f);
        trailTransform.anchorMin = new Vector2(.5f, .5f);
        trailTransform.anchoredPosition = new Vector2(0, -23.75f);
        trailTransform.sizeDelta = new Vector2(Width, height);

        var image = trailGameObject.AddComponent<Image>();
        image.color = PossesingPlayer == 1 ? Color.cyan : Color.red;

        trailBoxCollider = trailGameObject.AddComponent<BoxCollider2D>();
        trailBoxCollider.size = new Vector2(Width, height);

        var rigidbody2D = trailGameObject.AddComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.useFullKinematicContacts = true;

        trailTransform.rotation = rotation;
    }

    /// <summary>
    /// Increase the Speed of the Player Movement.
    /// </summary>
    /// <param name="amount">How much to increase by, can be negative.</param>
    private void IncreaseSpeed(float amount)
    {
        Speed += amount;
        movementDirection.Normalize();
        movementDirection *= Speed;
    }

    /// <summary>
    /// Game is Over if any collision occurs.
    /// </summary>
    /// <param name="other">other object you collided with.</param>
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag(PossesingPlayer == 1 ? "PlayerOne" : "PlayerTwo"))
        {
            GameManager.GameManagerInst.GameEnded();
        }
    }

    #endregion

    #region PrivateMethods

    /// <summary>
    /// Return player to starting locationa and reset stats if applicable
    /// </summary>
    public void Reset()
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMin = startingOffsetMin;
        rectTransform.offsetMax = startingOffsetMax;
        rectTransform.rotation = Quaternion.Euler(0, 0, PossesingPlayer == 1 ? (int)Direction.Up : (int)Direction.Down);
        movementDirection = new Vector3(0, PossesingPlayer == 1 ? Speed : -Speed, 0);
        CreateTrail(Quaternion.Euler(0, 0, 0));

        var trails = GameObject.Find("Trails").transform;
        for(var i = 0; i < trails.childCount; ++i)
        {
            Destroy(trails.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Ajusts the height and y position of the trail to the speed the bike is going.
    /// </summary>
    private void SetTrail()
    {
        var transformPosition = transform.position;
        var offset = movementDirection * Time.deltaTime;
        var newTransformPosition = transform.position + offset;

        Rect rect = new Rect(trailTransform.rect);
        if (newTransformPosition.x > transformPosition.x || newTransformPosition.x < transformPosition.x)
        {
            rect.height += Math.Abs(offset.x);
            rect.y += offset.x / 2;
        }
        else if (newTransformPosition.y > transformPosition.y || newTransformPosition.y < transformPosition.y)
        {
            rect.height += Math.Abs(offset.y);
            rect.y += offset.y / 2;
        }

        trailTransform.anchoredPosition = new Vector2(0,rect.y);
        trailTransform.sizeDelta = new Vector2(Width, rect.height);
        trailBoxCollider.size = trailTransform.sizeDelta;
    }

    /// <summary>
    /// Handles Turn left, right, top, bot 
    /// </summary>
    /// <param name="moveInput">The movement in the horizontal and vertical direction</param>
    private void SetMovementDirection(Vector3 moveInput)
    {
        if (moveInput.x > 0 || moveInput.x < 0)
        {
            //Can't move backwards
            if (movementDirection.x < 0 && moveInput.x > 0 || movementDirection.x > 0 && moveInput.x < 0)
            {
                return;
            }
            var rectTransform = GetComponent<RectTransform>();
            bool goingLeft = moveInput.x < 0;
            rectTransform.rotation = Quaternion.Euler(0, 0, goingLeft ? (int)Direction.Left : (int)Direction.Right);

            movementDirection = new Vector3(goingLeft ? -Speed : Speed, 0, 0);
            deltaTime = 0;
        }
        else if (moveInput.y > 0 || moveInput.y < 0)
        {
            //Can't move backwards
            if (movementDirection.y < 0 && moveInput.y > 0 || movementDirection.y > 0 && moveInput.y < 0)
            {
                return;
            }
            var rectTransform = GetComponent<RectTransform>();
            bool goingUp = moveInput.y > 0;
            rectTransform.rotation = Quaternion.Euler(0, 0, goingUp ? (int)Direction.Up : (int)Direction.Down);

            movementDirection = new Vector3(0, goingUp ? Speed : -Speed, 0);
            deltaTime = 0;
        }
    }

    #endregion

}
