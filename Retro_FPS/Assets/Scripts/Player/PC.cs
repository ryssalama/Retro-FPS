﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour {

	[Header("Ground Movement")]
	public float moveSpeed = 8f;
	public float acceleration = 25f;
	public float deacceleration = 15f;
	public float jumpHeight = 6f;

	[Header("Air Movement")]
	public float airAcceleration = 10f;

	[Header ("Shared")]
	public float gravity = 9.83f;

	[Header ("Camera")]
	public float mouseSensitivity = 40f;
	public float defaultCameraHeight = .6f;
	private Vector3 camEuler;
	private Camera cam;

	[Header ("Movement Vars")]
	public Vector3 velocity;
	public Vector3 moveScale;
	public Vector3 wishDir;

	[Header ("Options")]
	public bool hideCursor = true;

	// Classes
	private CharacterController controller;
	private Player player;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		player = GetComponent<Player> ();
		cam = player.cam;

		// Setup camera
		cam.transform.localPosition = new Vector3 (0, defaultCameraHeight, 0);

		// Hide cursor
		if (hideCursor) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	
	// Update is called once per frame
	void Update () {
		CheckInput ();
		CameraRotation ();
		Deaccelerate ();
		ApplyGravity ();
		Accelerate ();
		ApplyVelocity ();
	}

	void Accelerate() {
		// Calculate move scale
		wishDir = PlayerInput.GetMovementInput() * acceleration * Time.deltaTime;
		wishDir.x = Mathf.Clamp (wishDir.x, -1f, 1f);
		wishDir.z = Mathf.Clamp (wishDir.z, -1f, 1f);

		moveScale.x = Mathf.MoveTowards (moveScale.x, wishDir.x * moveSpeed, acceleration * Time.deltaTime);
		moveScale.z = Mathf.MoveTowards (moveScale.z, wishDir.z * moveSpeed, acceleration * Time.deltaTime);
		velocity = transform.TransformDirection (moveScale);
	}

	public void Deaccelerate() {
		
	}

	void ApplyVelocity() {
		controller.Move (velocity * Time.deltaTime);
	}

	// Camera rotation
	void CameraRotation() {
		// Add input
		camEuler.x -= PlayerInput.GetMouseInput().y;
		camEuler.y += PlayerInput.GetMouseInput().x;

		// Clamp x euler
		camEuler.x = Mathf.Clamp (camEuler.x, -89, 89);

		// Apply rotations
		cam.transform.localRotation = Quaternion.Euler (new Vector3 (camEuler.x, 0, 0));
		transform.localRotation = Quaternion.Euler (new Vector3 (0, camEuler.y, 0));
	}

	// Jump
	void Jump() {
		moveScale.y = jumpHeight * Time.fixedDeltaTime;
	}

	void ApplyGravity() {

		float gravityScale = 1f;
		if (controller.isGrounded) {
			gravityScale = 0;
		}

		moveScale.y -= gravity * Time.deltaTime * gravityScale;
		moveScale.y = Mathf.Clamp (moveScale.y, -50, 100);
	}

	// Check for player input
	void CheckInput() {

		// Jump
		if (Input.GetKeyDown (KeyCode.Space)) {
			Jump ();
		}

		// Show cursor when pressing ESCAPE
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}

// Input class
public static class PlayerInput {
	public static Vector3 GetMovementInput() {
		return new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
	}

	public static Vector2 GetMouseInput() {
		return new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
	}
}