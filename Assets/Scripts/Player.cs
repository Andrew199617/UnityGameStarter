using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
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

    #region Private Variables used for turning the player slowly instead of just rotating 90 degrees.

    /// <summary>
    /// Currently turning towards the player.
    /// </summary>
    private bool isTurning;

    /// <summary>
    /// The rotation to slowly turn to of the course timeBeforeCanMove.
    /// </summary>
    private Quaternion newRotation;

    /// <summary>
    /// The starting rotation when the pressed turn.
    /// </summary>
    private Quaternion startRotation;

    #endregion

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

    #region Trail variables used to set values for the trail behind the player.

    /// <summary>
    /// The GameObject of the Trail behind the Player.
    /// </summary>
    private GameObject trailGameObject;

    /// <summary>
    /// Nothing or the last trail behind the player.
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
    private const int Width = 8;

    #endregion

    #endregion

    #region PublicMethods

    /// <summary>
    /// Intialize values.
    /// </summary>
    public void Start()
    {

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

            if ((moveInput.y > 0 || moveInput.y < 0) &&
                Math.Abs(movementDirection.y) < 1.0f)
            {
                CreateTrail(Quaternion.Euler(0, 0, 90));
            }
            else if ((moveInput.x > 0 || moveInput.x < 0) &&
                Math.Abs(movementDirection.x) < 1.0f)
            {
                CreateTrail(Quaternion.Euler(0, 0, 0));
            }

            SetMovementDirection(moveInput);
            if (Math.Abs(deltaTime) < .01f)
            {
                isTurning = true;
            }
        }
        UpdateRotation(ref isTurning);

        IncreaseSpeed(Time.deltaTime);
        SetTrail();
        
        transform.position += movementDirection * Time.deltaTime;
    }

    private void UpdateRotation(ref bool isTurningParam)
    {
        if (isTurningParam)
        {
            var rectTransform = GetComponent<RectTransform>();
            //finish turning halfway through the players ability to turn again.
            var percentOfTimeBeforeCanMove = .5f;
            var speed = 90 / percentOfTimeBeforeCanMove;
            if (deltaTime / timeBeforeCanMove <= percentOfTimeBeforeCanMove)
            {
                rectTransform.rotation = Quaternion.RotateTowards(startRotation, newRotation, speed / timeBeforeCanMove * deltaTime);
            }
            else
            {
                rectTransform.rotation = newRotation;
                isTurningParam = false;
            }
        }
    }

    /// <summary>
    /// Allows the player to collide with his own trails again.
    /// </summary>
    /// <returns>Couroutine</returns>
    private IEnumerator SetTagAndLayer()
    {
        var myLastTrailGameObject = lastTrailGameObject;
        yield return new WaitForSeconds(timeBeforeCanMove * 2);
        if (myLastTrailGameObject)
        {
            myLastTrailGameObject.tag = "Untagged";
            myLastTrailGameObject.layer = 8;
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
        //Only let this player collide with the last trail after a grace period
        if (trailGameObject)
        {
            lastTrailGameObject = trailGameObject;
            StartCoroutine(SetTagAndLayer());
        }

        trailGameObject = new GameObject("Trail");
        trailGameObject.SetActive(true);
        trailGameObject.tag = PossesingPlayer == 1 ? "PlayerOne" : "PlayerTwo";
        trailGameObject.layer = PossesingPlayer == 1 ? LayerMask.NameToLayer("PlayerOne") : LayerMask.NameToLayer("PlayerTwo");

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

        var rigidbody2DComponent = trailGameObject.AddComponent<Rigidbody2D>();
        rigidbody2DComponent.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2DComponent.useFullKinematicContacts = true;

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
        OnDied();
    }

    #endregion

    #region PrivateMethods

    /// <summary>
    /// Return player to starting location and reset stats if applicable
    /// </summary>
    public void Reset()
    {
        //Has this player been intialized yet, if not he can not be reset.
        if (startingOffsetMax != new Vector2())
        {
            isTurning = false;
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.offsetMin = startingOffsetMin;
            rectTransform.offsetMax = startingOffsetMax;
            rectTransform.rotation = Quaternion.Euler(0, 0, PossesingPlayer == 1 ? (int)Direction.Up : (int)Direction.Down);
            movementDirection = new Vector3(0, PossesingPlayer == 1 ? Speed : -Speed, 0);
            CreateTrail(Quaternion.Euler(0, 0, 0));

            var trails = GameObject.Find("Trails").transform;
            for (var i = 0; i < trails.childCount; ++i)
            {
                Destroy(trails.GetChild(i).gameObject);
            }
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
        if ((moveInput.x > 0 || moveInput.x < 0) && 
            Math.Abs(movementDirection.x) < 1.0f)
        {
            var rectTransform = GetComponent<RectTransform>();
            bool goingLeft = moveInput.x < 0;
            startRotation = rectTransform.rotation;
            newRotation = Quaternion.Euler(0, 0, goingLeft ? (int)Direction.Left : (int)Direction.Right);

            movementDirection = new Vector3(goingLeft ? -Speed : Speed, 0, 0);
            deltaTime = 0;
        }
        else if ((moveInput.y > 0 || moveInput.y < 0) &&
                 Math.Abs(movementDirection.y) < 1.0f)
        {
            var rectTransform = GetComponent<RectTransform>();
            bool goingUp = moveInput.y > 0;
            startRotation = rectTransform.rotation;
            newRotation = Quaternion.Euler(0, 0, goingUp ? (int)Direction.Up : (int)Direction.Down);

            movementDirection = new Vector3(0, goingUp ? Speed : -Speed, 0);
            deltaTime = 0;
        }
    }

    /// <summary>
    /// Calls the GameManager PlayerDied().
    /// </summary>
    private void OnDied()
    {
        GameManager.GameManagerInst.PlayerDied(PossesingPlayer - 1);
    }

    #endregion

}
