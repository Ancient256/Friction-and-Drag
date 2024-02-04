using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private bool canMove = true;
    private float frictionMultiplier = 1f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    CharacterController characterController;
    private bool isSpeedBoosted = false;
    private bool isJumpBoosted = false;
    private bool isMudSlowed = false;
    private Vector3 checkpointPosition;

    // Life System
    public int maxLives = 3;
    private int currentLives;
    public TextMeshProUGUI livesText;

    // Countdown variables
    private float countdownTime = 2f;
    private bool isCountingDown = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        checkpointPosition = transform.position;
        currentLives = maxLives;
        UpdateLivesText();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();

        if (isCountingDown)
        {
            countdownTime -= Time.deltaTime;

            if (countdownTime <= 0)
            {
                SwitchToVictoryScene();
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isSpeedBoosted && hit.gameObject.CompareTag("SpeedBoost"))
        {
            ApplySpeedBoost(5f);
            isSpeedBoosted = true;
        }

        if (!isJumpBoosted && hit.gameObject.CompareTag("JumpPad"))
        {
            ApplyJumpBoost(10f, 5f);
            isJumpBoosted = true;
        }

        if (hit.gameObject.CompareTag("Mud") && !isMudSlowed)
        {
            ApplyMudSlowdown();
            isMudSlowed = true;
        }

        if (hit.gameObject.CompareTag("Checkpoint"))
        {
            SetCheckpoint(hit.transform.position);
        }

        if (hit.gameObject.CompareTag("Lava"))
        {
            RespawnAtCheckpoint();
            DeductLife();
        }

        if (hit.gameObject.CompareTag("Win") && !isCountingDown)
        {
            SwitchToVictoryScene();
        }
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        curSpeedX *= frictionMultiplier;
        curSpeedY *= frictionMultiplier;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (isMudSlowed)
        {
            StartCoroutine(ResetMudSlowdown(1f));
        }
    }

    private void HandleRotation()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    private void ApplyMudSlowdown()
    {
        frictionMultiplier = 0.5f;
    }

    private IEnumerator ResetMudSlowdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        frictionMultiplier = 1f;
        isMudSlowed = false;
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void ApplyFriction(float frictionAmount)
    {
        frictionMultiplier = Mathf.Clamp01(frictionMultiplier - frictionAmount);
    }

    public void ResetFriction()
    {
        frictionMultiplier = 1f;
    }

    private void ApplySpeedBoost(float boostAmount)
    {
        walkSpeed += boostAmount;
        runSpeed += boostAmount;

        StartCoroutine(ResetSpeedBoost(5f));
    }

    private IEnumerator ResetSpeedBoost(float duration)
    {
        yield return new WaitForSeconds(duration);
        walkSpeed = 6f;
        runSpeed = 12f;
        isSpeedBoosted = false;
    }

    private void ApplyJumpBoost(float jumpHeight, float duration)
    {
        jumpPower += jumpHeight;
        StartCoroutine(ResetJumpBoost(duration));
    }

    private IEnumerator ResetJumpBoost(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpPower = 7f;
        isJumpBoosted = false;
    }

    private void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
    }

    private void RespawnAtCheckpoint()
    {
        characterController.enabled = false;
        transform.position = new Vector3(checkpointPosition.x, checkpointPosition.y + 1f, checkpointPosition.z);
        characterController.enabled = true;
    }

    private void DeductLife()
    {
        currentLives--;
        UpdateLivesText();

        if (currentLives < 0)
        {
            SwitchToGameOverScene();
        }
    }

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + currentLives;
        }
    }

    private void StartCountdown()
    {
        isCountingDown = true;
    }

    private void SwitchToVictoryScene()
    {
        SceneManager.LoadScene("VictoryScene");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
