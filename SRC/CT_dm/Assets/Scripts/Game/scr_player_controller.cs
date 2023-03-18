using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
public class scr_player_controller : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float rotationSpeed = 4f;
    //public GameObject obj_player;

    //private PlayerInput playerInput;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool grounded_player;
    //private Transform cameraMainTransform;

    private Vector3 movement;
    private bool is_movement_pressed;
    private bool is_jumping;
    public float rotation_factor_per_frame = 10f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        //playerInput = GetComponent<PlayerInput>();
        //cameraMainTransform = Camera.main.transform;
    }

    public void moving(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            movement.x = context.ReadValue<Vector2>().x;
            //movement.y = Physics.gravity.y;
            movement.z = context.ReadValue<Vector2>().y;

            if (movement.x != 0 || movement.z != 0) { is_movement_pressed = true; }
            else { is_movement_pressed = false; }
        }

        if (context.canceled) //key released
        {
            movement.x = context.ReadValue<Vector2>().x;
            //movement.y = Physics.gravity.y;
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
        if (controller.isGrounded)
        {
            float grounded_gravity = -0.05f;
            movement.y = grounded_gravity;
        }
        else { movement.y = Physics.gravity.y; }
    }

    public void jump(InputAction.CallbackContext context)
    {
        if (context.performed) { is_jumping = true; }
        if (context.canceled) { is_jumping = false; }
    }

    void Update()
    {
        Debug.Log($"is moving?: {is_movement_pressed} || {movement} || playerVelocity.y = {playerVelocity.y} || is jumping? = {is_jumping} || is grounded?: {grounded_player}");
        playerRotation();
        playerGravity();

        //grounded_player = controller.isGrounded;
        //if (grounded_player && playerVelocity.y < 0) { playerVelocity.y = 0f; }

        //Jump
        //if (is_jumping == true)
        //{
        //    playerVelocity.y += Mathf.Sqrt(jumpHeight * -1.0f * Physics.gravity.y);
        //}
        //playerVelocity.y += Physics.gravity.y * Time.deltaTime;

        //controller.Move(playerVelocity * Time.deltaTime);

        controller.Move(movement * playerSpeed * Time.deltaTime);

        //angle player faces depending on the camera
        //if (movement != Vector3.zero)
        //{
        //    float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        //    Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        //}






        /*groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = playerInput.actions["Movement"].ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        //Jump
        if (playerInput.actions["Jump"].triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
        }

        /*playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }*/
        //Debug.Log($"playerVelocity & move: {playerVelocity} | {move}");
        //Debug.Log($"obj_player.transform.localPosition: {obj_player.transform.localPosition}");
    }
}