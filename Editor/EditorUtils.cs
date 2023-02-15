using System;
using UnityEngine;
using UnityEditor;

public static class EditorUtils
{
    static private GUIStyle _horizontalLineStyle;
    static public readonly float LineHeightPadding = 2;
    static public readonly float LineHeight = EditorGUIUtility.singleLineHeight + LineHeightPadding;
    static public readonly Color darkGrey = new Color { r = 0.3f, g = 0.3f, b = 0.3f, a = 1 };

    static EditorUtils()
    {
        _horizontalLineStyle = new GUIStyle();
        _horizontalLineStyle.normal.background = EditorGUIUtility.whiteTexture;
        _horizontalLineStyle.margin = new RectOffset( 0, 0, 4, 4 );
        _horizontalLineStyle.fixedHeight = 1;
    }

    public static void PrefixedText(string label, ref string value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        value = EditorGUILayout.TextField(value);
        EditorGUILayout.EndHorizontal();
    }

    public static void PrefixedBool(string label, ref bool value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        value = EditorGUILayout.Toggle(value);
        EditorGUILayout.EndHorizontal();
    }

    public static void PrefixedSlider(string label, float min, float max, ref float value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        value = EditorGUILayout.Slider(value, min, max);
        EditorGUILayout.EndHorizontal();
    }

    public static void PrefixedMinMaxSlider(string label, ref MinMaxFloat minMaxFloat)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        minMaxFloat.low = EditorGUILayout.FloatField(minMaxFloat.low, GUILayout.MaxWidth(60));
        EditorGUILayout.MinMaxSlider(ref minMaxFloat.low, ref minMaxFloat.high, minMaxFloat.min, minMaxFloat.max);
        minMaxFloat.high = EditorGUILayout.FloatField(minMaxFloat.high, GUILayout.MaxWidth(60));
        EditorGUILayout.EndHorizontal();
    }

    public static void HorizontalLine()
    {
        HorizontalLine(darkGrey);
    }

    public static void HorizontalLine(Color color, GUIStyle style = null)
    {
        var cachedGUIColor = GUI.color;
        GUI.color = color;
        if (style != null)
        {
            GUILayout.Box(GUIContent.none, style);
        }
        else
        {
            GUILayout.Box(GUIContent.none, _horizontalLineStyle);
        }
        GUI.color = cachedGUIColor;
    }

    public static void HorizontalSection(Action content)
    {
        EditorGUILayout.BeginHorizontal();
        content();
        EditorGUILayout.EndHorizontal();
    }

    public static float LinesHeight(int lineCount)
    {
        return LineHeight * lineCount;
    }


    public static void Spaces(int spaceCount)
    {
        for(int i = 0; i < spaceCount; ++i)
        {
            EditorGUILayout.Space();
        }
    }

    public static void SectionTitle(string title)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        HorizontalLine();
    }

    public static SerializedProperty AppendProperty(ref Rect position, SerializedProperty property, string propertyRelativeName, float addedSpace = 0)
    {
        SerializedProperty relativeProperty = property.FindPropertyRelative(propertyRelativeName);
        Sound.Utils.AssertNotNull(relativeProperty, $"Unable to find propertyRelative '{propertyRelativeName}' in children of {property.propertyPath}");
        if(relativeProperty == null)
        {
            return null;
        }


        EditorGUI.PropertyField(NextLineRect(ref position), relativeProperty);
        position.y += EditorGUI.GetPropertyHeight(relativeProperty) - LineHeight + LineHeightPadding + addedSpace;
        return relativeProperty;
    }

    public static Rect NextLineRect(ref Rect position, SerializedProperty property = null)
    {
        return new Rect(
                    position.x,
                    position.y += LineHeight,
                    position.width,
                    LineHeight - LineHeightPadding);
    }

    public static Rect PlacePropertyInRect(Rect targetRect, SerializedProperty property, string propertyRelativeName)
    {
        SerializedProperty propertyRelative = property.FindPropertyRelative(propertyRelativeName);
        float propertyHeight = EditorGUI.GetPropertyHeight(propertyRelative);
        targetRect.height = propertyHeight;
        EditorGUI.PropertyField(targetRect, propertyRelative);

        // Return next rectangle
        return new Rect(
            targetRect.x,
            targetRect.y + propertyHeight,
            targetRect.width,
            0);
    }

    public delegate void ConfirmCancelDelegate();

    public static void CancelConfirmButton(string buttonText, ref bool confirmationRequest, ConfirmCancelDelegate action)
    {
        EditorGUILayout.BeginVertical();
        if (confirmationRequest)
        {
            if (GUILayout.Button("Cancel"))
            {
                confirmationRequest = false;
            }
            if (GUILayout.Button("Confirm"))
            {
                action.Invoke();
                confirmationRequest = false;
            }
        }
        else
        {
            if (GUILayout.Button(buttonText))
            {
                confirmationRequest = true;
            }
        }
        EditorGUILayout.EndVertical();
    }
}
