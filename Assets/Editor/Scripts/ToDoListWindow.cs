using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class ToDoListWindow : EditorWindow
{
    private const string ToDoListPath = "Assets/Editor/Data/ToDoList.asset";
    
    public VisualTreeAsset windowStructureAsset;

    private TextField _toDoTextField;
    private ScrollView _toDoList;
    private ProgressBar _progressBar;
    private ToDoList _toDoListScriptableObject;
    
    [MenuItem("Tools/ToDo List")]
    public static void OpenWindow()
    {
        GetWindow<ToDoListWindow>("ToDo List");
    }

    private void CreateGUI()
    {
        rootVisualElement.Add(windowStructureAsset.Instantiate());

        Button addToDoButton = rootVisualElement.Q<Button>("addToDoButton");
        addToDoButton.clicked += AddToDoButtonClicked;

        _toDoTextField = rootVisualElement.Q<TextField>("toDoTextField");
        _toDoTextField.RegisterCallback<KeyDownEvent>(ToDoTextFieldKeyDown);
        
        _toDoList = rootVisualElement.Q<ScrollView>("toDoList");

        _progressBar = rootVisualElement.Q<ProgressBar>("progressBar");
        
        // Leggere i dati dallo Scriptable Object
        _toDoListScriptableObject = AssetDatabase.LoadAssetAtPath<ToDoList>(ToDoListPath);
        if (_toDoListScriptableObject == null)
        {
            _toDoListScriptableObject = CreateInstance<ToDoList>();
            AssetDatabase.CreateAsset(_toDoListScriptableObject, ToDoListPath);
        }

        // Inserire i dati della lista nella Scroll View
        UpdateScrollView();
        
        // Aggiornare la barra di progresso
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        List<ToDoList.ToDoElement> elements = _toDoListScriptableObject.GetElements();
        
        _progressBar.highValue = elements.Count;
        _progressBar.value = elements.Count(e => e.done);

        float percent = _progressBar.value / _progressBar.highValue * 100f;

        _progressBar.title = $"{percent:F0}%";
    }

    private void UpdateScrollView()
    {
        _toDoList.Clear();

        int index = 0;
        foreach (ToDoList.ToDoElement element in _toDoListScriptableObject.GetElements())
        {
            Toggle toggle = new Toggle
            {
                text = element.content,
                value = element.done,
                userData = index
            };
            toggle.AddToClassList("todoElement");
            toggle.RegisterCallback<ChangeEvent<bool>>(ToggleChangeEvent);

            _toDoList.Add(toggle);
            index++;
        }
    }

    private void ToggleChangeEvent(ChangeEvent<bool> e)
    {
        Toggle toggle = (Toggle)e.target;
        int index = (int)toggle.userData;
        
        _toDoListScriptableObject.SetValue(index, e.newValue);
        EditorUtility.SetDirty(_toDoListScriptableObject);
        
        UpdateProgressBar();
    }

    private void ToDoTextFieldKeyDown(KeyDownEvent e)
    {
        if (e.keyCode == KeyCode.Return)
        {
            AddToDoButtonClicked();
        }
    }

    private void AddToDoButtonClicked()
    {
        string newToDo = _toDoTextField.value.Trim();

        if (string.IsNullOrWhiteSpace(newToDo)) return;
        
        // Aggiungere l'elemento allo Scriptable Object
        _toDoListScriptableObject.AddElement(newToDo);
        EditorUtility.SetDirty(_toDoListScriptableObject);

        // Aggiornare la Scroll View
        UpdateScrollView();
        
        UpdateProgressBar();
        
        _toDoTextField.value = "";
        _toDoTextField.Focus();
    }
}
