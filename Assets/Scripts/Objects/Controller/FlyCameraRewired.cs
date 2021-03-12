﻿using UnityEngine;
using System.Collections;
using Rewired;

/* 
wasdqe : basic movement
hold shift : Makes camera accelerate
hold space : Moves camera on X and Z axis only.  So camera doesn't gain any height
*/

public class FlyCameraRewired : MonoBehaviour
{
    public int playerId = 0; // The Rewired player id of this controller
    private Player player;  //The Rewired player
    public float mainSpeed = 5.0f; //regular speed
    public float accelerateIncrement = 10.0f; //multiplied by how long acclerate key is held.  Basically running
    public float maxSpeed = 100.0f; //Maximum speed when accelerating
    public float rotateSensitivity = 0.25f; //How sensitive it with mouse
    public bool rotateOnMouseDown = true;

    [Header("BOUNDARIES")]
    public bool boundaryEnabled = true;
    public Vector2 xRange = new Vector2(-50, 50);
    public Vector2 yRange = new Vector2(-50, 50);
    public Vector2 zRange = new Vector2(-50, 50);

    private Vector3 lastMouse;
    private float totalRun = 1.0f;

    //input stats
    private Vector3 moveVector; //{AWSDQE} - Horizontal, vertical, QE thing
    private bool accelerated;   //{Shift}
    private bool movementStaysFlat; //{Space} - Movement on x & z axises only

    void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
    }

    void Update()
    {
        GetInput();
        ProcessInput();
        ProcessBoundary();
    }

    private void ProcessBoundary()
    {
        if (!boundaryEnabled) return;
        Vector3 newPosition;
        newPosition.x = Mathf.Clamp(transform.position.x, xRange.x, xRange.y);
        newPosition.y = Mathf.Clamp(transform.position.y, yRange.x, yRange.y);
        newPosition.z = Mathf.Clamp(transform.position.z, zRange.x, zRange.y);

        transform.position = newPosition;
    }

    private void ProcessInput()
    {
        ProcessRotationInput();
        ProcessAccelerationInput();
        ProcessMovementInput();
    }

    private void ProcessMovementInput()
    {
        moveVector = moveVector * Time.deltaTime;

        if (movementStaysFlat)
        {
            Vector3 newPosition = transform.position;
            transform.Translate(moveVector);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            transform.Translate(moveVector);
        }
    }

    private void ProcessAccelerationInput()
    {
        if (accelerated)
        {
            totalRun += Time.deltaTime;
            moveVector = moveVector * totalRun * accelerateIncrement;
            moveVector.x = Mathf.Clamp(moveVector.x, -maxSpeed, maxSpeed);
            moveVector.y = Mathf.Clamp(moveVector.y, -maxSpeed, maxSpeed);
            moveVector.z = Mathf.Clamp(moveVector.z, -maxSpeed, maxSpeed);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            moveVector = moveVector * mainSpeed;
        }
    }

    private void ProcessRotationInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMouse = Input.mousePosition; // $CTK reset when we begin
        }

        if (!rotateOnMouseDown || (rotateOnMouseDown && Input.GetMouseButton(1)))
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * rotateSensitivity, lastMouse.x * rotateSensitivity, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
        }
    }

    //TODO - Replace string references
    private void GetInput()
    {
        moveVector += new Vector3(player.GetAxis("Move Horizontal"), player.GetAxis("Move Vertical"), player.GetAxis("Move Zedical"));
        accelerated = player.GetButton("Accelerate");
        movementStaysFlat = player.GetButton("Stay Flat");
    }
}