using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*!!!Notes: 
 * Script Version (2.0V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * Will be upgraded to include 3D casts
 * 
 * Update Log:
 * 1.0V     -     : - Basic Introduction with library and its components.
 * 1.1V     -     : - Chunks and chains sounds (Doesn't actually work)
 * 1.2V     -     : - StartCoroutine style code allows for more action.
 * 1.3V     -     : - Includes sound volume and pitch adjustments.
 * 1.4V     -     : - Basic inclusion of delay for more possible adjustments.
 * 1.5V     -     : - Optimization
 * 1.6V     -     : - Sound Cast introduction.
 * 1.7V     -     : - Sound Cast optimization as well as including an updating integral.
 * 1.8V     -     : - Info statements and code cleanup.
 * 1.9V     -     : - Actually make the chunks and chains work. (Teehee)
 * 2.0V 05/02/2022: - Included looping for chunks within the chains, and the pieces within the chunks. (Ehe, it actually didn't work before this)
 *                  - Sound cast discovery optimization (Include this version into 2D when the time comes!!!)
 * 2.1V 13/02/2022: - Renamed to SoundGeneralDynamics, Seperated Sound Cast from Sound General Dynamics, added sound lumping (Sounds in the same zone doesn't just cause itself to be louder)
 * 2.2V 22/02/2022: - Doing surround sound.
 */
public class SoundGeneralDynamics : MonoBehaviour
{
    [SerializeField] SoundChain[] soundChains;
    [SerializeField] SoundChunk[] soundChunks;
    [SerializeField] SoundPiece[] soundPieces;
    [SerializeField] SoundCastDynamics[] soundCasts;

    private GameObject ears;

    public void Initialization() //Public so outside sources can reach this if needed
    {
        {
            foreach (SoundChain SoundChain in soundChains)
                foreach (SoundChunk SoundChunk in SoundChain.soundChunks)
                    foreach (SoundPiece SoundPiece in SoundChunk.soundPieces)
                    {
                        if (SoundPiece.soundCastName != "")
                        {
                            List<SoundCast> _CommonCasts = new List<SoundCast>();
                            foreach (SoundCastDynamics SoundCastDynamics in soundCasts)
                            {
                                foreach (SoundCast SoundCasts in SoundCastDynamics.soundCasts)
                                {
                                    if (SoundCasts.castName == SoundPiece.soundCastName)
                                    {
                                        _CommonCasts.Add(SoundCasts);
                                    }
                                }
                            }
                            SoundPiece.soundCastSource = _CommonCasts.ToArray();
                        }
                    }
            foreach (SoundChunk SoundChunk in soundChunks)
                foreach (SoundPiece SoundPiece in SoundChunk.soundPieces)
                {
                    if (SoundPiece.soundCastName != "")
                    {
                        List<SoundCast> _CommonCasts = new List<SoundCast>();
                        foreach (SoundCastDynamics SoundCastDynamics in soundCasts)
                        {
                            foreach (SoundCast SoundCasts in SoundCastDynamics.soundCasts)
                            {
                                if (SoundCasts.castName == SoundPiece.soundCastName)
                                {
                                    _CommonCasts.Add(SoundCasts);
                                }
                            }
                        }
                        SoundPiece.soundCastSource = _CommonCasts.ToArray();
                    }
                }
            foreach (SoundPiece SoundPiece in soundPieces)
            {
                if (SoundPiece.soundCastName != "")
                {
                    List<SoundCast> _CommonCasts = new List<SoundCast>();
                    foreach (SoundCastDynamics SoundCastDynamics in soundCasts)
                    {
                        foreach (SoundCast SoundCasts in SoundCastDynamics.soundCasts)
                        {
                            if (SoundCasts.castName == SoundPiece.soundCastName)
                            {
                                _CommonCasts.Add(SoundCasts);
                            }
                        }
                    }
                    SoundPiece.soundCastSource = _CommonCasts.ToArray();
                }
            }
        } // Sound Setting Sound Casts to sound pieces
        {
            foreach (SoundChain SoundChain in soundChains)
                foreach (SoundChunk SoundChunk in SoundChain.soundChunks)
                    foreach (SoundPiece SoundPiece in SoundChunk.soundPieces)
                    {
                        SoundPiece.source = gameObject.AddComponent<AudioSource>();
                        SoundPiece.source.clip = SoundPiece.Audio;
                        SoundPiece.source.volume = SoundPiece.originalMaxVolume = SoundChain.Volume * SoundChunk.Volume * SoundPiece.Volume;
                        SoundPiece.source.pitch = SoundPiece.originalPitch = SoundChain.Pitch * SoundChunk.Pitch * SoundPiece.Pitch;
                        SoundPiece.source.spatialBlend = 1;
                        if (SoundPiece.AudioMixer != null)
                            SoundPiece.source.outputAudioMixerGroup = SoundPiece.AudioMixer;
                        SoundPiece.delayBeforeWait = new WaitForSeconds(SoundPiece.delayBefore); //Create and Cache all wait for second objects.
                        SoundPiece.delayDuringWait = new WaitWhile(() => SoundPiece.source.isPlaying); //Create and Cache all wait for second objects.
                        SoundPiece.delayAfterWait = new WaitForSeconds(SoundPiece.delayAfter); //Create and Cache all wait for second objects.
                    }
            foreach (SoundChunk SoundChunk in soundChunks)
                foreach (SoundPiece SoundPiece in SoundChunk.soundPieces)
                {
                    SoundPiece.source = gameObject.AddComponent<AudioSource>();
                    SoundPiece.source.clip = SoundPiece.Audio;
                    SoundPiece.source.volume = SoundPiece.originalMaxVolume = SoundChunk.Volume * SoundPiece.Volume;
                    SoundPiece.source.pitch = SoundPiece.originalPitch = SoundChunk.Pitch * SoundPiece.Pitch;
                    SoundPiece.source.spatialBlend = 1;
                    if (SoundPiece.AudioMixer != null)
                        SoundPiece.source.outputAudioMixerGroup = SoundPiece.AudioMixer;
                    SoundPiece.delayBeforeWait = new WaitForSeconds(SoundPiece.delayBefore); //Create and Cache all wait for second objects.
                    SoundPiece.delayDuringWait = new WaitWhile(() => SoundPiece.source.isPlaying); //Create and Cache all wait for second objects.
                    SoundPiece.delayAfterWait = new WaitForSeconds(SoundPiece.delayAfter); //Create and Cache all wait for second objects.
                }
            foreach (SoundPiece SoundPiece in soundPieces)
            {
                SoundPiece.source = gameObject.AddComponent<AudioSource>();
                SoundPiece.source.clip = SoundPiece.Audio;
                SoundPiece.source.volume = SoundPiece.originalMaxVolume = SoundPiece.Volume;
                SoundPiece.source.pitch = SoundPiece.originalPitch = SoundPiece.Pitch;
                SoundPiece.source.spatialBlend = 1;
                if (SoundPiece.AudioMixer != null)
                    SoundPiece.source.outputAudioMixerGroup = SoundPiece.AudioMixer;
                SoundPiece.delayBeforeWait = new WaitForSeconds(SoundPiece.delayBefore); //Create and Cache all wait for second objects.
                SoundPiece.delayDuringWait = new WaitWhile(() => SoundPiece.source.isPlaying); //Create and Cache all wait for second objects.
                SoundPiece.delayAfterWait = new WaitForSeconds(SoundPiece.delayAfter); //Create and Cache all wait for second objects.
            }
        } // Setting values to each sound Pieces
    }

    // Awake is a preload
    void Awake()
    {
        Initialization();
    }

    private void Start()
    {
        ears = FindObjectOfType<AudioListener>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(ears == null)
        {
            ears = FindObjectOfType<AudioListener>().gameObject;
        }
        foreach (SoundChain soundChain in soundChains)
        {
            foreach (SoundChunk soundChunk in soundChain.soundChunks)
            {
                foreach (SoundPiece SoundPiece in soundChunk.soundPieces)
                {
                    if (SoundPiece.source != null && SoundPiece.source.isPlaying && SoundPiece.soundCastSource != null)
                    {
                        float _tempVolume = 0;
                        float _tempPan = 0;

                        SoundPiece.originalMaxVolume = soundChain.Volume * soundChunk.Volume * SoundPiece.Volume;
                        SoundPiece.originalPitch = SoundPiece.Pitch;
                        SoundPiece.source.volume = 0f;
                        foreach (SoundCast SoundCast in SoundPiece.soundCastSource)
                        {
                            float distance = Vector3.Distance(ears.transform.position, SoundCast.soundDynamic.gameObject.transform.position);
                            _tempVolume = Mathf.Min(_tempVolume, distance / SoundCast.soundRange);
                            SoundPiece.source.panStereo = (SoundCast.soundDynamic.gameObject.transform.position.x - ears.transform.position.x) / SoundCast.soundRange;
                            //SoundPiece.source.pitch = (1f - (distance / SC.soundRange)) * SoundPiece.originalPitch;
                        }
                        SoundPiece.source.volume = (1f - _tempVolume) * SoundPiece.originalMaxVolume;
                        SoundPiece.source.panStereo = _tempPan;
                    }
                }
            }
        }
        foreach (SoundChunk soundChunk in soundChunks)
        {
            foreach (SoundPiece SoundPiece in soundChunk.soundPieces)
            {
                if (SoundPiece.source != null && SoundPiece.source.isPlaying && SoundPiece.soundCastSource != null)
                {
                    float _tempVolume = 0;
                    float _tempPan = 0;

                    SoundPiece.originalMaxVolume = soundChunk.Volume * SoundPiece.Volume;
                    SoundPiece.originalPitch = SoundPiece.Pitch;
                    SoundPiece.source.volume = 0f;
                    foreach (SoundCast SoundCast in SoundPiece.soundCastSource)
                    {
                        float distance = Vector3.Distance(ears.transform.position, SoundCast.soundDynamic.gameObject.transform.position);
                        _tempVolume = Mathf.Min(_tempVolume, distance / SoundCast.soundRange);
                        SoundPiece.source.panStereo = (SoundCast.soundDynamic.gameObject.transform.position.x - ears.transform.position.x) / SoundCast.soundRange;
                        //SoundPiece.source.pitch = (1f - (distance / SC.soundRange)) * SoundPiece.originalPitch;
                    }
                    SoundPiece.source.volume = (1f - _tempVolume) * SoundPiece.originalMaxVolume;
                    SoundPiece.source.panStereo = _tempPan;
                }
            }
        }
        foreach (SoundPiece SoundPiece in soundPieces)
        {
            {
                //if (SoundPiece.source != null && SoundPiece.source.isPlaying && SoundPiece.soundCastSource != null)
                //{
                //    float _tempVolume = 0;
                //    float _tempPan = 0;
                //    float ImaginaryXPos = 0;
                //    float ImaginaryYPos = 0;
                //    float ImaginaryZPos = 0;
                //    int releventPosNum = 0;

                //    SoundPiece.originalMaxVolume = SoundPiece.Volume;
                //    SoundPiece.originalPitch = SoundPiece.Pitch;
                //    SoundPiece.source.volume = 0f;
                //    foreach (SoundCast SoundCast in SoundPiece.soundCastSource)
                //    {
                //        float distance = Vector3.Distance(ears.transform.position, SoundCast.soundDynamic.gameObject.transform.position);
                //        if (distance < SoundCast.soundRange)
                //        {
                //            float staticVolume = SoundCast.soundRange / distance;
                //            float SpeculativeXPos = ears.transform.position.x / staticVolume;
                //            float SpeculativeYPos = ears.transform.position.y / staticVolume;
                //            float SpeculativeZPos = ears.transform.position.z / staticVolume;
                //            ImaginaryXPos += SpeculativeXPos;
                //            ImaginaryYPos += SpeculativeYPos;
                //            ImaginaryZPos += SpeculativeZPos;
                //            releventPosNum++;
                //            _tempVolume = Mathf.Min(_tempVolume, distance / SoundCast.soundRange);
                //            SoundPiece.source.panStereo = (SoundCast.soundDynamic.gameObject.transform.position.x - ears.transform.position.x) / SoundCast.soundRange;
                //        }
                //        SoundPiece.source.pitch = (1f - (distance / SC.soundRange)) * SoundPiece.originalPitch;
                //    }

                //    SoundPiece.source.volume = (1f - _tempVolume) * SoundPiece.originalMaxVolume;

                //    Vector3 soundPoint = new Vector3(ImaginaryXPos / releventPosNum, ImaginaryYPos / releventPosNum, ImaginaryZPos / releventPosNum);

                //    Vector3 head = ears.transform.eulerAngles;
                //    Vector3 soundDirection = (soundPoint - ears.transform.position).normalized;


                //    SoundPiece.source.panStereo = _tempPan;
                //}
                //Quaternion.FromToRotation
            }
            if (SoundPiece.source != null && SoundPiece.source.isPlaying && SoundPiece.soundCastSource != null)
            {
                float _tempVolume = 0;
                float _tempPan = 0;

                SoundPiece.originalMaxVolume = SoundPiece.Volume;
                SoundPiece.originalPitch = SoundPiece.Pitch;
                SoundPiece.source.volume = 0f;
                foreach (SoundCast SoundCast in SoundPiece.soundCastSource)
                {
                    float distance = Vector3.Distance(ears.transform.position, SoundCast.soundDynamic.gameObject.transform.position);
                    _tempVolume = Mathf.Min(_tempVolume, distance / SoundCast.soundRange);
                    SoundPiece.source.panStereo = (SoundCast.soundDynamic.gameObject.transform.position.x - ears.transform.position.x) / SoundCast.soundRange;
                    //SoundPiece.source.pitch = (1f - (distance / SC.soundRange)) * SoundPiece.originalPitch;
                }
                SoundPiece.source.volume = (1f - _tempVolume) * SoundPiece.originalMaxVolume;
                SoundPiece.source.panStereo = _tempPan;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (SoundCastDynamics SoundCastDynamics in soundCasts)
        {
            foreach (SoundCast SoundCast in SoundCastDynamics.soundCasts)
            {
                Gizmos.DrawWireSphere(SoundCastDynamics.gameObject.transform.position, SoundCast.soundRange);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------- !!!Code Divider to Keep it clean
    
    public void PlayChain(int soundChainID) //Chain plays chunks of sound until there is no more in that chain, set the delay to 0 on the sound chunk to make them play together
    {
        StartCoroutine(playSoundChain(soundChainID));
    }
    
    public void PlayChunk(int soundChunkID) //Chunk randomizes 1 sound from the list to create uniqueness
    {
        StartCoroutine(playSoundChunk(soundChunkID));
    }
    
    public void PlayPiece(int soundPieceID) //Piece plays a sound based on volume and pitch
    {
        //soundPieces[soundPieceID].source.PlayDelayed(soundPieces[soundPieceID].delayBefore);
        StartCoroutine(playSoundPiece(soundPieceID));
    }

    //----------------------------------------------------------------------------------------------------------- !!!Code Divider to Keep it clean
    
    public void PlayChain(string soundChainName) //Chain plays chunks of sound until there is no more in that chain, set the delay to 0 on the sound chunk to make them play together, overload for name instead of id
    {
        //System.Array.Find(soundChains, soundChains => soundChains.chainName == soundChainName); !!!Note Possible usable system method, very useful
        int i = System.Array.IndexOf(soundChains, System.Array.Find(soundChains, soundChains => soundChains.chainName == soundChainName));
        StartCoroutine(playSoundChain(i)); //Using system to search for the instance value of array with the desired name, and pick it's instance index for array call usage
    }
    
    public void PlayChunk(string soundChunkName) //Chunk randomizes 1 sound from the list to create uniqueness, overload for name instead of id
    {
        int i = System.Array.IndexOf(soundChunks, System.Array.Find(soundChunks, soundChunks => soundChunks.chunkName == soundChunkName));
        StartCoroutine(playSoundChunk(i)); //Using system to search for the instance value of array with the desired name, and pick it's instance index for array call usage
    }
    
    public void PlayPiece(string soundPieceName) //Piece plays a sound based on volume and pitch, overload for name instead of id
    {
        int i = System.Array.IndexOf(soundPieces, System.Array.Find(soundPieces, soundPieces => soundPieces.pieceName == soundPieceName));
        StartCoroutine(playSoundPiece(i)); //Using system to search for the instance value of array with the desired name, and pick it's instance index for array call usage
    }


    //----------------------------------------------------------------------------------------------------------- !!!Code Divider to Keep it clean

    IEnumerator playSoundChain(int soundChainID) //An IEnumerator to play the specific sound(Chain, Chunk or Piece) by ID
    {
        int tempLooper = soundChains[soundChainID].loopNum; 
        while (tempLooper-- != 0)
        {
            for (int i = 0; i < soundChains[soundChainID].soundChunks.Length; i++)
            {
                int _tempLooper = soundChains[soundChainID].soundChunks[i].loopNum;
                while (_tempLooper-- != 0)
                {
                    int rand = Random.Range(0, soundChains[soundChainID].soundChunks[i].soundPieces.Length); //Produce the random instance of the choosen chunk in the chain, picked and used to check for the following array.
                    int __tempLooper = soundChains[soundChainID].soundChunks[i].soundPieces[rand].loopNum;
                    while (__tempLooper-- != 0)
                    {
                        yield return soundChains[soundChainID].soundChunks[i].soundPieces[rand].delayBeforeWait; //Delay the sound until as stated duration later, used for timing
                        soundChains[soundChainID].soundChunks[i].soundPieces[rand].source.Play(); //Play the attributed sound source
                        if (soundChains[soundChainID].soundChunks[i].soundPieces[rand].waitForSoundFinish)
                            yield return soundChains[soundChainID].soundChunks[i].soundPieces[rand].delayDuringWait;
                        yield return soundChains[soundChainID].soundChunks[i].soundPieces[rand].delayAfterWait;
                    }
                }
            //audioSource.PlayOneShot(soundChains[soundChainID].soundChunks[i].soundPieces[rand].Audio, soundChains[soundChainID].Volume * soundChains[soundChainID].soundChunks[i].Volume * soundChains[soundChainID].soundChunks[i].soundPieces[rand].Volume * Volume); //Play the sound stated following the Volume multipliers
            }
        }
    }

    IEnumerator playSoundChunk(int soundChunkID) //An IEnumerator to play the specific sound(Chain, Chunk or Piece) by ID
    {
        int tempLooper = soundChunks[soundChunkID].loopNum;
        while (tempLooper-- != 0)
        {
            int rand = Random.Range(0, soundChunks[soundChunkID].soundPieces.Length); //Produce the random instance of the choosen sound in the chunk, picked and used to check for the following arrays
            int _tempLooper = soundChunks[soundChunkID].soundPieces[rand].loopNum;
            while (_tempLooper-- != 0)
            {
                yield return soundChunks[soundChunkID].soundPieces[rand].delayBeforeWait; //Delay the sound until as stated duration later, used for timing
                soundChunks[soundChunkID].soundPieces[rand].source.Play(); //Play the attributed sound source
                if (soundChunks[soundChunkID].soundPieces[rand].waitForSoundFinish)
                    yield return soundChunks[soundChunkID].soundPieces[rand].delayDuringWait;
                yield return soundChunks[soundChunkID].soundPieces[rand].delayAfterWait;
            }

        //audioSource.PlayOneShot(soundChunks[soundChunkID].soundPieces[rand].Audio, soundChunks[soundChunkID].Volume * soundChunks[soundChunkID].soundPieces[rand].Volume * Volume); //Play the sound stated following the Volume multipliers

        }
    }

    IEnumerator playSoundPiece(int soundPieceID) //An IEnumerator to play the specific sound(Chain, Chunk or Piece) by ID
    {
        int tempLooper = soundPieces[soundPieceID].loopNum;
        while(tempLooper-- != 0)
        {
        yield return soundPieces[soundPieceID].delayBeforeWait; //Delay the sound until as stated duration later, used for timing
            soundPieces[soundPieceID].source.Play(); //Play the attributed sound source
        //Debug.Log(soundPieces[soundPieceID].source.clip.length);
        if (soundPieces[soundPieceID].waitForSoundFinish)
            yield return soundPieces[soundPieceID].delayDuringWait;
        yield return soundPieces[soundPieceID].delayAfterWait;

        //audioSource.PlayOneShot(soundPieces[soundPieceID].Audio, soundPieces[soundPieceID].Volume * Volume); //Play the sound stated following the Volume multipliers

        }
    }

    //----------------------------------------------------------------------------------------------------------- !!!Code Divider to Keep it clean

    [System.Serializable]
    public class SoundChain
    {
        public string chainName;
        public SoundChunk[] soundChunks;

        [Header("Sound Settings")]
        [Tooltip("This sets the number of looping before stopping, put the value below 0 for infinity.")]
        public int loopNum;
        [Tooltip("This sets the volume, multiplicative of each of the volumes within the structures.")]
        [Min(0f)] public float Volume = 1f;
        [Tooltip("This sets the pitch, multiplicative of each of the pitches within the structures.")]
        [Min(0.1f)] public float Pitch = 1f;
    }


    [System.Serializable]
    public class SoundChunk
    {
        public string chunkName;
        public SoundPiece[] soundPieces;

        [Header("Sound Settings")]
        [Tooltip("This sets the number of looping before stopping, put the value below 0 for infinity.")]
        public int loopNum;
        [Tooltip("This sets the volume, multiplicative of each of the volumes within the structures.")]
        [Min(0f)] public float Volume = 1f;
        [Tooltip("This sets the pitch, multiplicative of each of the pitches within the structures.")]
        [Min(0.1f)] public float Pitch = 1f;
    }


    [System.Serializable]
    public class SoundPiece
    {

        public string pieceName;
        public AudioClip Audio;
        public AudioMixerGroup AudioMixer;
        public string soundCastName;

        [Header("Sound Settings")]
        [Tooltip("This sets the number of looping before stopping, put the value below 0 for infinity.")]
        public int loopNum;
        [Tooltip("Delay before will cause the sound to wait the set moment before being played, useful for timing chains of sound as well as making cooldown time.")]
        [Min(0f)] public float delayBefore = 0f;
        [Tooltip("Causes the sound to finish playing before the next one can start, use this to make sure your loops don't play all at once, as well as delaying chains of sounds for timing.")]
        public bool waitForSoundFinish;
        [Tooltip("Delay after will cause the sound to wait the set moment after being played, useful for timing chains of sound as well as making cooldown time.")]
        [Min(0f)] public float delayAfter = 0f;
        [Tooltip("This sets the volume, multiplicative of each of the volumes within the structures.")]
        [Min(0f)] public float Volume = 1f;
        [Tooltip("This sets the pitch, multiplicative of each of the pitches within the structures.")]
        [Min(0.1f)] public float Pitch = 1f;

        [HideInInspector] public float originalMaxVolume;
        [HideInInspector] public float originalPitch;
        [HideInInspector] public AudioSource source;
        [HideInInspector] public SoundCast[] soundCastSource;

        [HideInInspector] public WaitForSeconds delayBeforeWait;
        [HideInInspector] public WaitWhile delayDuringWait;
        [HideInInspector] public WaitForSeconds delayAfterWait;
    }

}