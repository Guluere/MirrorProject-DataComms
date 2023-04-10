using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/*!!!Notes: 
 * Script Version (2.7V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * Will be upgraded to include 3D casts
 * 
 * Update Log:
 * 1.0V     -     : - Basic Introduction with library and its components..
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
 * 2.2V 22/02/2022: - Synced multiplayer sound play, doing surround sound.
 * 2.3V 08/03/2022: - Surround Sound based on y rotation, as long as the ears doesn't tilt.
 * 2.4V 15/03/2022: - Added Play On Start, made tilt included in calculation.
 * 2.5V 04/04/2022: - Progress on sound visual indicator.
 * 2.6V 09/04/2022: - Finished Sound Visualizer, not tested.
 * 2.7V 17/04/2022: - Fixed some bugs, optimized, seperated visual indicator with a last known location director, removed continuous checker(Hated this to bits)
 */
public class SoundGeneralDynamicsMultiplayerPhoton : MonoBehaviour
{
    [SerializeField] SoundChain[] soundChains;
    [SerializeField] SoundChunk[] soundChunks;
    [SerializeField] SoundPiece[] soundPieces;
    [SerializeField] SoundCastDynamics[] soundCasts;
    [SerializeField] SoundVisualizer[] soundVisualUI;

    private GameObject ears;

    void soundVisualUIInitialization()
    {
        int VisualizerCounter = 0;
        foreach (SoundVisualizer soundVisualizer in soundVisualUI)
        {
            //Create or hold the canvas that is specified
            GameObject canvas;
            if (soundVisualizer.canvas == null)
            {
                canvas = new GameObject("Visualizer Canvas " + VisualizerCounter++ + ": " + soundVisualizer.visualizerName);
                Canvas c = canvas.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler s = canvas.AddComponent<CanvasScaler>();
                s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvas.AddComponent<GraphicRaycaster>();
                canvas.transform.SetParent(gameObject.transform);

            }
            else if (soundVisualizer.canvas.scene.name == null) // Check if it's prefab
            {
                canvas = Instantiate(soundVisualizer.canvas);
                canvas.transform.SetParent(gameObject.transform);
            }
            else canvas = soundVisualizer.canvas;

            int VisualizerDynamicPartCounter = 0;
            int VisualizerStaticPartCounter = 0;
            foreach (SoundVisualAssets soundVisualAssets in soundVisualizer.VisualizerDynamicPart)
            {
                GameObject Image = new GameObject("Visualizer Dynamic Asset " + VisualizerDynamicPartCounter++);
                Image image = Image.AddComponent<Image>();
                image.sprite = soundVisualAssets.VisualSprite;
                RectTransform transform = Image.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(soundVisualAssets.XSize, soundVisualAssets.YSize);
                Color changeVisibility = image.color;
                changeVisibility.a = 0;
                image.color = changeVisibility;

                Image.transform.SetParent(canvas.transform);

                soundVisualAssets.ObjectImage = Image;
                soundVisualAssets.RectImage = Image.GetComponent<RectTransform>();
                soundVisualAssets.ComponentImage = image;
            }
            foreach (SoundVisualAssets soundVisualAssets in soundVisualizer.VisualizerStaticPart)
            {
                GameObject Image = new GameObject("Visualizer Static Asset " + VisualizerStaticPartCounter++);
                Image image = Image.AddComponent<Image>();
                image.sprite = soundVisualAssets.VisualSprite;
                RectTransform transform = Image.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(soundVisualAssets.XSize, soundVisualAssets.YSize);
                Color changeVisibility = image.color;
                changeVisibility.a = 0;
                image.color = changeVisibility;

                Image.transform.SetParent(canvas.transform);

                soundVisualAssets.ObjectImage = Image;
                soundVisualAssets.RectImage = Image.GetComponent<RectTransform>();
                soundVisualAssets.ComponentImage = image;

            }
            soundVisualizer.canvas = canvas;
        }
    }

