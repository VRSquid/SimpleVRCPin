using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GU_LockEditor : EditorWindow
{
	private GU_Lock[] keyPads { get { return FindObjectsOfType<GU_Lock>(); } }
	private string code;
	private Vector2 scrollPos;
	
	[MenuItem("Simple Keypad/Edit code")]
	public static void Window(){
		GetWindow<GU_LockEditor>("Change Password");
	}

	private void OnGUI(){
		GUILayout.BeginVertical();
		
		GUILayout.Label("List of menus in scene");
		GUILayout.BeginVertical("Box");
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		foreach (var key in keyPads)
		{
			GUILayout.BeginHorizontal("Box");
            key.globalUpdate = EditorGUILayout.Toggle(key.globalUpdate, GUILayout.Width(20));
			GUILayout.Label(string.Format("Name: {0}", key.gameObject.name));
			if (GUILayout.Button("Select", GUILayout.Width(100)))
			{
				Selection.activeGameObject = key.gameObject;
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		
		GUILayout.BeginVertical("Box");
		GUILayout.Label("Enter a new code bellow to change the password, then click the \"Set Password\" button");
		GUILayout.EndVertical();
		
		EditorGUILayout.Space();

		code = EditorGUILayout.TextField(code);
		
		EditorGUILayout.Space();
		
		if (GUILayout.Button("Set Password"))
		{
			foreach (var key in keyPads.Where(k=>k.globalUpdate == true))
			{
                foreach (GameObject button in key.buttons)
                {
                    foreach (GU_LockButton lockTrigger in Resources.FindObjectsOfTypeAll<GU_LockButton>().Where(b=>ReferenceEquals(b.transform.parent.gameObject, button)))
                    {
                        DestroyImmediate(lockTrigger.gameObject);
                    }
                }

				key.setNewCode(code);
				EditorUtility.SetDirty(key);
			}
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
		
		GUILayout.EndVertical();
	}
}
