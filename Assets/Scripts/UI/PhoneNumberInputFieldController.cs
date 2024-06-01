using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class PhoneNumberInputFieldController : MonoBehaviour
{
    public  TMP_InputField inputField;

    private void Start()
    {
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }

        inputField.characterLimit = 10; // Set a limit to the number of characters
        inputField.onValueChanged.AddListener(OnInputFieldChanged);
    }

    public void OnInputFieldChanged(string text)
    {
        // Remove non-numeric characters
        string numericText = new string(text.Where(char.IsDigit).ToArray());

        // Ensure the text is exactly 10 characters
        if (numericText.Length > 10)
        {
            inputField.text = numericText.Substring(0, 10); // Trim the text if it exceeds 10 characters
        }
        else if (numericText.Length < 10)
        {
            inputField.text = numericText; // Allow less than 10 characters, no padding needed
        }
        else
        {
            inputField.text = numericText;
        }
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(OnInputFieldChanged);
    }
}