    public void Initialization() //Public so outside sources can reach this if needed
    {
        {
            int CurrentChainID = 0; //In C#, there is no way to mark the instance of which the current Index is for an foreach...unless you instantiate a counter or use a for loop.
            int CurrentChunkID = 0; //In C#, there is no way to mark the instance of which the current Index is for an foreach...unless you instantiate a counter or use a for loop.
            int CurrentPieceID = 0; //In C#, there is no way to mark the instance of which the current Index is for an foreach...unless you instantiate a counter or use a for loop.
            foreach (SoundChain SoundChain in soundChains)
            {
                foreach (SoundChunk SoundChunk in SoundChain.soundChunks)
                    foreach (SoundPiece SoundPiece in SoundChunk.soundPieces)
                    {
                        if (SoundPiece.soundCastName != "") //Initializes the cast sets
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
                        if (SoundPiece.soundVisualizerName != "") //Initializes the visual sets
                        {
                            List<SoundVisualizer> _CommonVisuals = new List<SoundVisualizer>();
                            foreach (SoundVisualizer soundVisualizer in soundVisualUI)
                            {
                                if (soundVisualizer.visualizerName == SoundPiece.soundVisualizerName)
                                {
                                    _CommonVisuals.Add(soundVisualizer);
                                }
                            }
                            SoundPiece.soundVisualSource = _CommonVisuals.ToArray();
                        }

                        SoundPiece.source = gameObject.AddComponent<AudioSource>();
                        SoundPiece.source.clip = SoundPiece.Audio;
                        SoundPiece.source.volume = SoundPiece.originalMaxVolume = SoundChain.Volume * SoundChunk.Volume * SoundPiece.Volume;
                        SoundPiece.source.pitch = SoundPiece.originalPitch = SoundChain.Pitch * SoundChunk.Pitch * SoundPiece.Pitch;
                        SoundPiece.source.playOnAwake = false;
                        if (SoundPiece.AudioMixer != null)
                            SoundPiece.source.outputAudioMixerGroup = SoundPiece.AudioMixer;
                        SoundPiece.delayBeforeWait = new WaitForSeconds(SoundPiece.delayBefore); //Create and Cache all wait for second objects.
                        SoundPiece.delayDuringWait = new WaitWhile(() => SoundPiece.source.isPlaying); //Create and Cache all wait for second objects.
                        SoundPiece.delayAfterWait = new WaitForSeconds(SoundPiece.delayAfter); //Create and Cache all wait for second objects.
                    }
                if (SoundChain.playOnStart) PlayChain(CurrentChainID++);
                CurrentChainID++;
            }
            foreach (SoundChunk SoundChunk in soundChunks)
            {
                foreach (SoundPiece SoundPiece in SoundChunk.soundPieces)
                {
                    if (SoundPiece.soundCastName != "") //Initializes the cast sets
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
                    if (SoundPiece.soundVisualizerName != "") //Initializes the visual sets
                    {
                        List<SoundVisualizer> _CommonVisuals = new List<SoundVisualizer>();
                        foreach (SoundVisualizer soundVisualizer in soundVisualUI)
                        {
                            if (soundVisualizer.visualizerName == SoundPiece.soundVisualizerName)
                            {
                                _CommonVisuals.Add(soundVisualizer);
                            }
                        }
                        SoundPiece.soundVisualSource = _CommonVisuals.ToArray();
                    }

                    SoundPiece.source = gameObject.AddComponent<AudioSource>();
                    SoundPiece.source.clip = SoundPiece.Audio;
                    SoundPiece.source.volume = SoundPiece.originalMaxVolume = SoundChunk.Volume * SoundPiece.Volume;
                    SoundPiece.source.pitch = SoundPiece.originalPitch = SoundChunk.Pitch * SoundPiece.Pitch;
                    SoundPiece.source.playOnAwake = false;
                    if (SoundPiece.AudioMixer != null)
                        SoundPiece.source.outputAudioMixerGroup = SoundPiece.AudioMixer;
                    SoundPiece.delayBeforeWait = new WaitForSeconds(SoundPiece.delayBefore); //Create and Cache all wait for second objects.
                    SoundPiece.delayDuringWait = new WaitWhile(() => SoundPiece.source.isPlaying); //Create and Cache all wait for second objects.
                    SoundPiece.delayAfterWait = new WaitForSeconds(SoundPiece.delayAfter); //Create and Cache all wait for second objects.
                }
                if (SoundChunk.playOnStart) PlayChunk(CurrentChunkID);
                CurrentChunkID++;
            }
            foreach (SoundPiece SoundPiece in soundPieces)
            {
                if (SoundPiece.soundCastName != "") //Initializes the cast sets
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
                if (SoundPiece.soundVisualizerName != "") //Initializes the visual sets
                {
                    List<SoundVisualizer> _CommonVisuals = new List<SoundVisualizer>();
                    foreach (SoundVisualizer soundVisualizer in soundVisualUI)
                    {
                        if (soundVisualizer.visualizerName == SoundPiece.soundVisualizerName)
                        {
                            _CommonVisuals.Add(soundVisualizer);
                        }
                    }
                    SoundPiece.soundVisualSource = _CommonVisuals.ToArray();
                }

                SoundPiece.source = gameObject.AddComponent<AudioSource>();
                SoundPiece.source.clip = SoundPiece.Audio;
                SoundPiece.source.volume = SoundPiece.originalMaxVolume = SoundPiece.Volume;
                SoundPiece.source.pitch = SoundPiece.originalPitch = SoundPiece.Pitch;
                SoundPiece.source.playOnAwake = false;
                if (SoundPiece.AudioMixer != null)
                    SoundPiece.source.outputAudioMixerGroup = SoundPiece.AudioMixer;
                SoundPiece.delayBeforeWait = new WaitForSeconds(SoundPiece.delayBefore); //Create and Cache all wait for second objects.
                SoundPiece.delayDuringWait = new WaitWhile(() => SoundPiece.source.isPlaying); //Create and Cache all wait for second objects.
                SoundPiece.delayAfterWait = new WaitForSeconds(SoundPiece.delayAfter); //Create and Cache all wait for second objects.
                if (SoundPiece.playOnStart) PlayPiece(CurrentPieceID);
                CurrentPieceID++;
            }
        } // Setting values to each sound Pieces
    }

