using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class scr_cinemachineSwitcher : MonoBehaviour
{
    public GameObject player_mesh;
    public GameObject player_orientation;
    public CinemachineVirtualCamera cam_1st_person;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Debug.Log($"player_orientation.y = {player_orientation.transform.eulerAngles.y}");
        //Debug.Log($"cam_axis_vertical = {cam_1st_person.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value} | cam_axis_vertical = {cam_1st_person.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value}");
    }

    public void SwitchCamera(InputAction.CallbackContext context)
    {
        if (context.started) //key pressed
        {
            cam_1st_person.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;
            //cam_1st_person.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = player_orientation.transform.eulerAngles.y;
            cam_1st_person.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = player_mesh.transform.eulerAngles.y;

            player_mesh.SetActive(false);
            animator.Play("CM_1stP_Cam");
            Debug.Log("1st person cam activated");
        }
        if (context.canceled) //key released
        {
            player_mesh.transform.eulerAngles = new Vector3 (0, cam_1st_person.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value, 0);
            player_mesh.SetActive(true);
            animator.Play("CM_3rdP_Cam");
            Debug.Log("3rd person cam activated");
        }
    }
}
