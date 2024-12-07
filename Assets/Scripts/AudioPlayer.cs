using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip buildClip;
    public AudioClip playClip;
    public AudioClip clearClip;
    public AudioClip menuClip;
    public AudioClip snore;
    public AudioClip scream;
    public AudioClip wilhelm;
    public AudioClip motor;
    public AudioClip rocket;
    public AudioClip explosion;
    public AudioClip warning;
    public AudioClip crash;
	public AudioClip title;
	public AudioClip free_build;
	public AudioClip free_play;
	public AudioClip boss;
	public AudioClip yaju_short;
	public AudioClip yaju_long;
	public AudioClip sie;
    AudioSource musicSource;
    AudioSource soundEffectSource;
	AudioSource bossSource;
    public float min_snore_interval = 10.0f;
    public float max_snore_interval = 30.0f;
    bool can_snore = false;
    // Start is called before the first frame update
    static AudioPlayer inst;
    public static AudioPlayer Inst
    {
        get { Debug.Assert(inst != null, "Audio player not set");return inst; }
    }
    public bool is_transition3 = false;
    public bool is_transition1 = false;
    public bool is_transition2 = false;
    void Start()
    {
        Debug.Assert(inst == null, "Audio Player already instantiated");
        inst = this;
        var audioSources = GetComponents<AudioSource>();
        musicSource = audioSources[0];
        soundEffectSource = audioSources[1];
		bossSource = audioSources[2];
        musicSource.volume = 0.5f;
        soundEffectSource.volume = 0.2f;
        EventBus.Subscribe<ScreamEvent>(Scream);
        // StartCoroutine(Snore());
        if (is_transition3)
        {
            StartCoroutine(DelayScream());
		}
        if (is_transition1)
        {
            StartCoroutine(DelayRocket());
        }
        if (is_transition2)
        {
            StartCoroutine(DelayWarning());
		}
    }
	public void PlaySie()
	{
		bossSource.volume = 1.0f;
		if (bossSource.isPlaying)
		{
			bossSource.Stop();
		}
		bossSource.clip = sie;
		bossSource.loop = false;
		bossSource.Play();
	}
    public void PlayCrash()
    {
		soundEffectSource.volume = 1.0f;
		if (soundEffectSource.isPlaying)
		{
			soundEffectSource.Stop();
		}
		soundEffectSource.clip = crash;
		soundEffectSource.loop = false;
		soundEffectSource.Play();
	}
    IEnumerator DelayScream()
    {
        yield return new WaitForSeconds(0.2f);
		Scream(null);
	}
	IEnumerator DelayRocket()
	{
		yield return new WaitForSeconds(0.4f);
        RocketStart();
	}
	IEnumerator DelayWarning()
	{
		yield return new WaitForSeconds(0.4f);
		Warning();
	}
	private void OnDestroy()
	{
        inst = null;
	}
	void Scream(ScreamEvent e)
    {
        if (soundEffectSource.isPlaying)
        {
            soundEffectSource.Stop();
        }
        soundEffectSource.clip = scream;
        soundEffectSource.loop = false;
        soundEffectSource.Play();
    }


    public void Wilhelm()
    {
		if (soundEffectSource.isPlaying)
		{
			soundEffectSource.Stop();
		}
		soundEffectSource.clip = wilhelm;
		soundEffectSource.loop = false;
		soundEffectSource.Play();
	}

	public void YajuLong()
	{
		if (bossSource.isPlaying)
		{
			bossSource.Stop();
		}
		bossSource.clip = yaju_long;
		bossSource.loop = false;
		bossSource.Play();
	}
	public void YajuShort()
	{
		if (bossSource.isPlaying)
		{
			bossSource.Stop();
		}
		bossSource.clip = yaju_short;
		bossSource.loop = false;
		bossSource.Play();
	}
	public void Explode()
	{
		if (bossSource.isPlaying)
		{
			bossSource.Stop();
		}
        bossSource.volume = 1.0f;
		bossSource.clip = explosion;
		bossSource.loop = false;
		bossSource.Play();
	}
	public void Warning()
	{
		if (soundEffectSource.isPlaying)
		{
			soundEffectSource.Stop();
		}
		soundEffectSource.clip = warning;
		soundEffectSource.loop = false;
		soundEffectSource.Play();
	}
	public void MotorStart()
    {
		if (soundEffectSource.isPlaying)
		{
			soundEffectSource.Stop();
		}
		soundEffectSource.clip = motor;
        soundEffectSource.volume = 0.5f;
		soundEffectSource.loop = true;
		soundEffectSource.Play();
	}
    public void RocketStart()
    {
		if (soundEffectSource.isPlaying)
		{
			soundEffectSource.Stop();
		}
		soundEffectSource.clip = rocket;
        soundEffectSource.volume = 0.5f;
		soundEffectSource.loop = false;
		soundEffectSource.Play();
	}
    public void StopSoundEffect()
    {
		if (soundEffectSource.clip != yaju_long)
		{
			soundEffectSource.Stop();
			soundEffectSource.volume = 0.2f;
		}
    }
    IEnumerator Snore()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(min_snore_interval, max_snore_interval));
            if (!can_snore)
            {
                continue;
            }
            if (!soundEffectSource.isPlaying)
            {
                soundEffectSource.clip = snore;
                soundEffectSource.loop = false;
                soundEffectSource.Play();
            }
        }
    }
    public void TransitionToIntro()
    {
		can_snore = false;
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = menuClip;
		musicSource.loop = true;
		musicSource.Play();
	}
	public void TransitionToTitle()
	{
		can_snore = false;
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = title;
		musicSource.loop = false;
		musicSource.Play();
	}
	public void TransitionToStory()
    {
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = buildClip;
		musicSource.loop = true;
		musicSource.Play();
	}
	bool flag = false;
    public void TransitionToPlay()
    {
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		flag = !flag;
		if (flag)
		{
			musicSource.clip = free_play;
		}
		else
		{
			musicSource.clip = playClip;
		}
		musicSource.loop = true;
		musicSource.Play();
	}
	public void TransitionToBuild()
	{
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = free_build;
		musicSource.loop = true;
		musicSource.Play();
	}
	public void PlayCheckpoint()
    {
		if (bossSource.isPlaying)
		{
			bossSource.Stop();
		}
		bossSource.clip = clearClip;
		bossSource.loop = false;
		bossSource.Play();
	}
}
