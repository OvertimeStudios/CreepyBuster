using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour 
{
	#region enum
	public enum Musics
	{
		None,
		MainMenuTheme,
		GameTheme,
	}

	public enum SoundFX
	{
		None,
	}

	#endregion

	private static Musics currentMusic;

	#region Music Audioclips
	public AudioClip mainMenuTheme;
	public AudioClip gameTheme;
	#endregion

	#region SoundFX Audioclips

	#endregion

	#region singleton
	private static SoundController instance;
	public static SoundController Instance
	{
		get { return instance; }
	}
	#endregion

	private AudioSource audioSourceMusic;

	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
		}

		DontDestroyOnLoad (gameObject);

		audioSourceMusic = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () 
	{

	}

	public void PlayMusic(Musics music)
	{
		PlayMusic (music, true);
	}

	public void PlayMusic(Musics music, bool loop)
	{
		if(music == currentMusic) return;

		AudioClip clip = null;
		
		if(music == Musics.MainMenuTheme)
			clip = mainMenuTheme;
		else if(music == Musics.GameTheme)
			clip = gameTheme;

		audioSourceMusic.loop = loop;
		audioSourceMusic.clip = clip;
		audioSourceMusic.Stop ();
		audioSourceMusic.Play ();
		
		currentMusic = music;
	}

	public void MuteMusic()
	{
		audioSourceMusic.volume = 0;
	}

	public void UnmuteMusic()
	{
		audioSourceMusic.volume = 1;
	}
}
