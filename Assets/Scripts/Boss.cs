using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public Animator animator;
	static Boss inst;

	public bool Invulnerable { get; set; } = false;
	public bool Moving { get; set; } = false;
	public static Boss Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
	}
	public void Rush()
	{
		animator.SetTrigger("Rush");
	}
	public void Pant()
	{
		animator.SetTrigger("Pant");
	}
	public void Smash()
	{
		animator.SetTrigger("Smash");
	}
	public void Spell()
	{
		animator.SetTrigger("Spell");
	}
	public void Knockout()
	{
		animator.SetTrigger("Knockout");
	}
	public void Hit()
	{
		animator.SetTrigger("Hit");
	}
	public void Roar()
	{
		animator.SetTrigger("Roar");
	}
	public void Idle()
	{
		animator.SetTrigger("Idle");
	}
	float cooldown = 0.0f;
	IEnumerator TempUnbreakable()
	{
		CarCore.Inst.EnableUnbreakable();
		yield return new WaitForSeconds(0.5f);
		CarCore.Inst.DisableUnbreakable();
	}
	bool second_stage = false;
	public Coroutine BossStartCoroutine { get; set; }
	IEnumerator SecondStageCutscene()
	{
		if (BossStartCoroutine != null)
		{
			StopCoroutine(BossStartCoroutine);
		}
		MainCamera.Inst.Stop();
		MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CaveCamera3), 1.0f);
		Invulnerable = true;
		PlayCanvas.Inst.Hide();
		LineCanvas.Bottom.DisplayLineAsync("Groundhog the Juggernaut", "You these filthy little piggies! I'm going to use my full strength!", 3.0f, Util.VoiceLine.None);
		Roar();
		yield return new WaitForSeconds(0.5f);
		AudioPlayer.Inst.PlaySie();
		yield return new WaitForSeconds(1.0f);
		Smash();
		yield return new WaitForSeconds(0.5f);
		Roar();
		yield return new WaitForSeconds(1.5f);
		Invulnerable = false;
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		second_stage_coroutine = StartCoroutine(SecondStage());
		PlayCanvas.Inst.Show();
	}
	Coroutine second_stage_coroutine;
	public MeshRenderer square_mesh;
	public Transform square_pivot;
	IEnumerator DoRush(bool first_time)
	{
		Vector3 start_position = transform.position;
		Invulnerable = true;
		
		Rush();
		AudioPlayer.Inst.PlaySie();
		Vector3 dir = CarCore.Inst.transform.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(dir, new Vector3(0, 1, 0));
		Vector3 euler_angles = rotation.eulerAngles;
		euler_angles.x = 0;
		euler_angles.z = 0;
		rotation = Quaternion.Euler(euler_angles);
		transform.rotation = rotation;
		square_pivot.transform.rotation = rotation;
		
		float duration = 3.0f;
		Vector3 end_position = start_position + rotation * Vector3.forward * 62;
		square_mesh.enabled = true;
		float start_time = Time.time;
		if (first_time)
		{
			duration = 3.0f;
		}
		else
		{
			duration = 0.5f;
		}
		while (Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			Color color = square_mesh.material.color;
			color.a = Mathf.Lerp(0.0f, 1.0f, t);
			square_mesh.material.color = color;
			yield return null;
		}
		duration = 1.5f;
		start_time = Time.time;
		player_protection = false;
		while (Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			Vector3 pos = Vector3.Lerp(start_position, end_position, t);
			transform.position = pos;
			yield return null;
		}
		transform.position = end_position;
		square_mesh.enabled = false;
		player_protection = true;
		// Knockout();
		//Invulnerable = false;
		
		//yield return new WaitForSeconds(5.0f);
		//Invulnerable = true;
		duration = 0.5f;
		Quaternion start_rotation = transform.rotation;
		Quaternion end_rotation = Quaternion.Euler(0, 180, 0) * start_rotation;
		start_time = Time.time;
		while (Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			transform.rotation = Quaternion.Slerp(start_rotation, end_rotation, t);
			yield return null;
		}
		Vector3 end_pos = start_position;
		start_position = transform.position;
		Invulnerable = true;
		Rush();
		duration = 1.5f;
		start_time = Time.time;
		while (Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			Vector3 pos = Vector3.Lerp(start_position, end_pos, t);
			transform.position = pos;
			yield return null;
		}
	}
	bool player_protection = true;
	IEnumerator SecondStage()
	{
		Idle();
		yield return new WaitForSeconds(2.0f);
		while (true)
		{
			yield return DoRush(true);
			yield return DoRush(false);
			yield return DoRush(false);
			yield return Rest();
		}
	}
	public void Death()
	{
		animator.SetTrigger("Death");
	}
	IEnumerator DeathAnim()
	{
		AudioPlayer.Inst.YajuLong();
		PlayCanvas.Inst.Hide();
		MainCamera.Inst.Stop();
		MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CaveCamera3), 1.0f);
		Death();
		yield return new WaitForSeconds(6.0f);
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		Vector3 position = transform.position;
		position.y += 1.0f;
		Instantiate(rockets_prefab, position, Quaternion.identity);
		PlayCanvas.Inst.Show();
		PlayCanvas.Inst.AutoFill = false;
		GameState.Inst.StartCoroutine(GameState.Inst.HandleEnding());
		Destroy(gameObject);
	}
	public GameObject rockets_prefab;
	private void OnCollisionEnter(Collision other)
	{
		if (player_protection)
			GameState.Inst.StartCoroutine(TempUnbreakable());
		if (!Invulnerable && cooldown <=0.0f)
		{
			HealthBar.Inst.Health -= CarCore.Inst.Velocity1 * 0.005f;
			if (HealthBar.Inst.Health <= 0.0f)
			{
				if (second_stage_coroutine != null)
				{
					StopCoroutine(second_stage_coroutine);
				}
				StartCoroutine(DeathAnim());
			}
			else if (HealthBar.Inst.Health < 0.5f && !second_stage)
			{
				second_stage = true;
				StartCoroutine(SecondStageCutscene());
			}
			else
			{
				AudioPlayer.Inst.YajuShort();
			}
			cooldown = 2.0f;
			Hit();
		}
	}
	public Transform cylinder;
	public MeshRenderer mesh_renderer;
	public SphereCollider sphere_collider;
	public GameObject rain_prefab;
	IEnumerator DoSmash()
	{
		Invulnerable = true;
		animator.speed = 0.01f;
		Smash();
		float duration1 = 1.0f;
		float duration2 = 3.0f;
		float start_time = Time.time;	
		LineCanvas.Bottom.DisplayLineAsync("Shirley", "The boss is going to smash us! Stay away!", 2.0f, Util.VoiceLine.the_boss_is);
		while (Time.time - start_time < duration1)
		{
			Color color = mesh_renderer.material.color;
			color.a = Mathf.Lerp(0.0f, 0.2f, (Time.time - start_time) / duration1);
			mesh_renderer.material.color = color;
			cylinder.localScale = Vector3.one * Mathf.Lerp(0.001f, 1.0f, (Time.time - start_time) / duration1);
			yield return null;
		}
		start_time = Time.time;
		while( Time.time - start_time < duration2)
		{
			Color color = mesh_renderer.material.color;
			color.a = Mathf.Lerp(0.2f, 0.5f, (Time.time - start_time) / duration1);
			mesh_renderer.material.color = color;
			yield return null;
		}
		animator.speed = 1.0f;
		start_time = Time.time;
		float duration_3 = 0.5f;
		float max_radius = 24.0f;
		while (Time.time - start_time < duration_3)
		{
			sphere_collider.radius = Mathf.Lerp(0.0f, max_radius, (Time.time - start_time) / duration_3);
			yield return null;
		}
		MainCamera.Inst.Shake(0.5f, 1.0f);
		AudioPlayer.Inst.Explode();
		sphere_collider.radius = max_radius;
		yield return null;
		sphere_collider.radius = 0.01f;
		cylinder.localScale = Vector3.one * 0.001f;
		Idle();
		Invulnerable = false;
	}
	IEnumerator DoSpell()
	{
		Invulnerable = true;
		Spell();
		LineCanvas.Bottom.DisplayLineAsync("Shirley", "Watch out the TNTs!", 2.0f, Util.VoiceLine.watch_out);
		yield return new WaitForSeconds(1.0f);
		for(int i = 0;i < 5;i++)
		{
			Vector3 target_pos = CarCore.Inst.transform.position;
			target_pos.y = -55.82f;
			Instantiate(rain_prefab, target_pos, Quaternion.identity);
			yield return new WaitForSeconds(1.0f);
		}
		Invulnerable = false;
		Idle();
	}
	IEnumerator Rest()
	{
		LineCanvas.Bottom.DisplayLineAsync("Shirley", "The boss is tired! Now is our chance!", 2.0f, Util.VoiceLine.boss_tired);
		Invulnerable = false;
		Pant();
		yield return new WaitForSeconds(10.0f);
	}
	public IEnumerator BossStart()
	{
		PlayCanvas.Inst.AutoFill = true;
		Idle();
		yield return new WaitForSeconds(0.5f);
		while(true)
		{
			yield return DoSmash();
			Pant();
			yield return new WaitForSeconds(Random.Range(4.0f, 8.0f));
			yield return DoSpell();
			yield return Rest();
		}
	}
	private void Update()
	{
		if (cooldown > 0.0f)
		{
			cooldown -= Time.deltaTime;
		}
	}
}