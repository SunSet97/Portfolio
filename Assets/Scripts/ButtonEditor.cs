using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControlRobot))]
public class ButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ControlRobot RobotControl= (ControlRobot)target;

        EditorGUILayout.BeginHorizontal();//이거 이후로 GUI들이 가로로 생성
        GUILayout.FlexibleSpace();//고정된 여백 넣는다 버튼이 가운데로 오기위해

        if(GUILayout.Button("Attack",GUILayout.Width(120),GUILayout.Height(30)))
        {
            RobotControl._attackTrigger();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();//이거 이후로 GUI들이 가로로 생성
        GUILayout.FlexibleSpace();//고정된 여백 넣는다 버튼이 가운데로 오기위해
        
        if(GUILayout.Button("Power Up",GUILayout.Width(120),GUILayout.Height(30)))
        {
            RobotControl.Power=true;
            RobotControl.PowerUp();
            
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

    }
    
   
}
