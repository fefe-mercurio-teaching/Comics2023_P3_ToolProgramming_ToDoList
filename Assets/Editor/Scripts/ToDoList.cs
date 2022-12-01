using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDoList : ScriptableObject
{
    [Serializable]
    public struct ToDoElement
    {
        public string content;
        public bool done;
    }

    [SerializeField] private List<ToDoElement> elements = new();

    public List<ToDoElement> GetElements() => elements;

    public void AddElement(string content) => elements.Add(new ToDoElement
    {
        content = content,
        done = false
    });

    public void SetValue(int index, bool newValue)
    {
        ToDoElement element = elements[index];
        element.done = newValue;
        elements[index] = element;
    }
}
