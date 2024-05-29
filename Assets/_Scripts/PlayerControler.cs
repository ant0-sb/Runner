using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using LootLocker.Requests;
using UnityEngine.SceneManagement;


namespace TempleRun.Player {

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    private float initialPlayerSpeed = 8f;
    [SerializeField]
    private float maximumPlayerSpeed = 30f;
    [SerializeField]
    private float playerSpeedIncreaseRate = 0.4f;
    [SerializeField]
    private float jumpHeight = 2.0f;
    [SerializeField]
    private float initialGravityValue = -9.81f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask turnLayer;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip slideAnimationClip;
    [SerializeField]
    private float scoreMultiplier =10f;
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private GameObject mesh;
    [SerializeField]
    private Gun Gun;
    [SerializeField]
    private AudioSource jumpSound;
    [SerializeField]
    private Animator transition;
    

    private bool isGroundedAlreadyChecked = false;
    private float gravity;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;

    private PlayerInput playerInput;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction shootAction;

    private CharacterController controller;

    private int slidingAnimationId;

    private bool sliding = false;
    private float score = 0;
    


    [SerializeField]
    private UnityEvent<Vector3> turnEvent;
    [SerializeField]
    private UnityEvent<int> gameOverEvent;
    [SerializeField]
    private UnityEvent<int> scoreUpdateEvent;
    [SerializeField]
    private UnityEvent shootingEvent;
    [SerializeField]
    private UnityEvent<bool> runningEvent;
    [SerializeField]
    private UnityEvent slidingEvent;
    [SerializeField]
    private UnityEvent<bool> musicEvent;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        slidingAnimationId = Animator.StringToHash("Sliding");

        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
        shootAction = playerInput.actions["Shoot"];
    }

    private void OnEnable() {
        turnAction.performed += PlayerTurn;
        jumpAction.performed += PlayerJump;
        slideAction.performed += PlayerSlide;
        shootAction.performed += PlayerShoot;
    }

    private void OnDisable() {
        turnAction.performed -= PlayerTurn;
        jumpAction.performed -= PlayerJump;
        slideAction.performed -= PlayerSlide;
        shootAction.performed -= PlayerShoot;
    }

    private void Start() {
        playerSpeed = initialPlayerSpeed;
        gravity = initialGravityValue;
        StartCoroutine(ChangePlayerMaterial());
        musicEvent.Invoke(true);
    }

    private IEnumerator ChangePlayerMaterial() {
        string color = "";
        bool? playerSkinRequest = null;
        LootLockerSDKManager.GetSingleKeyPersistentStorage("skin", (response) => {
            if (response.success) {
                Debug.Log("successfully retrieved player storage");
                if (response.payload is not null) color = response.payload.value;
                else Debug.Log("player storage is null"); //normally not possible as there is the white basic skin
                playerSkinRequest = true;
            }else {
                Debug.Log("unsuccessfully retrieved player storage");
                playerSkinRequest = false;
            }
        });
        yield return new WaitUntil(() => playerSkinRequest.HasValue);
        if (playerSkinRequest.Value) {
            if (UnityEngine.ColorUtility.TryParseHtmlString(color, out Color newColor)) {
                mesh.GetComponent<MeshRenderer>().material.color = newColor;
            }
        }
    }
/// <summary>
///  Makes player and tiles turn
/// </summary>
/// <param name="context"></param>
    private void PlayerTurn(InputAction.CallbackContext context) {
        Vector3? turnPosition = CheckTurn(context.ReadValue<float>()); //returns 1 or -1 (axis value) depending of the key pressed
        if (!turnPosition.HasValue) {
            GameOver();
            return;
        }
        Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
        turnEvent.Invoke(targetDirection);
        Turn(context.ReadValue<float>(), turnPosition.Value);
    }

    private Vector3? CheckTurn(float turnValue) {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, 0.1f, turnLayer);
        if (hitCollider.Length != 0) {
            Tile tile = hitCollider[0].transform.parent.GetComponent<Tile>();
            TileType type = tile.type;
            if ((type == TileType.LEFT && turnValue == -1) 
            || (type == TileType.RIGHT && turnValue == 1)
            || (type == TileType.SIDEWAYS)) {
                return tile.pivot.position;
            }
        }
        return null;
    }

