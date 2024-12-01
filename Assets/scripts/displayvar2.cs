using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;

public class displayvar2 : MonoBehaviour
{
    [Header("Inspector Inputs")]
    
    public string gameObjectName; // Name of the GameObject
    public string scriptTypeName; // Name of the script to find
    public string variableName;   // Variable to access
    


    [Header("UI References")]
    public Text outputText;       // UI Text to display the variable

    private Component targetScript; // Reference to the target script

    void Start()
    {
        // Find the target GameObject by name
        GameObject targetObject = GameObject.Find(gameObjectName);
        if (targetObject == null)
        {
            Debug.LogError($"GameObject with name '{gameObjectName}' not found.");
            return;
        }

        // Get the specified script/component from the GameObject
        Type scriptType = Type.GetType(scriptTypeName);
        if (scriptType == null)
        {
            Debug.LogError($"Script type '{scriptTypeName}' not found. Ensure it is fully qualified if in a namespace.");
            return;
        }

        targetScript = targetObject.GetComponent(scriptType);
        if (targetScript == null)
        {
            Debug.LogError($"Script '{scriptTypeName}' not found on GameObject '{gameObjectName}'.");
        }
    }

    void Update()
    {
        if (targetScript == null || string.IsNullOrEmpty(variableName) || outputText == null)
            return;

        // Use reflection to get the variable value
        var variableValue = GetVariableValue(targetScript, variableName);
        if (variableValue != null)
        {
            outputText.text = variableName + ": " + variableValue.ToString();
        }
        else
        {
            outputText.text = $"Variable '{variableName}' not found.";
        }
    }

    private object GetVariableValue(Component script, string varName)
    {
        // Use reflection to get the variable's value
        var fieldInfo = script.GetType().GetField(varName);
        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(script);
        }

        var propertyInfo = script.GetType().GetProperty(varName);
        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(script);
        }

        return null; // Variable not found
    }
}
