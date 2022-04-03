using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//[CustomPropertyDrawer(typeof(WaveSpawner.Wave))]
public class WavePropertyDrawer : PropertyDrawer
{
    int selected = 0;
    SerializedProperty waveDelay, spawnInterval, waitForPrevious, enemiesTypes;
    string[] enemiesTypesNames = System.Enum.GetNames(typeof(WaveSpawner.EnemyTypesEnum));
    List<int> numOfEnemies = new List<int>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight =  EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
        if (selected != 0)
        {
            return baseHeight;
        }
        else
        {
            return baseHeight + EditorGUIUtility.singleLineHeight * enemiesTypesNames.Length + EditorGUIUtility.standardVerticalSpacing * enemiesTypesNames.Length;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // populates properties
        waveDelay = property.FindPropertyRelative(nameof(WaveSpawner.Wave.waveDelay));
        spawnInterval = property.FindPropertyRelative(nameof(WaveSpawner.Wave.spawnInterval));
        waitForPrevious = property.FindPropertyRelative(nameof(WaveSpawner.Wave.waitForPrevious));
        enemiesTypes = property.FindPropertyRelative(nameof(WaveSpawner.Wave.enemiesTypes));

        float padding = EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.BeginProperty(position, label, property);

        position.height /= selected == 0 ? 7 : 4;
        position.height -= padding;
        EditorGUI.PropertyField(position, waveDelay);

        position.y += EditorGUIUtility.singleLineHeight + padding;
        EditorGUI.PropertyField(position, spawnInterval);

        position.y += EditorGUIUtility.singleLineHeight + padding;
        EditorGUI.PropertyField(position, waitForPrevious);

        position.y += EditorGUIUtility.singleLineHeight + padding;
        GUIContent[] options = new GUIContent[enemiesTypesNames.Length + 2];
        options[0] = new GUIContent("Mixed", null, "choose how many from each enemy type");
        options[1] = new GUIContent("customized", null, "choose the enemy type and order");
        for (int i = 0; i < enemiesTypesNames.Length; i++)
        {
            options[i + 2] = new GUIContent(enemiesTypesNames[i], null, null);
        }
        selected = EditorGUI.Popup(position, new GUIContent("Enemy Type", null, "mixed - choose nuber from each type, customized - choose order and type"), selected, options);

        if (selected == 0)
        {
            for (int i = 0; i < enemiesTypesNames.Length; i++)
            {
                position.y += EditorGUIUtility.singleLineHeight + padding;
                if (numOfEnemies.Count <= i)
                {
                    numOfEnemies.Add(0);
                }
                numOfEnemies[i] = EditorGUI.IntField(position, enemiesTypesNames[i], numOfEnemies[i]);
            }

            enemiesTypes.ClearArray();
            for (int i = 0; i < enemiesTypesNames.Length; i++)
            {
                for (int j = 0; j < numOfEnemies[i]; j++)
                {
                    int end = enemiesTypes.arraySize;
                    enemiesTypes.InsertArrayElementAtIndex(end);
                    var temp = enemiesTypes.GetArrayElementAtIndex(end);
                    temp.intValue = i;
                }
            }
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