/// <summary>
/// Makes the player turn
/// </summary>
/// <param name="turnValue"></param>
/// <param name="turnPosition"></param>
    private void Turn(float turnValue, Vector3 turnPosition) {
        Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
        // we disable the character controller to move the player to the pivot point of the tile otherwise it doesn't work
        controller.enabled = false;
        transform.position = tempPlayerPosition;
        controller.enabled = true;

        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0,90 * turnValue,0);
        transform.rotation = targetRotation;
        movementDirection = transform.forward.normalized;
    }

    private void PlayerJump(InputAction.CallbackContext context) {
        if (IsGrounded()) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3.0f); 
            //SUVAT equation v^2 = vinit^2 + 2*acceleration*distance
            //-3 because gravity is negative
            controller.Move(playerVelocity * Time.deltaTime);
            runningEvent.Invoke(false);
            jumpSound.Play();
        }
    }

    private void PlayerSlide(InputAction.CallbackContext context) {
        if (!sliding && IsGrounded()) {
            // Coroutine allows to wait for the animation to finish before setting the collider back to normal
            StartCoroutine(Slide());
            slidingEvent.Invoke();
        }
    }

    private IEnumerator Slide() {
        sliding = true;

        //shrink the collider
        Vector3 originalControllerCenter = controller.center;
        Vector3 newControllerCenter = originalControllerCenter;
        controller.height /= 2;
        newControllerCenter.y -= controller.height / 2;
        controller.center = newControllerCenter;
        
        //play the sliding animation
        animator.Play(slidingAnimationId);
        yield return new WaitForSeconds(slideAnimationClip.length / animator.speed);

        //set the character controller collider back to normal after sliding
        controller.height *= 2;
        controller.center = originalControllerCenter;

        sliding = false;
    }

    private void PlayerShoot(InputAction.CallbackContext context) {
        Gun.Shoot();
        shootingEvent.Invoke();
    }

    private void Update() {
        // casting a long ray to check if the player is "grounded" even if the player is jumping
        if(!IsGrounded(20f) && !isGroundedAlreadyChecked){
            GameOver();
            isGroundedAlreadyChecked = true;
            return;
        }
        if(isGroundedAlreadyChecked) return; // stops the player from falling

        //Score functionality
        score += scoreMultiplier * Time.deltaTime;
        scoreUpdateEvent.Invoke((int)score);
        controller.Move(transform.forward * playerSpeed * Time.deltaTime);

        if (IsGrounded() && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
            runningEvent.Invoke(true);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (playerSpeed < maximumPlayerSpeed) {
            playerSpeed += playerSpeedIncreaseRate * Time.deltaTime;
            gravity = initialGravityValue - playerSpeed; //increases the gravity as the player speed increases to make the jump more realistic
        }

        if (animator.speed < 1.25f){
            animator.speed += (1/playerSpeed)*Time.deltaTime; //increases the animation speed as the player speed increases to make the game more challenging
        }
    }

    private bool IsGrounded(float length = 0.2f) {
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= controller.height / 2f;
        raycastOriginFirst.y += 0.1f; // adding a little offset to the raycast origin so that it doesn't accidently go through the ground

        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst -= transform.forward * 0.2f;
        raycastOriginSecond += transform.forward * 0.2f;

        //Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green, 2f);
        //Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red, 2f);

        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) 
        || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer)) {
            return true;
        } else {
            return false;
        }
    }

    private void GameOver(){
        Debug.Log("game over");
        PlayerPrefs.SetInt("Score", (int)score);
        SceneManager.LoadScene("Leaderboard");
        musicEvent.Invoke(false);
        gameObject.SetActive(false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit){
        if(((1<<hit.collider.gameObject.layer) & obstacleLayer)!=0) GameOver();  
    }
}

}