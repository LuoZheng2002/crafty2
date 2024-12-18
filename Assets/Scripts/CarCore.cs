using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarCore : MonoBehaviour
{
	static CarCore inst;
	public static CarCore Inst { get { Debug.Assert(inst != null); return inst; } }
	public Rigidbody RB { get; private set; }
	public FixedJoint joint;
	public FixedJoint fix_joint;
	[SerializeField]
	private Transform camera_pivot;
	[SerializeField]
	private Transform camera_end;
	[SerializeField]
	Transform container;

	public Transform CameraEnd => camera_end;
	public Transform Container => container;
	public bool AutoRotation { get; set; } = false;

	float pivot_distance = 5.0f;
	public float min_dist = 3.0f;
	public float max_dist = 7.0f;
	public float Velocity1 { get; set; }
	public float Velocity2 { get; set; }
	public float Velocity3 { get; set; }
	float PivotDistance
	{
		get { return pivot_distance; }
		set
		{
			pivot_distance = value;
			Vector3 position = CameraEnd.localPosition;
			position.z = -pivot_distance;
			CameraEnd.localPosition = position;
		}
	}
	public float zoom_speed = 0.25f;
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		RB = GetComponent<Rigidbody>();
		EventBus.Subscribe<CanvasDragEvent>(OnCanvasDrag);
		// joint = transform.AddComponent<FixedJoint>();
		EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
	}
	void OnGameStateChanged(GameStateChangedEvent e)
	{
		if (e.is_play)
		{
			camera_pivot.transform.localPosition = new Vector3(0, VerticalOffset, 0);
		}
		else
		{
			camera_pivot.transform.localPosition = new Vector3(0, 0, 0);
		}
	}
	private void OnDestroy()
	{
		inst = null;
	}
	float cooldown = 0.0f;
	private void Update()
	{
		Velocity1 = Velocity2;
		Velocity2 = Velocity3;
		Velocity3 = RB.velocity.magnitude;
		if (cooldown > 0.0f)
		{
			cooldown -= Time.deltaTime;
		}
		if (Input.mouseScrollDelta.y != 0)
		{
			PivotDistance = Mathf.Clamp(PivotDistance - Input.mouseScrollDelta.y * zoom_speed, min_dist, max_dist);
		}
		Vector3 euler_angles = transform.eulerAngles;
		euler_angles.x = 0;
		euler_angles.z = 0;
		if (AutoRotation)
		{
			camera_pivot.rotation = Quaternion.Euler(euler_angles) * Quaternion.Euler(drag_euler_angle);
		}
		else
		{
			camera_pivot.rotation = Quaternion.Euler(drag_euler_angle);
		}
	}
	//public void AttachPiggy()
	//{
	//	Debug.Assert(PiggyPreview.Inst != null);
	//	joint = transform.AddComponent<FixedJoint>();
	//	joint.connectedBody = PiggyPreview.Inst.RB;
	//}
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Trigger entered1 !");
		if (cooldown > 0.0f)
		{
			return;
		}
		Debug.Log("Trigger entered2 !");
		cooldown = 5.0f;
		if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
		{
			AddRandomImpulse();
		}
	}
	public void AddRandomImpulse()
	{
		foreach (Transform child in container)
		{
			VehicleComponent vehicle_component = child.GetComponent<VehicleComponent>();
			vehicle_component.RB.AddForce(Random.insideUnitSphere * 3000.0f, ForceMode.Impulse);
		}
	}
	public void Fix()
	{
		Debug.Assert(fix_joint == null);
		if (fix_joint != null)
		{
			Destroy(fix_joint);
		}
		fix_joint = null;
		fix_joint = transform.AddComponent<FixedJoint>();
	}
	public void Unfix()
	{
		Debug.Assert(fix_joint != null);
		if (fix_joint != null)
		{
			Destroy(fix_joint);
		}
		fix_joint = null;
	}
	bool container_activated = true;
	public void ActivateContainer()
	{
		Debug.Assert(!container_activated);
		container_activated = true;
		container.gameObject.SetActive(true);
		BindToPig();
	}
	void BindToPig()
	{
		Debug.Assert(joint == null);
		joint = transform.AddComponent<FixedJoint>();
		joint.connectedBody = PiggyPreview.Inst.RB;
	}
	public void UnbindPig()
	{
		Debug.Assert(joint != null);
		Destroy(joint);
		joint = null;
	}
	public void Move()
	{
		RB.velocity = new Vector3(0.5f, 0.0f, 0.0f);
	}
	public void DestroyComponents()
	{
		UnbindPig();
		foreach (Transform child in container)
		{
			Debug.Assert(child.GetComponent<VehicleComponent>() != null);
			Destroy(child.gameObject);
		}
	}
	public IEnumerator Build()
	{
		Debug.Assert(container.childCount == 0);
		yield return AlignToGridMatrix();
		BindToPig();
		Debug.Assert(container_activated);
	}
	bool HasComponent()
	{
		foreach (Transform child in container)
		{
			if (child.GetComponent<VehicleComponent>() != null)
			{
				return true;
			}
		}
		return false;
	}
	public void DeactivateContainer()
	{
		Debug.Assert(container_activated);
		container_activated = false;
		container.gameObject.SetActive(false);
		UnbindPig();
	}
	public void DampStart()
	{
		foreach(Transform child in container)
		{
			VehicleComponent vehicle_component = child.GetComponent<VehicleComponent>();
			vehicle_component.DampStart();
		}
	}
	public void DampStop()
	{
		foreach (Transform child in container)
		{
			VehicleComponent vehicle_component = child.GetComponent<VehicleComponent>();
			vehicle_component.DampStop();
		}
	}
	public void EnableUnbreakable()
	{
		foreach (Transform child in container)
		{
			VehicleComponent vehicleComponent = child.GetComponent<VehicleComponent>();
			vehicleComponent.EnableUnbreakable();
		}
	}
	public void DisableUnbreakable()
	{
		foreach (Transform child in container)
		{
			VehicleComponent vehicleComponent = child.GetComponent<VehicleComponent>();
			vehicleComponent.DisableUnbreakable();
		}
	}
	public void AlignToGridMatrixAsync()
	{
		RB.MovePosition(GridMatrix.Inst.transform.position + GridMatrix.Inst.transform.rotation* GridMatrix.Inst.ProbeTargetPos);
		RB.MoveRotation(GridMatrix.Inst.transform.rotation);
	}
	public float VerticalOffset { get; set; } = 0.0f;
	public IEnumerator AlignToGridMatrix()
	{
		Vector3 target_pos = GridMatrix.Inst.transform.position + GridMatrix.Inst.transform.rotation * GridMatrix.Inst.ProbeTargetPos;
		Quaternion target_rotation = GridMatrix.Inst.transform.rotation;
		// Debug.Log($"Target pos: {target_pos}");
		// Debug.Log($"Target rotation: {target_rotation}");
		RB.MovePosition(target_pos);
		RB.MoveRotation(target_rotation);
		while((transform.position - target_pos).magnitude > 0.01f || Quaternion.Angle(transform.rotation, target_rotation)>1.0f)
		{
			// Debug.Log("Yield return!");
			RB.MovePosition(target_pos);
			RB.MoveRotation(target_rotation);
			yield return null;
		}
		// Debug.Log("Test completed!");
		// Debug.Log($"rb position: {rb.position}, rb rotation: {rb.rotation}");
		// Debug.Log($"position: {transform.position}, rotation: {transform.rotation}");
	}
	public float drag_rotation_speed = 1.0f;
	Vector3 drag_euler_angle = new Vector3(0, 0, 0);
	void OnCanvasDrag(CanvasDragEvent e)
	{
		float rotationX = -e.deltaY * drag_rotation_speed;  // Vertical rotation
		float rotationY = e.deltaX * drag_rotation_speed;  // Horizontal rotation											   // Rotate the camera accordingly
		drag_euler_angle += new Vector3(rotationX, rotationY, 0);
		camera_pivot.rotation = Quaternion.Euler(drag_euler_angle);
	}
	public void ResetPivot()
	{
		Vector3 temp_rotation = transform.rotation.eulerAngles;
		temp_rotation.z = 0;
		Quaternion reset_rotation = Quaternion.Euler(temp_rotation) * Quaternion.Euler(10, -90, 0);
		drag_euler_angle = reset_rotation.eulerAngles;
		camera_pivot.rotation = Quaternion.Euler(drag_euler_angle);
	}
}
