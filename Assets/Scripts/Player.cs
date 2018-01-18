using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Pawns;
using UnityEngine;

public class Player : Pawn
{

    public float Speed;

    public float JumpForce;

    private bool isGrounded = true;

    /// <summary>
    /// This is used for if there are multiple players in a game.
    /// </summary>
    public int PlayerNum;

    /// <summary>
    /// This a shallow copy of the player ogtten when the game first loaded.
    /// </summary>
    private Player startingPlayerCopy;

    public Player()
    {
        Died += PlayerDied;
    }

    public void Awake()
    {
        startingPlayerCopy = (Player)Clone();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Controllable)
        {
            return;
        }

        Move();

        if (Input.GetKey("space"))
        {
            Jump();
        }
        else if (Input.GetKeyUp("space"))
        {
            StopJumping();
        }
    }

    private void Move()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        if (Math.Abs(move.x) > .1f)
        {
            transform.rotation = Quaternion.LookRotation(move.x < 0 ? Vector3.back : Vector3.forward, Vector3.up);
        }
        transform.position += move * Speed * Time.deltaTime;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void StopJumping()
    {
        var rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = new Vector2(0, 0);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            var rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.velocity = new Vector2(0, JumpForce);
            isGrounded = false;
        }

    }

    /// <summary>
    /// Calls the GameManager PlayerDied().
    /// </summary>
    private void PlayerDied(object sender, EventArgs args)
    {
        GameManager.GameManagerInst.PlayerDied(PlayerNum);
    }

    public void Reset()
    {

    }
}