    // Awake is a preload
    void Awake()
    {
        Initialization();
        soundVisualUIInitialization();
    }

    private void Start()
    {
        ears = FindObjectOfType<AudioListener>().gameObject;
    }

    private void UpdateCalculate()
    {
        foreach (SoundChain soundChain in soundChains)
        {
            foreach (SoundChunk soundChunk in soundChain.soundChunks)
            {
                foreach (SoundPiece SoundPiece in soundChunk.soundPieces)
                {
                    CalculateSoundSourceCast(SoundPiece, soundChain.Volume * soundChunk.Volume * SoundPiece.Volume, soundChain.Pitch * soundChunk.Pitch * SoundPiece.Pitch);
                }
            }
        }
        foreach (SoundChunk soundChunk in soundChunks)
        {
            foreach (SoundPiece SoundPiece in soundChunk.soundPieces)
            {
                CalculateSoundSourceCast(SoundPiece, soundChunk.Volume * SoundPiece.Volume, soundChunk.Pitch * SoundPiece.Pitch);
            }
        }
        foreach (SoundPiece SoundPiece in soundPieces)
        {
            CalculateSoundSourceCast(SoundPiece, SoundPiece.Volume, SoundPiece.Pitch);
        }
    }

    private void CalculateSoundSourceCast(SoundPiece SoundPiece, float MaxVolume, float MaxPitch)
    {
        if (SoundPiece.source != null && SoundPiece.source.isPlaying && SoundPiece.soundCastSource != null && SoundPiece.soundCastName != "")
        {
            float _tempVolume = 1f; //Cannot be 0 in this min calculation...
            Vector3 totalBalance = Vector3.zero;
            int releventPosNum = 0;

            SoundPiece.originalMaxVolume = MaxVolume;
            SoundPiece.originalPitch = MaxPitch;
            SoundPiece.source.volume = 0f;
            foreach (SoundCast SoundCast in SoundPiece.soundCastSource)
            {
                float distance = Vector3.Distance(ears.transform.position, SoundCast.soundDynamic.gameObject.transform.position);
                if (distance < SoundCast.soundRange)
                {
                    _tempVolume = Mathf.Min(_tempVolume, distance / SoundCast.soundRange);
                    totalBalance += SoundCast.soundDynamic.gameObject.transform.position; //Originally, this was gonna be an multiply each, add and divide but that is not needed
                    releventPosNum++;
                }
            }
            SoundPiece.source.volume = (1f - _tempVolume) * SoundPiece.originalMaxVolume;

            Vector3 soundDirection = (totalBalance - ears.transform.position * releventPosNum).normalized;

            if (SoundPiece.soundVisualizerName != null && SoundPiece.soundVisualSource != null)
            {
                foreach (SoundVisualizer soundVisualizer in SoundPiece.soundVisualSource)
                {
                    soundVisualizer.LastKnownSoundPoint = soundDirection;
                }
            }
            float DifferenciationY = Vector3.Dot(soundDirection, ears.transform.right);

            SoundPiece.source.panStereo = DifferenciationY;
        }

        if (SoundPiece.soundVisualizerName != null && SoundPiece.soundVisualSource != null)
        {
            foreach (SoundVisualizer soundVisualizer in SoundPiece.soundVisualSource)
            {
                float DifferenciationY = Vector3.Dot(soundVisualizer.LastKnownSoundPoint, ears.transform.right);
                float DifferenciationX = Vector3.Dot(soundVisualizer.LastKnownSoundPoint, ears.transform.forward);
                float DifferenciationZ = Vector3.Dot(soundVisualizer.LastKnownSoundPoint, ears.transform.up);
                CalculateVisualPositioning(soundVisualizer, DifferenciationX, DifferenciationY, DifferenciationZ);
            }
        }

    }
    #region DebugCalls
    //Debug Calls List
    //Debug.Log("0");
    //float ImaginaryXPos = 0;
    //float ImaginaryYPos = 0;
    //float ImaginaryZPos = 0;
    //Debug.Log("distance" + distance);
    //SoundPiece.source.pitch = (1f - (distance / SC.soundRange)) * SoundPiece.originalPitch;
    //Debug.Log("_tempVolume" + _tempVolume);
    //Vector3 soundPoint = new Vector3(ImaginaryXPos / releventPosNum, ImaginaryYPos / releventPosNum, ImaginaryZPos / releventPosNum);

