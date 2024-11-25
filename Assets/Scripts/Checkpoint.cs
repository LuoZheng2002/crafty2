using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointReachedEvent
{
	public Util.WaypointName waypoint_name;
    public CheckpointReachedEvent(Util. WaypointName waypoint_name)
    {
        this.waypoint_name = waypoint_name;
	}
}
public class DeactivateAllCheckpointEvent
{

}

public class Checkpoint : MonoBehaviour
{
	public bool permanent = false;
	public Util.WaypointName waypoint_name;
    static Dictionary<Util.WaypointName, Checkpoint> checkpoints = new();
	public static Dictionary<Util.WaypointName, Checkpoint> Checkpoints =>checkpoints;
	public GameObject red_mesh;
	public GameObject green_mesh;
	public GameObject checkpoint_goal;
	public static Checkpoint Get(Util.WaypointName waypoint_name)
    {
        Debug.Assert(checkpoints.ContainsKey(waypoint_name), $"checkpoint {waypoint_name} not set");
		return checkpoints[waypoint_name];
	}
	private void Start()
	{
		Debug.Assert(!checkpoints.ContainsKey(waypoint_name));
		checkpoints[waypoint_name] = this;
		EventBus.Subscribe<DeactivateAllCheckpointEvent>(OnOtherCheckpointReached);
	}
	void OnOtherCheckpointReached(DeactivateAllCheckpointEvent e)
	{
		red_mesh.SetActive(false);
		green_mesh.SetActive(false);
		DeactivateColliderHelper();
	}
	void DeactivateColliderHelper()
	{
		// Debug.Log($"{waypoint_name} collider deactivated!");
		checkpoint_goal.SetActive(false);
	}
	private void OnDestroy()
	{
		checkpoints.Clear();
	}
	public void Activate()
	{
		Debug.Log($"{waypoint_name} activated!");
		red_mesh.SetActive(true);
		green_mesh.SetActive(false);
		checkpoint_goal.SetActive(true);
	}
	public ParticleSystem particle_system;
	public void OnCheckpointGoalReached()
	{
		EventBus.Publish(new CheckpointReachedEvent(waypoint_name));
		EventBus.Publish(new DeactivateAllCheckpointEvent());
		red_mesh.SetActive(false);
		green_mesh.SetActive(true);
		GameSave.CurrentCheckpoint = waypoint_name;
        if (permanent)
        {
			MapCanvas.Inst.PermWaypoints.Add(waypoint_name);
        }
		particle_system.Play();
    }
	// on checkpoint goal reached

}
