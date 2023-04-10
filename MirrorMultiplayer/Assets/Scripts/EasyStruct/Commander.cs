using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using MyBox;
using Mirror;

/*!!!Notes: 
 * Script Version (1.4V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V     -     : - Basic introduction CreateObject, InvokeFuction, LoadScene
 * 1.1V : - ChangeTMPText,RigidBodyAddRelativeForce, RigidBodyAddVelocity
 * 1.2V : - QuitGame
 * 1.3V : - ChangeTimeScale, ChangeAudioGroupDecibelBySlider
 * 1.4V : - Dragged enum and Order class into the Component. Added random Order Function, the switch case and isn't functions are now in the Order class instead
 * 1.5V : - Now includes Mirror to server and server to all client call
 */

public class Commander : NetworkBehaviour
{
    public enum OrderTypes
    {
        CreateObject,
        InvokeFunction,
        LoadScene,
        QuitGame,
        ChangeTMPText,
        RigidBodyAddRelativeForce,
        RigidBodyAddVelocity,
        ChangeTimeScale,
        ChangeAudioGroupDecibelBySlider
    }

    [System.Serializable]
    public class Order
    {
        [SearchableEnum]
        [SerializeField]
        public OrderTypes OrderType;

        [Space]

        [ConditionalField("OrderType", false, OrderTypes.CreateObject)]
        public GameObject CreatingObject;
        [ConditionalField("OrderType", false, OrderTypes.CreateObject)]
        public Transform TargetTransform;

        [ConditionalField("OrderType", false, OrderTypes.InvokeFunction)]
        public UnityEvent Functions;

        [ConditionalField("OrderType", false, OrderTypes.LoadScene)]
        public SceneReference Scene;

        [ConditionalField("OrderType", false, OrderTypes.ChangeTMPText)]
        public TextMeshProUGUI TextMesh;
        [ConditionalField("OrderType", false, OrderTypes.ChangeTMPText)]
        [TextArea]
        public string ChangeTo;

        [ConditionalField("OrderType", false, OrderTypes.RigidBodyAddRelativeForce)]
        public Rigidbody Rigidbody;
        [ConditionalField("OrderType", false, OrderTypes.RigidBodyAddRelativeForce)]
        public Vector3 Force;

        [ConditionalField("OrderType", false, OrderTypes.RigidBodyAddVelocity)]
        public Rigidbody RigidbodyVelocity;
        [ConditionalField("OrderType", false, OrderTypes.RigidBodyAddVelocity)]
        public Vector3 Velocity;

        [ConditionalField("OrderType", false, OrderTypes.ChangeTimeScale)]
        public float TargetTimeScale;

        [ConditionalField("OrderType", false, OrderTypes.ChangeAudioGroupDecibelBySlider)]
        public Slider TargetSlider;
        [ConditionalField("OrderType", false, OrderTypes.ChangeAudioGroupDecibelBySlider)]
        public AudioMixer TargetAudioMixer;
        [ConditionalField("OrderType", false, OrderTypes.ChangeAudioGroupDecibelBySlider)]
        public string TargetAudioMixerPerameterName;

        public void CallOrder()
        {
            switch (OrderType)
            {
                case OrderTypes.CreateObject:
                    GameObject Go = Instantiate(CreatingObject, TargetTransform.position, new Quaternion(0, 0, 0, 0));
                    Go.transform.rotation = TargetTransform.rotation;
                    break;
                case OrderTypes.InvokeFunction:
                    Functions.Invoke();
                    break;
                case OrderTypes.LoadScene:
                    Scene.LoadScene();
                    break;
                case OrderTypes.QuitGame:
                    Application.Quit();
                    break;
                case OrderTypes.ChangeTMPText:
                    TextMesh.text = ChangeTo;
                    break;
                case OrderTypes.RigidBodyAddRelativeForce:
                    Rigidbody.AddRelativeForce(Force);
                    break;
                case OrderTypes.RigidBodyAddVelocity:
                    RigidbodyVelocity.velocity += Velocity;
                    break;
                case OrderTypes.ChangeTimeScale:
                    Time.timeScale = TargetTimeScale;
                    break;
                case OrderTypes.ChangeAudioGroupDecibelBySlider:
                    TargetAudioMixer.SetFloat(TargetAudioMixerPerameterName, Mathf.Log10(TargetSlider.value) * 20);
                    break;
                default:
                    Debug.Log("Unknown Order: " + OrderType);
                    break;
            }
        }
    }

    [SerializeField]
    List<Order> Orders = new List<Order>();

    public void CallOrder(int OrderID)
    {
        if (isServer) ServerToClients(OrderID);
        else if (isClient) ClientToServerToClients(OrderID);
        else Orders[OrderID].CallOrder();
    }

    [ClientRpc]
    private void ServerToClients(int OrderID)
    {
        Orders[OrderID].CallOrder();
    }

    [Command]
    private void ClientToServerToClients(int OrderID)
    {
        ServerToClients(OrderID);
    }
}