    //Vector3 NormalizedSpeculation = Vector3.Normalize(ears.transform.position);
    //float staticVolume = ( SoundCast.soundRange - distance ) / SoundCast.soundRange; //The futher away something is, the less it's impact becomes.
    //float SpeculativeXPos = NormalizedSpeculation.x * staticVolume;
    //float SpeculativeYPos = NormalizedSpeculation.y * staticVolume;
    //float SpeculativeZPos = NormalizedSpeculation.z * staticVolume;
    //ImaginaryXPos += SpeculativeXPos;
    //ImaginaryYPos += SpeculativeYPos;
    //ImaginaryZPos += SpeculativeZPos;
    //Debug.Log("releventPosNum" + releventPosNum);
    //SoundPiece.source.panStereo = (SoundCast.soundDynamic.gameObject.transform.position.x - ears.transform.position.x) / SoundCast.soundRange;

    //Vector3 soundDirectionAngle = Quaternion.FromToRotation(Vector3.forward, soundDirection).eulerAngles;
    //Vector3 soundDirectionAngleX = Quaternion.FromToRotation(Vector3.up, new Vector3(0, soundDirection.y, soundDirection.z)).eulerAngles;

    //float soundDirectionAngleX = Vector3.SignedAngle(Vector3.up, new Vector3(0, soundDirection.y, soundDirection.z), Vector3.up);
    //float soundDirectionAngleY = Vector3.SignedAngle(Vector3.forward, new Vector3(soundDirection.x, 0, soundDirection.z), Vector3.forward);
    //float soundDirectionAngleZ = Vector3.SignedAngle(Vector3.up, new Vector3(soundDirection.x, soundDirection.y, 0), Vector3.up);

    //Debug.Log("head.x" + head.x);
    //Debug.Log("head.y" + head.y);
    //Debug.Log("head.z" + head.z);

    //Debug.Log("soundDirection.x" + soundDirection.x);
    //Debug.Log("soundDirection.y" + soundDirection.y);
    //Debug.Log("soundDirection.z" + soundDirection.z);


    //float soundDirectionAngleY = Quaternion.FromToRotation(Vector3.forward, soundDirection).eulerAngles.y;
    //float soundDirectionAngleZ = Quaternion.FromToRotation(Vector3.up, soundDirection).eulerAngles.z;

    //Debug.Log("soundDirectionAngleX" + soundDirectionAngleX.x);
    //Debug.Log("soundDirectionAngleY" + soundDirectionAngleY.y);
    //Debug.Log("soundDirectionAngleZ" + soundDirectionAngleZ.z);

    //Debug.Log("soundDirectionAngleX" + soundDirectionAngleX);
    //Debug.Log("soundDirectionAngleY" + soundDirectionAngleY);
    //Debug.Log("soundDirectionAngleZ" + soundDirectionAngleZ);



    //Debug.Log("soundDirectionAngle.x" + soundDirectionAngle.x);
    //Debug.Log("soundDirectionAngle.y" + soundDirectionAngle.y);
    //Debug.Log("soundDirectionAngle.z" + soundDirectionAngle.z);

