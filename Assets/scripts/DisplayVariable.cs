using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class DisplayVariable : MonoBehaviour
{
    public MonoBehaviour refrence; // Reference to the script containing the variable;
    public Text displayText; // UI Text element to display the information



    void Update()
    {
        // Set variableName to the GameObject's name
        string variableName = gameObject.name;

        // Ensure there is a script and displayText is not null
        if (refrence != null && !string.IsNullOrEmpty(variableName) && displayText != null)
        {
            // Use reflection to get the value of the variable
            FieldInfo fieldInfo = refrence.GetType().GetField(variableName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                object value = fieldInfo.GetValue(refrence);

                // Update the display text with the variable name and value
                displayText.text = variableName + ": " + (value != null ? value.ToString() : "null");
            }
        }
    }
}