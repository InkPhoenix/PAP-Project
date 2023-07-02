using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_guard : MonoBehaviour
{
    public static event System.Action onGuardSpottedPlayer;

    public Transform path_holder; //Gizmo visualization

    private float speed = 5;
    private float wait_time = 0.3f;
    private float turn_speed = 90;
    private float time_spot;

    public Light spotlight_01;
    public Light spotlight_02;
    private float view_distance_01 = 7.5f;
    private float view_distance_02 = 11.5f;
    private float view_angle_01;
    private float view_angle_02;
    private float player_visible_timer;
    public LayerMask view_mask;

    private Transform player;
    public bool has_path;

    private void OnDrawGizmos()
    {
        //Patrol path visualization
        Vector3 start_pos = path_holder.GetChild(0).position;
        Vector3 previous_pos = start_pos;

        foreach (Transform waypoint in path_holder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previous_pos, waypoint.position);
            previous_pos = waypoint.position;
        }
        Gizmos.DrawLine(previous_pos, start_pos);

        //Vision visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y+0.2f, transform.position.z), transform.right * view_distance_01);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y-0.2f, transform.position.z), transform.right * view_distance_02);
    }

    private void Start()
    {
        switch (globalVars.difficulty)
        {
            case "easy":    time_spot = 4.0f; break;
            case "normal":  time_spot = 3.0f; break;
            case "hard":    time_spot = 2.0f; break;
            case "extreme": time_spot = 1.0f; break;

            default: time_spot = 3.0f; break;
        }

        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.Find("test_player").transform;
        //Debug.Log($"player.name = {player.name}");
        view_angle_01 = spotlight_01.spotAngle;
        view_angle_02 = spotlight_02.spotAngle;

        if (has_path)
        {
            Vector3[] waypoints = new Vector3[path_holder.childCount];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = path_holder.GetChild(i).position;
                waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z); //correct the guard pos
            }
            StartCoroutine(followPath(waypoints));
        }
    }

    IEnumerator followPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int target_waypoint_index = 1;
        Vector3 target_waypoint = waypoints[target_waypoint_index];
        //Debug.Log($"target_waypoint = {target_waypoint}");
        transform.LookAt(target_waypoint/100); //this single line makes me wanna commit arson to my computer

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target_waypoint, speed * Time.deltaTime);
            if (transform.position == target_waypoint)
            {
                target_waypoint_index = (target_waypoint_index+1) % waypoints.Length;
                target_waypoint = waypoints[target_waypoint_index];
                yield return new WaitForSeconds(wait_time);
                yield return StartCoroutine(turnGuard(target_waypoint));
            }
            yield return null;
        }
    }

    IEnumerator turnGuard(Vector3 look_target)
    {
        Vector3 direction_look_target = (look_target - transform.position).normalized;
        float target_angle = 0 - Mathf.Atan2(direction_look_target.z, direction_look_target.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, target_angle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, target_angle, turn_speed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    bool canSeePlayer_1()
    {
        if (Vector3.Distance(transform.position, player.position) < view_distance_01)
        {
            Vector3 direction_to_player = (player.position - transform.position).normalized;
            float angle_guard_n_player = Vector3.Angle(transform.right, direction_to_player);
            if (angle_guard_n_player < view_angle_01 / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, view_mask)) { return true; }
            }
        }
        return false;
    }

    bool canSeePlayer_2()
    {
        if (Vector3.Distance(transform.position, player.position) < view_distance_02)
        {
            Vector3 direction_to_player = (player.position - transform.position).normalized;
            float angle_guard_n_player = Vector3.Angle(transform.right, direction_to_player);
            if (angle_guard_n_player < view_angle_02 / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, view_mask)) { return true; }
            }
        }
        return false;
    }

    private void Update()
    {
        //primary view cone
        if (canSeePlayer_1()) { player_visible_timer = time_spot; }

        //secundary view cone
        if (canSeePlayer_2()) { player_visible_timer += Time.deltaTime; }
        else { player_visible_timer -= Time.deltaTime; }
        player_visible_timer = Mathf.Clamp(player_visible_timer, 0, time_spot);
        spotlight_01.color = Color.Lerp(Color.yellow, Color.green, player_visible_timer / time_spot);
        spotlight_02.color = Color.Lerp(Color.red, Color.green, player_visible_timer / time_spot);
        if (player_visible_timer >= time_spot)
        {
            spotlight_01.color = Color.green;
            if (onGuardSpottedPlayer != null) { onGuardSpottedPlayer(); }
        }
        //Debug.Log($"player_visible_timer = {player_visible_timer}");
    }
}