    //float DifferenciationX = soundDirectionAngleX.x - head.x;
    //Debug.LogFormat("DifferenciationX" + DifferenciationX);
    //Debug.LogFormat("DifferenciationY" + DifferenciationY);
    //Debug.LogFormat("DifferenciationZ" + DifferenciationZ);

    //float EncircleX = Mathf.Abs(Mathf.Cos(DifferenciationX * Mathf.PI / 180));

    //Debug.LogFormat("panStereo" + SoundPiece.source.panStereo);
    //Debug.LogFormat("EncircleX" + EncircleX);
    //Debug.LogFormat("EncircleY" + EncircleY);
    //Debug.LogFormat("EncircleZ" + EncircleZ);

    //float look = Vector3.Dot(ears.transform.forward, soundDirection);
    //Debug.LogFormat("look" + look);

    //if(look < 0f && Mathf.Abs(normalizedEncircles.z) > Mathf.Abs(normalizedEncircles.y))
    //    SoundPiece.source.panStereo = (normalizedEncircles.y + normalizedEncircles.z) * -1;
    //else
    //if (SoundPiece.source != null && SoundPiece.source.isPlaying && SoundPiece.soundCastSource != null)
    //{
    //    float _tempVolume = 0;

    //    SoundPiece.originalMaxVolume = SoundPiece.Volume;
    //    SoundPiece.originalPitch = SoundPiece.Pitch;
    //    SoundPiece.source.volume = 0f;
    //    foreach (SoundCast SoundCast in SoundPiece.soundCastSource)
    //    {
    //        float distance = Vector3.Distance(ears.transform.position, SoundCast.soundDynamic.gameObject.transform.position);
    //        _tempVolume = Mathf.Min(_tempVolume, distance / SoundCast.soundRange);
    //        SoundPiece.source.panStereo = (SoundCast.soundDynamic.gameObject.transform.position.x - ears.transform.position.x) / SoundCast.soundRange;
    //        //SoundPiece.source.pitch = (1f - (distance / SC.soundRange)) * SoundPiece.originalPitch;
    //    }
    //    SoundPiece.source.volume = (1f - _tempVolume) * SoundPiece.originalMaxVolume;
    //}
    //Quaternion.FromToRotation

    #endregion DebugCalls

