using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class scr_player_controller : MonoBehaviour
{
    public event System.Action onFinishedLevel;

    public GameObject player_cam;

    public Transform orientation;
    public Transform player;
    public Transform obj_player;
    public Rigidbody rigidbd;

    private Vector2 input_movement;
    private bool input_disabled;

    //movement vars
    private float rotation_speed = 7f;
    private float move_speed = 4f;
    private Vector3 move_direction;

    //ground vars
    private float player_height = 1.8f;
    public LayerMask what_is_ground;
    private bool grounded;
    private float ground_drag = 5f;

    //jump vars
    private float jump_force = 8f;
    private float jump_cooldown = 0.25f;
    private float air_multiplier = 0.4f;
    private bool ready_to_jump;
    private bool is_jumping;

    private void Start()
    {
        rigidbd = GetComponent<Rigidbody>();
        rigidbd.freezeRotation = true;
        ready_to_jump = true;
        scr_guard.onGuardSpottedPlayer += disableInput;
    }

    public void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, player_height * 0.5f + 0.2f, what_is_ground);

        //handle drag
        if (grounded) { rigidbd.drag = ground_drag; }
        else { rigidbd.drag = 0; }

        speedControl();

        if (is_jumping) //continue jumping if jump key is held
        {
            if (ready_to_jump && grounded)
            {
                ready_to_jump = false;
                Jump();
                Invoke(nameof(resetJump), jump_cooldown);
            }
        }

        //rotate orientation
        Vector3 view_direction = player.position - new Vector3(player_cam.transform.position.x, player.position.y, player_cam.transform.position.z);
        if (view_direction != Vector3.zero) { orientation.forward = view_direction.normalized; } //if check needed to get rid of error message

        //rotate player object
        float horizontal_input = input_movement.x;
        float vertical_input = input_movement.y;
        Vector3 input_direction = orientation.forward * vertical_input + orientation.right * horizontal_input;

        if (input_direction != Vector3.zero)
        {
            obj_player.forward = Vector3.Slerp(obj_player.forward, input_direction.normalized, Time.deltaTime * rotation_speed);
        }
    }

    private void FixedUpdate()
    {
        if (!input_disabled) { movePlayer(); }
    }

    public void inputUpdateMovement(InputAction.CallbackContext context)
    {
        Vector3 input_direction = Vector3.zero;
        if (!input_disabled)
        {
            if (context.performed) //key pressed
            {
                input_movement.x = context.ReadValue<Vector2>().x;
                input_movement.y = context.ReadValue<Vector2>().y;
            }

            if (context.canceled) //key released
            {
                input_movement.x = context.ReadValue<Vector2>().x;
                input_movement.y = context.ReadValue<Vector2>().y;
            }
        }
        else { input_movement = Vector2.zero; }
    }

    private void disableInput() { input_disabled = true; }

    private void movePlayer()
    {
        move_direction = orientation.forward * input_movement.y + orientation.right * input_movement.x; //calculate direction
        if (grounded) { rigidbd.AddForce(move_direction.normalized * move_speed * 10f, ForceMode.Force); } //on ground
        else { rigidbd.AddForce(move_direction.normalized * move_speed * 10f * air_multiplier, ForceMode.Force); } //on air
    }

    private void speedControl()
    {
        Vector3 flat_velocity = new Vector3(rigidbd.velocity.x, 0f, rigidbd.velocity.z);

        if (flat_velocity.magnitude > move_speed) //limit velocity if needed
        {
            Vector3 limited_velocity = flat_velocity.normalized * move_speed;
            rigidbd.velocity = new Vector3(limited_velocity.x, rigidbd.velocity.y, limited_velocity.z);
        }
    }

    private void Jump()
    {
        rigidbd.velocity = new Vector3(rigidbd.velocity.x, 0f, rigidbd.velocity.z); //reset y velocity
        rigidbd.AddForce(transform.up * jump_force, ForceMode.Impulse);
    }

    private void resetJump() { ready_to_jump = true; }

    public void inputJump(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            if (ready_to_jump && grounded)
            {
                ready_to_jump = false;
                Jump();
                Invoke(nameof(resetJump), jump_cooldown);
            }
            is_jumping = true;
        }

        if (context.canceled) //key released
        {
            is_jumping = false;
        }
    }

    private void OnDestroy() { scr_guard.onGuardSpottedPlayer -= disableInput; }

    private void OnTriggerEnter(Collider hit_collider)
    {
        if (hit_collider.tag == "Finish")
        {
            disableInput();
            if (onFinishedLevel != null) { onFinishedLevel(); }
        }
        if (hit_collider.tag == "trigger_void")
        {
            DevCon.Commands.SceneCommands.RestartScene();
        }
    }
}