/*using UnityEngine;
using UnityEditor;

public class HelloWorldWindow : EditorWindow
{
    private bool show;
    private int num;
    private string colurS;

    private int maxNum;
    private string maxNumCol;

    private int credits =99;
    [MenuItem("TEST/TEST")]
    public static void ShowWindow()
    {
        GetWindow<HelloWorldWindow>("Test Window");
    }

    void OnGUI()
    {
        colurS = "<color=white>";
        GUIStyle richStyle = new GUIStyle(GUI.skin.label);
        richStyle.richText = true;
        
        GUILayout.Label("This is a test window!!!", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("And this text is small text", EditorStyles.label);
        GUILayout.Space(20);
        GUILayout.Label("You have $" +credits + " editor credits.", EditorStyles.label);

        if (credits > 0)
        {
            if (GUILayout.Button("Gamble ($1)"))
            {
                num = Random.Range(0, 100);
                credits--;
            }

            if (num >= 25 && num < 50)
            {
                colurS = "<color=lightblue>";
            }
            if (num >= 50 && num < 75)
            {
                colurS = "<color=green>";
            }
            if (num >= 75 && num < 100)
            {
                colurS = "<color=yellow>";
            }
            GUILayout.Label("You Got: "+ colurS + num + "</color>", richStyle);
            if (num > maxNum)
            {
                maxNum = num;
                maxNumCol = colurS;
            }
        }
        
        GUILayout.Label("Record: "+ maxNumCol + maxNum + "</color>", richStyle);
        GUILayout.Space(40);
        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}*/