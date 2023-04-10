using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenQuestPointer : MonoBehaviour
{
    GameObject Camera;
    [SerializeField] ScreenPointerVisualizer[] screenPointerVisualizers;

    void soundVisualUIInitialization()
    {
        int VisualizerCounter = 0;
        foreach (ScreenPointerVisualizer soundVisualizer in screenPointerVisualizers)
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
            foreach (ScreenQuestAssets soundVisualAssets in soundVisualizer.VisualizerDynamicPart)
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
            foreach (ScreenQuestAssets soundVisualAssets in soundVisualizer.VisualizerStaticPart)
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

    private void Awake()
    {
        soundVisualUIInitialization();
    }

    void Update()
    {
        if (Camera == null)
        {
            Camera = FindObjectOfType<AudioListener>().gameObject;
        }
        Vector3 soundDirection = (transform.position - Camera.transform.position).normalized;
        float DifferenciationY = Vector3.Dot(soundDirection, Camera.transform.right);
        float DifferenciationX = Vector3.Dot(soundDirection, Camera.transform.forward);
        float DifferenciationZ = Vector3.Dot(soundDirection, Camera.transform.up);
        foreach (ScreenPointerVisualizer screenPointerVisualizer in screenPointerVisualizers)
        {
            CalculateVisualPositioning(screenPointerVisualizer, DifferenciationX, DifferenciationY, DifferenciationZ);
        }
    }
    private void CalculateVisualPositioning(ScreenPointerVisualizer screenPointerVisualizers, float fowardBack, float leftRight, float upDown) //Sets a normalized Vector2 for the positional direction the sprite should be within the Canvas screen, after that, a multiplier for how far they should be from the center.
    {
        Vector2 EncircleVector = new Vector2(leftRight, upDown).normalized;
        foreach (ScreenQuestAssets soundVisualAssets in screenPointerVisualizers.VisualizerDynamicPart)
        {
            //soundVisualAssets.currentTime -= Time.deltaTime;
            Vector2 RealVector = new Vector2(EncircleVector.x * soundVisualAssets.XVisualDistanceFromCenter, EncircleVector.y * soundVisualAssets.YVisualDistanceFromCenter);
            soundVisualAssets.RectImage.localPosition = RealVector;
            soundVisualAssets.ObjectImage.transform.up = RealVector;

            float a = Mathf.Min(Mathf.Max(1 - fowardBack - soundVisualAssets.VisualAngle, 0) / soundVisualAssets.VisualAngleBuffer, 1); //Calculate Visibility of UI Based on how close you are looking at them.
            Color changeVisibility = soundVisualAssets.ComponentImage.color;
            changeVisibility.a = a;
            soundVisualAssets.ComponentImage.color = changeVisibility;

        }
        foreach (ScreenQuestAssets soundVisualAssets in screenPointerVisualizers.VisualizerStaticPart)
        {
            //soundVisualAssets.currentTime -= Time.deltaTime;
            Vector2 RealVector = new Vector2(EncircleVector.x * soundVisualAssets.XVisualDistanceFromCenter, EncircleVector.y * soundVisualAssets.YVisualDistanceFromCenter);
            soundVisualAssets.RectImage.localPosition = RealVector;
            soundVisualAssets.ObjectImage.transform.localPosition = RealVector;

            float a = Mathf.Min(Mathf.Max(1 - fowardBack - soundVisualAssets.VisualAngle, 0) / soundVisualAssets.VisualAngleBuffer, 1); //Calculate Visibility of UI Based on how close you are looking at them.
            Color changeVisibility = soundVisualAssets.ComponentImage.color;
            changeVisibility.a = a;
            soundVisualAssets.ComponentImage.color = changeVisibility;

        }
    }
}


[System.Serializable]
public class ScreenPointerVisualizer
{
    [Header("Vanilla Settings")]
    [Tooltip("Sets the defaulting canvas, is none is set, one will be created, its recommended that you allow one to be created if the existing canvas already has a lot of UI.")]
    public GameObject canvas;

    [Header("Sound Visualizer Settings")]
    [Tooltip("Sets the visualizer name.")]
    public string visualizerName;
    [Tooltip("Sets in the dynamic rotating part of the visual.")]
    public ScreenQuestAssets[] VisualizerDynamicPart;
    [Tooltip("Sets in the static unrotating part of the visual.")]
    public ScreenQuestAssets[] VisualizerStaticPart;

}

[System.Serializable]
public class ScreenQuestAssets
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

    [HideInInspector] public GameObject ObjectImage;
    [HideInInspector] public RectTransform RectImage;
    [HideInInspector] public Image ComponentImage;

}