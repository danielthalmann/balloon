using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Topography))]
public class EditModeTopo : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh"))
        {
            ((Topography)this.target).generateMesh();
        }
   
    }
}
