using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr_player_controller : MonoBehaviour
{
    public GameObject player_cam;

    public Transform orientation;
    public Transform player;
    public Transform obj_player;
    public Rigidbody rigidbd;

    private Vector2 input_movement;

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
    private float jump_force = 7f;
    private float jump_cooldown = 0.25f;
    private float air_multiplier = 0.4f;
    private bool ready_to_jump;
    private bool is_jumping;

    private void Start()
    {
        rigidbd = GetComponent<Rigidbody>();
        rigidbd.freezeRotation = true;
        ready_to_jump = true;
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
        //Debug.Log($"player view_direction: {view_direction}");
        if (view_direction != Vector3.zero) { orientation.forward = view_direction.normalized; } //if check needed to get rid of error message
        //Debug.Log($"player orientation.forward: {orientation.forward}");
        //Debug.Log($"player orientation.transform.rotation: {orientation.transform.eulerAngles}");

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
        movePlayer();
    }

    public void inputUpdateMovement(InputAction.CallbackContext context)
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


















    /*public GameObject obj_player;
    public GameObject obj_player_cam;

    private CharacterController controller;

    private Vector3 movement;
    private Vector3 modified_movement;
    private bool is_movement_pressed;
    private float player_speed = 4.0f;
    private float rotation_factor_per_frame = 10f; //how fast the player rotates

    private float grounded_gravity = -0.05f;          //gravity when on the ground
    private float player_gravity = Physics.gravity.y; //gravity when off the ground
    private float terminal_velocity = -15.0f;

    private float max_jump_height = 2.0f;
    private float max_jump_time = 0.75f;
    private bool is_jumping = false;
    private float initial_jump_velocity;
    private int max_jumps = 2;
    private int jumps = 0;
    private float double_jump_multiplier = 0.8f; //force multiplier for jumps after the first one

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        //setup jump variables
        float time_to_apex = max_jump_time / 2;
        player_gravity = (-2 * max_jump_height) / Mathf.Pow(time_to_apex, 2);
        initial_jump_velocity = (2 * max_jump_height) / time_to_apex;
    }

    public void moving(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            movement.x = context.ReadValue<Vector2>().x * player_speed;
            movement.z = context.ReadValue<Vector2>().y * player_speed;
            //Debug.Log($"context = X: {context.ReadValue<Vector2>().x} | Y: {context.ReadValue<Vector2>().y}");
            //Debug.Log($"movement.x = {movement.x} | movement.z = {movement.z}");

            if (movement.x != 0 || movement.z != 0) { is_movement_pressed = true; }
            else { is_movement_pressed = false; }
        }

        if (context.canceled) //key released
        {
            movement.x = context.ReadValue<Vector2>().x;
            movement.z = context.ReadValue<Vector2>().y;

            if (movement.x != 0 || movement.z != 0) { is_movement_pressed = true; }
            else { is_movement_pressed = false; }
        }
    }

    private void playerRotation()
    {
        Vector3 pos_to_look_at;

        pos_to_look_at.x = movement.x;
        pos_to_look_at.y = 0f;
        pos_to_look_at.z = movement.z;

        Quaternion current_rotation = transform.rotation;

        if (is_movement_pressed)
        {
            Quaternion target_rotation = Quaternion.LookRotation(pos_to_look_at);
            transform.rotation = Quaternion.Slerp(current_rotation, target_rotation, rotation_factor_per_frame * Time.deltaTime);
        }
    }

    private void playerGravity()
    {
        bool is_falling = movement.y <= 0.0f || !is_jumping;
        float fall_multiplier = 2.0f;

        if (controller.isGrounded)
        {
            movement.y = grounded_gravity;
            modified_movement.y = grounded_gravity;
        }
        else if(is_falling)
        {
            float previous_Y_velocity = movement.y;
            movement.y = movement.y + (player_gravity * fall_multiplier * Time.deltaTime);
            modified_movement.y = Mathf.Max((previous_Y_velocity + movement.y) * 0.5f, terminal_velocity);
        }
        else
        {
            float previous_Y_velocity = movement.y;
            movement.y = movement.y + (player_gravity * Time.deltaTime);
            modified_movement.y = (previous_Y_velocity + movement.y) * 0.5f;
        }
    }

    public void jump(InputAction.CallbackContext context)
    {
        if (context.started) //key pressed
        {
            if (jumps != max_jumps)
            {
                jumps++;
                is_jumping = context.ReadValueAsButton();
                if (jumps > 1)
                {
                    movement.y = (initial_jump_velocity) * double_jump_multiplier;
                    modified_movement.y = (initial_jump_velocity) * double_jump_multiplier;
                }
                else
                {
                    movement.y = initial_jump_velocity;
                    modified_movement.y = initial_jump_velocity;
                }
            }
        }

        if (context.canceled) { is_jumping = context.ReadValueAsButton(); } //key released
    }

    void Update()
    {
        //Debug.Log($"is moving?: {is_movement_pressed} || movement: {movement} || grounded gravity {grounded_gravity} || is jumping? = {is_jumping} || jumps: {jumps} || controller.isGrounded: {controller.isGrounded}");
        playerRotation();

        modified_movement.x = movement.x;
        modified_movement.z = movement.z;
        controller.Move(modified_movement * Time.deltaTime);

        playerGravity();
        if (controller.isGrounded) { jumps = 0; }
        
        //Debug.Log($"obj_player_cam.rotation = {obj_player_cam.transform.rotation.eulerAngles.normalized.y}"); //DEBUG
    }*/
}