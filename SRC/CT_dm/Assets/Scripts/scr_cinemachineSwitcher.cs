using UnityEngine;
using UnityEngine.InputSystem;

public class scr_cinemachineSwitcher : MonoBehaviour
{
    private Animator animator;
    private bool stPersonCam = false;
    public GameObject player_mesh;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SwitchCamera(InputAction.CallbackContext context)
    {
        if (context.started) //key pressed
        {
            player_mesh.SetActive(false);
            animator.Play("CM_1stP_Cam");
            Debug.Log("1st person cam activated");
        }
        if (context.canceled) //key released
        {
            player_mesh.SetActive(true);
            animator.Play("CM_3rdP_Cam");
            Debug.Log("3rd person cam activated");
        }
    }
}