    #region CalculateVisualPositioningBackupCodes
    //private void CalculateVisualPositioning(float DifferenciationY, float DifferenciationZ) //Sets a normalized Vector2 for the positional direction the sprite should be within the Canvas screen, after that, a multiplier for how far they should be from the center.
    //{
    //    float EncircleForwardBack = Mathf.Cos(DifferenciationY * Mathf.PI / 180); //Returns 1 for forward, and -1 for back.
    //    float EncircleLeftRight = Mathf.Sin(DifferenciationY * Mathf.PI / 180); //Returns 1 for right, -1 for left.
    //    float EncircleUpDown = Mathf.Cos(DifferenciationZ * Mathf.PI / 180); //Returns 1 for up, -1 for down.
    //}
    #endregion CalculateVisualPositioningBackupCodes
    private void CalculateVisualPositioning(SoundVisualizer soundVisualizer, float fowardBack, float leftRight, float upDown) //Sets a normalized Vector2 for the positional direction the sprite should be within the Canvas screen, after that, a multiplier for how far they should be from the center.
    {
        Vector2 EncircleVector = new Vector2(leftRight, upDown).normalized;
        foreach (SoundVisualAssets soundVisualAssets in soundVisualizer.VisualizerDynamicPart)
        {
            if (soundVisualAssets.currentTime > 0)
            {
                soundVisualAssets.currentTime -= Time.deltaTime;
                Vector2 RealVector = new Vector2(EncircleVector.x * soundVisualAssets.XVisualDistanceFromCenter, EncircleVector.y * soundVisualAssets.YVisualDistanceFromCenter);
                soundVisualAssets.RectImage.localPosition = RealVector;
                soundVisualAssets.ObjectImage.transform.up = RealVector;

                float a = Mathf.Min(Mathf.Max(1 - fowardBack - soundVisualAssets.VisualAngle, 0) / soundVisualAssets.VisualAngleBuffer, 1); //Calculate Visibility of UI Based on how close you are looking at them.
                float b = Mathf.Min(soundVisualAssets.currentTime / soundVisualAssets.BufferTime, 1); //Calculate Visibility based on how much longer the visual will remain active.
                Color changeVisibility = soundVisualAssets.ComponentImage.color;
                changeVisibility.a = a * b;
                soundVisualAssets.ComponentImage.color = changeVisibility;
            }
        }
        foreach (SoundVisualAssets soundVisualAssets in soundVisualizer.VisualizerStaticPart)
        {
            if (soundVisualAssets.currentTime > 0)
            {
                soundVisualAssets.currentTime -= Time.deltaTime;
                Vector2 RealVector = new Vector2(EncircleVector.x * soundVisualAssets.XVisualDistanceFromCenter, EncircleVector.y * soundVisualAssets.YVisualDistanceFromCenter);
                soundVisualAssets.RectImage.localPosition = RealVector;
                soundVisualAssets.ObjectImage.transform.localPosition = RealVector;

                float a = Mathf.Min(Mathf.Max(1 - fowardBack - soundVisualAssets.VisualAngle, 0) / soundVisualAssets.VisualAngleBuffer, 1); //Calculate Visibility of UI Based on how close you are looking at them.
                float b = Mathf.Min(soundVisualAssets.currentTime / soundVisualAssets.BufferTime, 1); //Calculate Visibility based on how much longer the visual will remain active.
                Color changeVisibility = soundVisualAssets.ComponentImage.color;
                changeVisibility.a = a * b;
                soundVisualAssets.ComponentImage.color = changeVisibility;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ears == null)
        {
            ears = FindObjectOfType<AudioListener>().gameObject;
        }
        UpdateCalculate();
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
        //Debug.Log(soundChainID);
        StartCoroutine(playSoundChain(soundChainID));
    }
    public void PlayChunk(int soundChunkID) //Chunk randomizes 1 sound from the list to create uniqueness
    {
        //Debug.Log(soundChunkID);
        StartCoroutine(playSoundChunk(soundChunkID));
    }
    public void PlayPiece(int soundPieceID) //Piece plays a sound based on volume and pitch
    {
        //soundPieces[soundPieceID].source.PlayDelayed(soundPieces[soundPieceID].delayBefore);
        //Debug.Log(soundPieceID);
        StartCoroutine(playSoundPiece(soundPieceID));
    }

    //----------------------------------------------------------------------------------------------------------- !!!Code Divider to Keep it clean
    
    public void PlayChain(string soundChainName) //Chain plays chunks of sound until there is no more in that chain, set the delay to 0 on the sound chunk to make them play together, overload for name instead of id
    {
        //System.Array.Find(soundChains, soundChains => soundChains.chainName == soundChainName); !!!Note Possible usable system method, very useful
        //int i = System.Array.IndexOf(soundChains, System.Array.FindIndex(soundChains, soundChains => soundChains.chainName == soundChainName));

        int currentIndex = 0; //manually indicate the selected index, the one provided by System doesn't seem to work
        foreach (SoundChain soundChain in soundChains)
        {
            if (soundChain.chainName == soundChainName)
            {
                StartCoroutine(playSoundChain(currentIndex));
                //Debug.Log(currentIndex);
            }
            currentIndex++;
        }
        //int i = System.Array.FindIndex(soundChains, soundChains => soundChains.chainName == soundChainName);
        //Debug.Log(i);
        //StartCoroutine(playSoundChain(i)); //Using system to search for the instance value of array with the desired name, and pick it's instance index for array call usage
    }
    public void PlayChunk(string soundChunkName) //Chunk randomizes 1 sound from the list to create uniqueness, overload for name instead of id
    {
        int currentIndex = 0; //manually indicate the selected index, the one provided by System doesn't seem to work
        foreach (SoundChunk soundChunk in soundChunks)
        {
            if (soundChunk.chunkName == soundChunkName)
            {
                StartCoroutine(playSoundChunk(currentIndex));
                //Debug.Log(currentIndex);
            }
            currentIndex++;
        }
        //int i = System.Array.FindIndex(soundChunks, soundChunks => soundChunks.chunkName == soundChunkName);
        //Debug.Log(i);
        //StartCoroutine(playSoundChunk(i)); //Using system to search for the instance value of array with the desired name, and pick it's instance index for array call usage
    }
    public void PlayPiece(string soundPieceName) //Piece plays a sound based on volume and pitch, overload for name instead of id
    {
        int currentIndex = 0; //manually indicate the selected index, the one provided by System doesn't seem to work
        foreach (SoundPiece soundPiece in soundPieces)
        {
            Debug.Log("Foreach1");
            if (soundPiece.pieceName == soundPieceName)
            {
                StartCoroutine(playSoundPiece(currentIndex));
                Debug.Log(currentIndex);
            }
            currentIndex++;
            Debug.Log("Index Log" + currentIndex);
        }
        //Debug.Log(currentIndex);
        //int i = System.Array.FindIndex(soundPieces, soundPieces => soundPieces.pieceName == soundPieceName);
        //Debug.Log(i);
        //StartCoroutine(playSoundPiece(i)); //Using system to search for the instance value of array with the desired name, and pick it's instance index for array call usage
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
                        soundChains[soundChainID].soundChunks[i].soundPieces[rand].ActivateTime(); //Activate any existing visual assets
                        UpdateCalculate();
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
                soundChunks[soundChunkID].soundPieces[rand].ActivateTime(); //Activate any existing visual assets
                UpdateCalculate();
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
        while (tempLooper-- != 0)
        {
            yield return soundPieces[soundPieceID].delayBeforeWait; //Delay the sound until as stated duration later, used for timing
            soundPieces[soundPieceID].source.Play(); //Play the attributed sound source
            soundPieces[soundPieceID].ActivateTime(); //Activate any existing visual assets
            UpdateCalculate();
            //Debug.Log(soundPieces[soundPieceID].source.clip.length);
            if (soundPieces[soundPieceID].waitForSoundFinish)
                yield return soundPieces[soundPieceID].delayDuringWait;
            yield return soundPieces[soundPieceID].delayAfterWait;

            //audioSource.PlayOneShot(soundPieces[soundPieceID].Audio, soundPieces[soundPieceID].Volume * Volume); //Play the sound stated following the Volume multipliers

        }
    }

    [System.Serializable]
    public class SoundChain
    {
        public string chainName;
        public SoundChunk[] soundChunks;

        [Header("Sound Settings")]
        [Tooltip("Plays on start, this includes any loop if infinate or not.")]
        public bool playOnStart;
        [Tooltip("This sets the number of looping before stopping, put the value below 0 for infinity.\nIf infinity, please set some value of delay otherwise it crashes the game.")]
        public int loopNum;
        [Tooltip("This sets the volume, multiplicative of each of the volumes within the structures.")]
        [Min(0f)] public float Volume = 1.0f;
        [Tooltip("This sets the pitch, multiplicative of each of the pitches within the structures.")]
        [Min(0.1f)] public float Pitch = 1.0f;
    }


    [System.Serializable]
    public class SoundChunk
    {
        public string chunkName;
        public SoundPiece[] soundPieces;

        [Header("Sound Settings")]
        [Tooltip("Plays on start, this includes any loop if infinate or not. \nIf inside an soundChain, this does not happen.")]
        public bool playOnStart;
        [Tooltip("This sets the number of looping before stopping, put the value below 0 for infinity.\nIf infinity, please set some value of delay otherwise it crashes the game.")]
        public int loopNum;
        [Tooltip("This sets the volume, multiplicative of each of the volumes within the structures.")]
        [Min(0f)] public float Volume = 1.0f;
        [Tooltip("This sets the pitch, multiplicative of each of the pitches within the structures.")]
        [Min(0.1f)] public float Pitch = 1.0f;
    }


    [System.Serializable]
    public class SoundPiece
    {

        public string pieceName;
        public AudioClip Audio;
        public AudioMixerGroup AudioMixer;

        [Header("Sound Cast Settings")]
        [Tooltip("Within this sound general dynamics can have sound cast dynamics set as its array roots, using this sound cast name as the base for the targets to find the sources.")]
        public string soundCastName;

        [Header("Sound Visualizer Settings")]
        [Tooltip("Within this sound general dynamics, you can set up multiple visualizer under the same name and each will be casted based on the stated name, if no cast is set, this does not go off.")]
        public string soundVisualizerName;

        [Header("Sound Settings")]
        [Tooltip("Plays on start, this includes any loop if infinate or not. \nIf inside an soundChunk, this does not happen.")]
        public bool playOnStart;
        [Tooltip("This sets the number of looping before stopping, put the value below 0 for infinity. \nIf infinity, please set some value of delay otherwise it crashes the game.")]
        public int loopNum;
        [Tooltip("Delay before will cause the sound to wait the set moment before being played, useful for timing chains of sound as well as making cooldown time.")]
        [Min(0f)] public float delayBefore = 0f;
        [Tooltip("Causes the sound to finish playing before the next one can start, use this to make sure your loops don't play all at once, as well as delaying chains of sounds for timing.")]
        public bool waitForSoundFinish;
        [Tooltip("Delay after will cause the sound to wait the set moment after being played, useful for timing chains of sound as well as making cooldown time.")]
        [Min(0f)] public float delayAfter = 0f;
        [Tooltip("This sets the volume, multiplicative of each of the volumes within the structures.")]
        [Min(0f)] public float Volume = 1.0f;
        [Tooltip("This sets the pitch, multiplicative of each of the pitches within the structures.")]
        [Min(0.1f)] public float Pitch = 1.0f;

        [HideInInspector] public float originalMaxVolume;
        [HideInInspector] public float originalPitch;
        [HideInInspector] public AudioSource source;
        [HideInInspector] public SoundCast[] soundCastSource;
        [HideInInspector] public SoundVisualizer[] soundVisualSource;

        [HideInInspector] public WaitForSeconds delayBeforeWait;
        [HideInInspector] public WaitWhile delayDuringWait;
        [HideInInspector] public WaitForSeconds delayAfterWait;

        public void ActivateTime()
        {
            foreach (SoundVisualizer soundVisualizer in soundVisualSource)
            {
                soundVisualizer.ApplyTime();
            }
        }
        public void ActivateSpecificTime(float time)
        {
            foreach (SoundVisualizer soundVisualizer in soundVisualSource)
            {
                soundVisualizer.ApplySpecificTime(time);
            }
        }
    }

}


[System.Serializable]
public class SoundVisualizer
{
    [Header("Vanilla Settings")]
    [Tooltip("Sets the defaulting canvas, is none is set, one will be created, its recommended that you allow one to be created if the existing canvas already has a lot of UI.")]
    public GameObject canvas;

