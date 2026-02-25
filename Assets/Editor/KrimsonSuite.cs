using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Networking;
using Unity.EditorCoroutines;
using Unity.EditorCoroutines.Editor;

public class KrimsonSuite : EditorWindow
{
    private EventContainer _container;
    private SerializedObject _serializedObj;
    private SerializedProperty _eventProp;

    private string selectedMatName = "selectedMat";

    private Vector3 pos;

    public enum Materials
    {
        Red,
        Green,
        Blue,
        Yellow,
        Pink,
        Orange,
        Purple,
        Violet,
        White,
        Black,
        Metal,
        Ceramic,
        MatteRed,
        MatteGreen,
        MatteBlue,
        MatteYellow,
        MattePink,
        MatteOrange,
        MattePurple,
        MatteViolet,
        MatteWhite,
        MatteBlack,
    }

    public struct MaterialData
    {
        public Color Color;
        public float Metallic;
        public float Smoothness;
    }

    private Dictionary<Materials, MaterialData> materialLookup = new()
    {
        {
            Materials.Red,
            new MaterialData { Color = Color.red, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Green,
            new MaterialData { Color = Color.green, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Blue,
            new MaterialData { Color = Color.blue, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Yellow,
            new MaterialData { Color = Color.yellow, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Pink,
            new MaterialData { Color = Color.pink, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Orange,
            new MaterialData { Color = Color.orange, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Purple,
            new MaterialData { Color = Color.purple, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Violet,
            new MaterialData { Color = Color.violet, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.White,
            new MaterialData { Color = Color.white, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Black,
            new MaterialData { Color = Color.black, Metallic = 0f, Smoothness = 0.5f }
        },
        {
            Materials.Metal,
            new MaterialData { Color = Color.white, Metallic = 1f, Smoothness = 0.5f }
        },
        {
            Materials.Ceramic,
            new MaterialData { Color = Color.white, Metallic = 0f, Smoothness = 1f }
        },
        {
            Materials.MatteRed,
            new MaterialData { Color = Color.red, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MatteGreen,
            new MaterialData { Color = Color.green, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MatteBlue,
            new MaterialData { Color = Color.blue, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MatteYellow,
            new MaterialData { Color = Color.yellow, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MattePink,
            new MaterialData { Color = Color.pink, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MatteOrange,
            new MaterialData { Color = Color.orange, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MattePurple,
            new MaterialData { Color = Color.purple, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MatteViolet,
            new MaterialData { Color = Color.violet, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MatteWhite,
            new MaterialData { Color = Color.white, Metallic = 0f, Smoothness = 0 }
        },
        {
            Materials.MatteBlack,
            new MaterialData { Color = Color.black, Metallic = 0f, Smoothness = 0 }
        },
    };

    private string selectedColour;

    private Font robotoBlack;
    Texture headerTexture;
    private Material defaultMat;
    Texture materialTexture;

    [MenuItem("Krimson Suite/Main")]
    public static void ShowWindow()
    {
        GetWindow<KrimsonSuite>("Main");
    }

    void OnEnable()
    {
        _container = CreateInstance<EventContainer>();
        _serializedObj = new SerializedObject(_container);
        _eventProp = _serializedObj.FindProperty("functionToTrigger");
        robotoBlack = Resources.Load<Font>("FlatSkin/Font/Roboto-Black");
        headerTexture = Resources.Load("Icons/SettingsIcons") as Texture;
        defaultMat = GetDefaultLit();
    }

    private void CreateGUI()
    {
        this.StartCoroutine(GetTexture());
    }

    void OnGUI()
    {
        if (_serializedObj.targetObject!=null)
            _serializedObj.Update();
        GUIStyle richStyle = new GUIStyle(GUI.skin.label)
        {
            richText = true
        };
        GUIStyle richButtonStyle = new GUIStyle(EditorStyles.popup)
        {
            richText = true
        };
        GUIStyle borderLabel = new GUIStyle(EditorStyles.label)
        {
            font = robotoBlack,
            fontSize = 14,
            fontStyle = FontStyle.Italic
        };
        GUIStyle windowTitle = new GUIStyle(EditorStyles.boldLabel)
        {
            font = robotoBlack,
            fontSize = 20,
        };
        windowTitle.normal.textColor = Color.black;
        windowTitle.hover.textColor = Color.gray1;

        if (headerTexture != null)
        {
            GUI.DrawTexture(new Rect(5, 5, 400, 60), headerTexture, ScaleMode.StretchToFill);
        }

        GUILayout.Label("Krimson Suite", windowTitle);
        GUILayout.Space(60);

        GUILayout.Label("Ezy Function Test", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_eventProp);
        if (_serializedObj.targetObject!=null)
            _serializedObj.ApplyModifiedProperties();
        SetToEditorAndRuntime();

        if (GUILayout.Button("Trigger Function"))
        {
            _container.functionToTrigger.Invoke();
        }

        GUILayout.Space(30);

        GUILayout.Label("Quick Material Apply", richStyle);

        if (Selection.activeGameObject != null)
        {
            HandleUtility.Repaint();
            materialTexture = (Texture)EditorGUILayout.ObjectField(
                "Material Texture",
                materialTexture,
                typeof(Texture)
            );

            GenericMenu menu = new GenericMenu();
            var enumValues = (Materials[])Enum.GetValues(typeof(Materials));
            foreach (var value in enumValues)
            {
                menu.AddItem(new GUIContent($"{value}"), false, HandleItemClicked, value);
            }

            if (EditorGUILayout.DropdownButton(new GUIContent("<color=#" + selectedColour + ">" + selectedMatName + "</color>"), FocusType.Keyboard,
                    richButtonStyle))
            {
                menu.ShowAsContext();
            }
        }
        else
        {
            HandleUtility.Repaint();
            GUILayout.Label("Select object for material options", borderLabel);
        }

        GUILayout.Space(20);

        GUILayout.Label("Primitives spawner", richStyle);

        GUILayout.BeginHorizontal();
        GUILayout.Space(40);
        GUILayout.Label("x");
        pos.x = float.Parse(GUILayout.TextField("" + pos.x));
        GUILayout.Space(40);
        GUILayout.Label("y");
        pos.y = float.Parse(GUILayout.TextField("" + pos.y));
        GUILayout.Space(40);
        GUILayout.Label("z");
        pos.z = float.Parse(GUILayout.TextField("" + pos.z));
        GUILayout.Space(40);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cube"))
        {
            Selection.activeGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Selection.activeGameObject.transform.position = pos;
        }

        if (GUILayout.Button("Sphere"))
        {
            Selection.activeGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Selection.activeGameObject.transform.position = pos;
        }

        if (GUILayout.Button("Capsule"))
        {
            Selection.activeGameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Selection.activeGameObject.transform.position = pos;
        }

        if (GUILayout.Button("Cylinder"))
        {
            Selection.activeGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Selection.activeGameObject.transform.position = pos;
        }

        if (GUILayout.Button("Plane"))
        {
            Selection.activeGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Selection.activeGameObject.transform.position = pos;
        }

        if (GUILayout.Button("Quad"))
        {
            Selection.activeGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Selection.activeGameObject.transform.position = pos;
        }

        GUILayout.EndHorizontal();
        if (GUILayout.Button("Delete Object"))
        {
            Undo.DestroyObjectImmediate(Selection.activeGameObject);
        }

        GUILayout.Space(20);
        GUILayout.Label("Path Tool", borderLabel);
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.GetComponent<PathFollow>() != null)
            {
                Handles.DrawDottedLine(Selection.activeTransform.position, new Vector3(5, 5, 5), 4);
            }
            else
            {
                if (GUILayout.Button("Add Path script"))
                {
                    Selection.activeGameObject.AddComponent<PathFollow>();
                }
            }
        }
        else
        {
            GUILayout.Label("Select an object for settings", richStyle);
        }

        GUILayout.Space(40);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Undo"))
        {
            Undo.PerformUndo();
        }

        GUILayout.Space(40);
        if (GUILayout.Button("Redo"))
        {
            Undo.PerformRedo();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Close"))
        {
            Close();
        }
    }

    private class EventContainer : ScriptableObject
    {
        public UnityEvent functionToTrigger;
    }

    void SetToEditorAndRuntime()
    {
        SerializedProperty calls = _eventProp.FindPropertyRelative("m_PersistentCalls.m_Calls");

        if (calls != null)
        {
            for (int i = 0; i < calls.arraySize; i++)
            {
                SerializedProperty call = calls.GetArrayElementAtIndex(i);
                SerializedProperty mode = call.FindPropertyRelative("m_CallState");
                if (mode.intValue != 1)
                {
                    mode.intValue = 1;
                }
            }
        }

        if (_serializedObj.targetObject!=null)
            _serializedObj.ApplyModifiedProperties();
    }

    private void HandleItemClicked(object userData)
    {
        if (Selection.activeTransform.gameObject.GetComponent<Renderer>().sharedMaterial != defaultMat)
        {
            DestroyImmediate(Selection.activeTransform.gameObject.GetComponent<Renderer>().sharedMaterial, true);
        }
        else
        {
            Debug.Log("Skipping over default mat");
        }

        Materials mat = (Materials)userData;
        Selection.activeTransform.gameObject.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Selection.activeTransform.gameObject.GetComponent<Renderer>().sharedMaterial.color = materialLookup[mat].Color;
        selectedColour = ColorUtility.ToHtmlStringRGB(materialLookup[mat].Color);
        Selection.activeTransform.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_Metallic", materialLookup[mat].Metallic);
        Selection.activeTransform.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_Smoothness", materialLookup[mat].Smoothness);

        if (materialTexture != null)
        {
            Selection.activeTransform.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = materialTexture;
        }

        selectedMatName = mat.ToString();
    }

    IEnumerator GetTexture()
    {
        Debug.Log("loaded image at: " + DateTime.Now);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(
            "https://upload.wikimedia.org/wikipedia/commons/2/2e/Lipetsk_banner_Lipetsk_Administration_Building.png");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            headerTexture = DownloadHandlerTexture.GetContent(www);
        }
    }

    Material GetDefaultLit()
    {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
        primitive.active = false;
        Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
        DestroyImmediate(primitive);
        return diffuse;
    }
}