using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BasicMovement : MonoBehaviour
{
    float deadZoneAmount = 0.5f;
    public float speed = 10;

    public bool debugMode = false;
    private XRNode leftControllerNode = XRNode.LeftHand;
    private List<UnityEngine.XR.InputDevice> leftInputDevices = new List<UnityEngine.XR.InputDevice>();

    private UnityEngine.XR.InputDevice leftController;
    private XRNode rightControllerNode = XRNode.RightHand;
    private List<UnityEngine.XR.InputDevice> rightInputDevices = new List<UnityEngine.XR.InputDevice>();
    private UnityEngine.XR.InputDevice rightController;

    private Gamepad gamepad;
    //private Rigidbody _rigidbody;
    public Rigidbody _rigidbody;

    //public GameObject Maze;

    bool controllerHeld = false, rotating = false;
    public static bool gameOver = false;

    /*void Awake(){
      _rigidbody = GetComponent<Rigidbody>();
    }*/

    void OnCollisionStay(Collision col) {
      Vector3 c = col.GetContact(0).point - this.gameObject.transform.position;
      Vector3 velocity = _rigidbody.velocity;
      Vector3 proj  = Vector3.Dot(c, velocity) / c.sqrMagnitude * c;
      Vector3 newVelocity = velocity - proj;
      Debug.Log(newVelocity); 
    }


    void Start() {
        //Lets get all the devices we can find.
        if(!debugMode){
          GetDevices();
        }else{
          gamepad = Gamepad.current;
          if(gamepad == null){
            Debug.LogError("No gamepad connected");
            return;
          }
          //this.gameObject.AddComponent<Rigidbody>();
        }
    }

    void Update() {
        if(!debugMode){
            if (leftController == null) {
                GetControllerDevices(leftControllerNode, ref leftController, ref leftInputDevices);
            }

            if (rightController == null) {
                GetControllerDevices(rightControllerNode, ref rightController, ref rightInputDevices);
            }

            CheckForChanges();

            if (!isHeld()) {
              _rigidbody.velocity = Vector2.zero;
            }

            if (!rotating) { //bad code, computer shouldnt check if it is rotating in each update but this is for testing
              _rigidbody.freezeRotation = true;
            } else {
              _rigidbody.freezeRotation = false;
            }
            var rotation = this.gameObject.transform.rotation;
            if (rotation.x != 0) {
              this.gameObject.transform.Rotate(new Vector3(-rotation.x, 0, 0));
            }
            if (rotation.z != 0) {
              this.gameObject.transform.Rotate(new Vector3(0, 0, -rotation.z));
            }
        }else{
          Vector2 leftStick = gamepad.leftStick.ReadValue();
          Vector2 rightStick = gamepad.rightStick.ReadValue();

          if(leftStick != Vector2.zero){
            if(leftStick.x < -deadZoneAmount){
              MoveRight(leftStick.x);
            }else if(leftStick.x > deadZoneAmount){
              MoveLeft(leftStick.x);
            }

            if (leftStick.y < -deadZoneAmount) {
                // touching bottom side, move backwards
                MoveForward(leftStick.y);
            } else if (leftStick.y > deadZoneAmount) {
                // touching top side, move forward
                MoveBackward(leftStick.y);
            }
          }


          if(rightStick != Vector2.zero){
            if(rightStick.x < -deadZoneAmount){
              RotateLeft(rightStick.x);
            }else if(rightStick.x > deadZoneAmount){
              RotateRight(rightStick.x);
            }

            if (rightStick.y < -deadZoneAmount) {
              MoveUp(rightStick.y);
            } else if (rightStick.y > deadZoneAmount) {
                // touching top side, move forward
                MoveDown(rightStick.y);
            }
          }

        }
    }

    //Here we need to add code to work
    void CheckForChanges() {
        Vector2 leftTouchCoords;
        Vector2 rightTouchCoords;
        bool triggerVal;

        if (!gameOver) {
          if (leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out leftTouchCoords) && leftTouchCoords != Vector2.zero)
          {
              if(leftTouchCoords.x < -deadZoneAmount){
                MoveLeft(leftTouchCoords.x);
              }else if(leftTouchCoords.x > deadZoneAmount){
                MoveRight(leftTouchCoords.x);
              }

              if (leftTouchCoords.y < -deadZoneAmount) {
                  MoveBackward(leftTouchCoords.y);
              } else if (leftTouchCoords.y > deadZoneAmount) {
                  MoveForward(leftTouchCoords.y);
              }
          }

          if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rightTouchCoords) && rightTouchCoords != Vector2.zero)
          { 
              if(rightTouchCoords.x < -deadZoneAmount){
                rotating = true;
                RotateLeft(rightTouchCoords.x);
              }else if(rightTouchCoords.x > deadZoneAmount){
                rotating = true;
                RotateRight(rightTouchCoords.x);
              } else {
                rotating = false;
              }

              if (rightTouchCoords.y < -deadZoneAmount) {
                MoveDown(rightTouchCoords.y);
              } else if (rightTouchCoords.y > deadZoneAmount) {
                MoveUp(rightTouchCoords.y);
              }
          }
        }

    }

    void MoveForward(float input){
      _rigidbody.velocity = transform.forward * 0.5f;
    }
    void MoveBackward(float input){
      _rigidbody.velocity = -transform.forward * 0.5f;
    }

    void MoveLeft(float input){
      _rigidbody.velocity = -transform.right * 0.5f;
    }

    void MoveRight(float input){
      _rigidbody.velocity = transform.right * 0.5f;
    }

    void MoveUp(float input){
      //Debug.Log(Vector3.up * input * Time.deltaTime * speed);
      MoveObject(Vector3.up * input * Time.deltaTime * speed);
    }

    void MoveDown(float input){
      Debug.Log(Vector3.down * input * Time.deltaTime * -speed);
      MoveObject(Vector3.down * input * Time.deltaTime * -speed);
    }

    void RotateLeft(float input){
      RotateObject(Vector3.up * input * Time.deltaTime * speed * 5);
    }

    void RotateRight(float input){
      RotateObject(Vector3.down * input * Time.deltaTime * -speed * 5);
    }
    
    void MoveObject(Vector3 translation){ //should use addforce because translate "teleports' the player which allows them to bypass walls
      //_rigidbody.AddRelativeForce(translation.x * 5, translation.y * 5, translation.z * 5);
      _rigidbody.velocity = new Vector3(translation.x * 5, translation.y * 5, translation.z * 5);
      /*if (!isHeld()) {
          StartCoroutine(stopObject());
      }*/
    }

    void RotateObject(Vector3 rotation){
      transform.Rotate(rotation);
    }

    void GetDevices() {
        //Gets the Right Controller Devices
        GetControllerDevices(leftControllerNode, ref leftController, ref leftInputDevices);

        //Gets the Right Controller Devices
        GetControllerDevices(rightControllerNode, ref rightController, ref rightInputDevices);


        Debug.Log(string.Format("Device name '{0}' with characteristics '{1}'", leftController.name, leftController.characteristics));

        Debug.Log(string.Format("Device name '{0}' with characteristics '{1}'", rightController.name, rightController.characteristics));

    }

    void GetControllerDevices(XRNode controllerNode, ref UnityEngine.XR.InputDevice controller,ref List<UnityEngine.XR.InputDevice> inputDevices) {
        Debug.Log("Get devices is called");
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(controllerNode, inputDevices);

        if (inputDevices.Count == 1){
            controller = inputDevices[0];
            Debug.Log(string.Format("Device name '{0}' with characteristics '{1}'", controller.name, controller.characteristics));
        }

        if (inputDevices.Count > 1) {
            Debug.LogAssertion("More than one device found with the same input characteristics");
        }
    }

    bool isHeld() {
        Vector2 leftTouchCoords;
        Vector2 rightTouchCoords;

        leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out leftTouchCoords);
        rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rightTouchCoords);

        if (leftTouchCoords != Vector2.zero || rightTouchCoords != Vector2.zero) {
          return true;
        }

        return false;
    }

}
