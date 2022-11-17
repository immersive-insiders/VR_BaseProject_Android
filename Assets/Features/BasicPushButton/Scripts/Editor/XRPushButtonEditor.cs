using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;

[CustomEditor(typeof(XRPushButton))]
[CanEditMultipleObjects]
public class XRPushButtonEditor : XRBaseInteractableEditor
{
    SerializedProperty onPushed;

    protected override void OnEnable()
    {
        base.OnEnable();
        onPushed = serializedObject.FindProperty("OnPushed");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(onPushed);
        serializedObject.ApplyModifiedProperties();
    }
}
