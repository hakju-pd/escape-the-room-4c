using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using StarterAssets;

public class PasswordPad : MonoBehaviour
{
    [SerializeField] private string correctPassword = "123"; // 3-digit password (digits 1-9)
    [SerializeField] private TMPro.TextMeshProUGUI displayText; // Text to show entered digits
    [SerializeField] private TMPro.TextMeshProUGUI messageText; // Text to show error/success messages
    [SerializeField] private Button[] numberButtons = new Button[9]; // Buttons 1-9
    [SerializeField] private Button clearButton;
    [SerializeField] private Button enterButton;
    [SerializeField] private float wrongPasswordDisplayTime = 2f;

    [SerializeField] public GameObject exitDoor; // Reference to the exit door GameObject

    [SerializeField] public GameObject passwordPadUI; // Reference to the Password Pad UI GameObject

    private string enteredPassword = "";
    private const int MAX_PASSWORD_LENGTH = 3;
    private StarterAssetsInputs playerInputs;

    void Start()
    {

        // be inactive at start
        passwordPadUI.SetActive(false);
        messageText.text = "";

        // Setup number buttons (1-9)
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int digit = i + 1; // Digits 1-9 (index 0 = digit 1, index 8 = digit 9)
            if (numberButtons[i] != null)
            {
                numberButtons[i].onClick.AddListener(() => OnNumberButtonPressed(digit));
            }
        }

        // Setup clear button
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearPassword);
        }

        // Setup enter button
        if (enterButton != null)
        {
            enterButton.onClick.AddListener(CheckPassword);
        }

        UpdateDisplay();
    }

    private void OnNumberButtonPressed(int digit)
    {
        if (enteredPassword.Length < MAX_PASSWORD_LENGTH)
        {
            enteredPassword += digit.ToString();
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (displayText != null)
        {
            // Display asterisks or dots instead of actual digits for security
            displayText.text = new string('*', enteredPassword.Length);

            // Alternatively, display the actual digits:
            // displayText.text = enteredPassword;
        }
    }

    private void ClearPassword()
    {
        enteredPassword = "";
        if (messageText != null)
        {
            messageText.text = "";
        }
        UpdateDisplay();
    }

    private void CheckPassword()
    {
        if (enteredPassword == correctPassword)
        {
            OnPasswordCorrect();
        }
        else
        {
            OnPasswordWrong();
        }
    }

    private void OnPasswordCorrect()
    {
        if (messageText != null)
        {
            messageText.text = "Correct! Door Unlocked!";
            messageText.color = Color.green;
        }

        Debug.Log("Password correct! Door Unlocked!");

        // Exit the game
    }

    private void OnPasswordWrong()
    {
        if (messageText != null)
        {
            messageText.text = "Wrong Password";
            messageText.color = Color.red;
        }

        ClearPassword();
    }

    // Optional: Allow entering password via keyboard (digits 1-9 only)
    void Update()
    {
        // set to active when player close to gameobject exitDoor
        var player = GameObject.FindGameObjectWithTag("Player");
        if (transform.position != null && exitDoor != null)
        {
            float distance = Vector3.Distance(player.transform.position, exitDoor.transform.position);
            bool shouldBeActive = distance < 2f; // arbitrary interaction distance

            if (shouldBeActive != passwordPadUI.activeSelf)
            {
                passwordPadUI.SetActive(shouldBeActive);

                // Control cursor and camera input based on password pad state
                if (playerInputs != null)
                {
                    if (shouldBeActive)
                    {
                        // Password pad is active - unlock cursor and disable camera input
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        playerInputs.cursorInputForLook = false;
                    }
                    else
                    {
                        // Password pad is inactive - lock cursor and enable camera input
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        playerInputs.cursorInputForLook = true;
                    }
                }
            }
        }


        if (Input.inputString.Length > 0)
        {
            char c = Input.inputString[0];

            // Only accept digits 1-9 (not 0)
            if (c >= '1' && c <= '9' && enteredPassword.Length < MAX_PASSWORD_LENGTH)
            {
                enteredPassword += c;
                UpdateDisplay();
            }
            else if (c == '\b') // Backspace
            {
                if (enteredPassword.Length > 0)
                {
                    enteredPassword = enteredPassword.Substring(0, enteredPassword.Length - 1);
                    UpdateDisplay();
                }
            }
            else if (c == '\r' || c == '\n') // Enter key
            {
                CheckPassword();
            }
        }
    }
}
