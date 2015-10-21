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
		BossTheme,
	}

	public enum SoundFX
	{
		None,
		EnemyDie,
		Pause,
		Resume,
		AttackLoop,
		AttackStart,
		OrbPP,
		OrbP,
		OrbM,
		OrbG,
		PowerUpCollected,
		Freeze,
		DeathRay,
		Score,
		Click,
		MenuIn,
		MenuOut,
		MenuScroll,
		ShopBuy,
		PlayerDamage,
		LevelUp,
		BossMeteorIdle,
		BossMeteorDamage,
		BossMeteorDrop,
		BossTwinsIdle,
		BossTwinsDamage,
		BossIllusionIdle,
		BossIllusionMultiply,
		BossIllusionDamage,
		BossDie,
	}

	#endregion

	private static Musics currentMusic;

	#region Music Audioclips
	[Header("Musics")]
	public AudioClip mainMenuTheme;
	public AudioClip gameTheme;
	public AudioClip bossTheme;
	#endregion

	#region Sound FX AudioClips
	[Header("Sound FX")]
	public AudioClip enemyDie;
	public AudioClip pause;
	public AudioClip resume;
	public AudioClip attackLoop;
	public AudioClip attackStart;
	public AudioClip orbPP;
	public AudioClip orbP;
	public AudioClip orbM;
	public AudioClip orbG;
	public AudioClip freeze;
	public AudioClip deathRay;
	public AudioClip powerUpCollect;
	public AudioClip score;
	public AudioClip playerDamage;
	public AudioClip click;
	public AudioClip menuIn;
	public AudioClip menuOut;
	public AudioClip menuScroll;
	public AudioClip shopBuy;
	public AudioClip levelUp;
	public AudioClip bossMeteorIdle;
	public AudioClip bossMeteorDamage;
	public AudioClip meteorDrop1;
	public AudioClip meteorDrop2;
	public AudioClip meteorDrop3;
	public AudioClip bossTwinsIdle;
	public AudioClip bossTwinsDamage;
	public AudioClip bossIllusionIdle;
	public AudioClip bossIllusionMultiply;
	public AudioClip bossIllusionDamage;
	public AudioClip bossDie;
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
	private AudioSource audioSourceMusic2;
	private AudioSource audioSourceMusic3;
	private AudioSource audioSourceSoundFX;

	private bool musicMute = false;
	private bool soundFXMute = false;

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

		AudioSource[] sources = GetComponents<AudioSource>();
		
		audioSourceMusic = sources[0];
		audioSourceMusic2 = sources[1];
		audioSourceMusic3 = sources[2];
		audioSourceSoundFX = sources[3];

		currentMusic = Musics.MainMenuTheme;
	}

	public void CrossFadeMusic(Musics music, float crossTime)
	{
		if(musicMute) return;

		StartCoroutine(DoCrossFade(music, crossTime));
	}

	private IEnumerator DoCrossFade(Musics music, float crossTime)
	{
		AudioSource sourceFrom = null;

		if(currentMusic == Musics.MainMenuTheme)
			sourceFrom = audioSourceMusic;
		else if(currentMusic == Musics.GameTheme)
			sourceFrom = audioSourceMusic2;
		else if(currentMusic == Musics.BossTheme)
			sourceFrom = audioSourceMusic3;

		AudioSource sourceTo = null;
		
		if(music == Musics.MainMenuTheme)
			sourceTo = audioSourceMusic;
		else if(music == Musics.GameTheme)
			sourceTo = audioSourceMusic2;
		else if(music == Musics.BossTheme)
		{
			sourceTo = audioSourceMusic3;
			sourceTo.Stop();
			sourceTo.Play();
		}

		sourceFrom.mute = false;
		sourceTo.mute = false;

		float maxTimeFrom = sourceFrom.volume;
		float maxTimeTo = sourceTo.volume;

		sourceTo.volume = 0;

		while(sourceFrom.volume > 0)
		{
			sourceFrom.volume -= maxTimeFrom * Time.deltaTime / crossTime;
			sourceTo.volume += maxTimeTo * Time.deltaTime / crossTime;

			yield return null;
		}

		sourceFrom.volume = maxTimeFrom;
		sourceTo.volume = maxTimeTo;

		sourceFrom.mute = true;

		currentMusic = music;
	}

	public void FadeOut(float fadeTime)
	{
		if(musicMute) return;

		StartCoroutine(DoFadeOut(fadeTime));
	}

	private IEnumerator DoFadeOut(float fadeTime)
	{
		AudioSource sourceFrom = null;
		
		if(currentMusic == Musics.MainMenuTheme)
			sourceFrom = audioSourceMusic;
		else if(currentMusic == Musics.GameTheme)
			sourceFrom = audioSourceMusic2;
		else if(currentMusic == Musics.BossTheme)
			sourceFrom = audioSourceMusic3;
		
		sourceFrom.mute = false;
		
		float maxTimeFrom = sourceFrom.volume;
		
		while(sourceFrom.volume > 0)
		{
			sourceFrom.volume -= maxTimeFrom * Time.deltaTime / fadeTime;
			
			yield return null;
		}
		
		sourceFrom.volume = maxTimeFrom;
		
		sourceFrom.mute = true;
		
		currentMusic = Musics.None;
	}

	public void PlayMusic(Musics music)
	{
		PlayMusic (music, true);
	}

	public void PlayMusic(Musics music, bool loop)
	{
		if(musicMute) return;

		if(music == currentMusic) return;

		audioSourceMusic.mute = true;
		audioSourceMusic2.mute = true;
		audioSourceMusic3.mute = true;

		AudioSource source = null;
		
		if(music == Musics.MainMenuTheme)
			source = audioSourceMusic;
		else if(music == Musics.GameTheme)
			source = audioSourceMusic2;
		else if(music == Musics.BossTheme)
		{
			source = audioSourceMusic3;
			source.Stop();
			source.Play();
		}

		source.loop = loop;
		source.mute = false;
		
		currentMusic = music;
	}

	public void PlaySoundFX(SoundFX sound)
	{
		if(soundFXMute) return;

		AudioClip s = GetSound(sound);

		audioSourceSoundFX.PlayOneShot(s);
	}

	public void PlaySoundFX(params SoundFX[] sounds)
	{
		if(soundFXMute) return;

		StartCoroutine(PlaySoundFXSimultaneouly(sounds));
	}

	private IEnumerator PlaySoundFXSimultaneouly(SoundFX[] sounds)
	{
		foreach(SoundFX sound in sounds)
		{
			AudioClip s = GetSound(sound);
			
			audioSourceSoundFX.PlayOneShot(s);

			//Debug.Log(string.Format("Played sound {0} in {1}", sound, Time.time));

			yield return new WaitForEndOfFrame();
		}
	}

	public AudioClip GetSound(SoundFX sound)
	{
		AudioClip s = null;

		switch(sound)
		{
			case SoundFX.AttackLoop:
				s = attackLoop;
			break;

			case SoundFX.AttackStart:
				s = attackStart;
			break;

			case SoundFX.Click:
				s = click;
			break;

			case SoundFX.EnemyDie:
				s = enemyDie;
			break;

			case SoundFX.Freeze:
				s = freeze;
			break;

			case SoundFX.MenuIn:
				s = menuIn;
			break;

			case SoundFX.MenuOut:
				s = menuOut;
			break;

			case SoundFX.MenuScroll:
				s = menuScroll;
			break;

			case SoundFX.OrbPP:
				s = orbPP;
			break;

			case SoundFX.OrbP:
				s = orbP;
			break;

			case SoundFX.OrbM:
				s = orbM;
			break;

			case SoundFX.OrbG:
				s = orbG;
			break;

			case SoundFX.Pause:
				s = pause;
			break;

			case SoundFX.Resume:
				s = resume;
			break;

			case SoundFX.ShopBuy:
				s = shopBuy;
			break;

			case SoundFX.DeathRay:
				s = deathRay;
			break;

			case SoundFX.PowerUpCollected:
				s = powerUpCollect;
			break;

			case SoundFX.Score:
				s = score;
			break;

			case SoundFX.PlayerDamage:
				s = playerDamage;
			break;

			case SoundFX.LevelUp:
				s = levelUp;
			break;

			case SoundFX.BossMeteorIdle:
				s = bossMeteorIdle;
			break;

			case SoundFX.BossMeteorDamage:
				s = bossMeteorDamage;
			break;

			case SoundFX.BossMeteorDrop:
				float rnd = Random.Range(0f, 1f);

				if(rnd < 0.33f)
					s = meteorDrop1;
				else if(rnd < 0.66f)
					s = meteorDrop2;
				else
					s = meteorDrop3;
			break;

			case SoundFX.BossTwinsIdle:
				s = bossTwinsIdle;
			break;

			case SoundFX.BossTwinsDamage:
				s = bossTwinsDamage;
			break;

			case SoundFX.BossIllusionIdle:
				s = bossIllusionIdle;
			break;

			case SoundFX.BossIllusionMultiply:
				s = bossIllusionMultiply;
			break;

			case SoundFX.BossIllusionDamage:
				s = bossIllusionDamage;
			break;

			case SoundFX.BossDie:
				s = bossDie;
			break;
		}

		return s;
	}

	public void MuteMusic()
	{
		musicMute = true;
		audioSourceMusic.mute = true;
		audioSourceMusic2.mute = true;
		audioSourceMusic3.mute = true;
	}

	public void UnmuteMusic()
	{
		musicMute = false;
		audioSourceMusic.mute = false;
	}

	public void MuteSoundFX()
	{
		soundFXMute = true;
		audioSourceSoundFX.mute = true;
	}
	
	public void UnmuteSoundFX()
	{
		soundFXMute = false;
		audioSourceSoundFX.mute = false;
	}
}