    [Header("Sound Visualizer Settings")]
    [Tooltip("Sets the visualizer name.")]
    public string visualizerName;
    [Tooltip("Sets in the dynamic rotating part of the visual.")]
    public SoundVisualAssets[] VisualizerDynamicPart;
    [Tooltip("Sets in the static unrotating part of the visual.")]
    public SoundVisualAssets[] VisualizerStaticPart;

    [HideInInspector] public Vector3 LastKnownSoundPoint;

    public void ApplyTime()
    {
        foreach (SoundVisualAssets soundVisualAssets in VisualizerDynamicPart)
        {
            soundVisualAssets.currentTime = soundVisualAssets.ActiveTime;
        }
        foreach (SoundVisualAssets soundVisualAssets in VisualizerStaticPart)
        {
            soundVisualAssets.currentTime = soundVisualAssets.ActiveTime;
        }
    }
    public void ApplySpecificTime(float time)
    {
        foreach (SoundVisualAssets soundVisualAssets in VisualizerDynamicPart)
        {
            soundVisualAssets.currentTime = time;
        }
        foreach (SoundVisualAssets soundVisualAssets in VisualizerStaticPart)
        {
            soundVisualAssets.currentTime = time;
        }
    }
}

[System.Serializable]
public class SoundVisualAssets
{
    [Tooltip("Sets in the visual part of the visual.")]
    public Sprite VisualSprite;
    [Tooltip("How far should the X-cord visual be from the center.")]
    [Min(0f)] public float XVisualDistanceFromCenter;
    [Tooltip("How far should the Y-cord visual be from the center.")]
    [Min(0f)] public float YVisualDistanceFromCenter;
    [Tooltip("The X size of the UI element.")]
    [Min(0f)] public float XSize;
    [Tooltip("The Y size of the UI element.")]
    [Min(0f)] public float YSize;
    [Tooltip("How close you are to looking at the source before this would disappear. This value has to be between 0 to 1")]
    [Min(0f)] public float VisualAngle;
    [Tooltip("A buffer of visual angle to soften the transition of vanish. This value has to be between 0 to 1.")]
    [Min(0f)] public float VisualAngleBuffer;
    [Tooltip("How long should the solid object exist since the trigger.")]
    [Min(0f)] public float ActiveTime;
    [Tooltip("Buffer for when the object should start to slowly vanish.")]
    [Min(0f)] public float BufferTime;

    [HideInInspector] public GameObject ObjectImage;
    [HideInInspector] public RectTransform RectImage;
    [HideInInspector] public Image ComponentImage;
    public float currentTime;
}