using MainSpace.Activities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MainSpace.ScriptableObject
{

    [CreateAssetMenu]
    public class CommanderConfig : ActivityConfig
    {

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CommanderConfig), true)]
    public class CommanderConfigInspectorEditor : ActivityConfigEditor
    {
        public CommanderConfig editorTarget;
        private void OnEnable()
        {
            OnInit();
            editorTarget = target as CommanderConfig;
        }

        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();

        //if (GUI.changed)
        //{
        //    EditorUtility.SetDirty(target);
        //}
    //}
}

#endif


}