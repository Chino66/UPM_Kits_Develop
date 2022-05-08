using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UPMKits;

public class DataBindingExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}

public class DataBindingExampleWindow : EditorWindow
{
    [MenuItem("Tools/Data Binding Example")]
    public static void ShowWindow()
    {
        var window = GetWindow<DataBindingExampleWindow>();
        window.minSize = new Vector2(800, 520);
        window.titleContent = new GUIContent("Data Binding Example Window");
        window.Show();
    }

    private void OnEnable()
    {
        var root = new VisualElement();

        var lab = new Label();
        root.Add(lab);
        lab.text = "sss";

        var data = new PureData();
        var db = new DataBinding(data);
        lab.text = db.ToString();
        rootVisualElement.Add(root);

        var exa = new Exa();
        Type t = typeof(Exa);
        var method = t.GetMethod("Method");
    }
}

public class Exa
{
    public string Method()
    {
        return "Method()";
    }
}

public class PureData
{
    public string StringValue;
    public int IntValue;
    public bool BoolValue;
    private string psv;
    public string StringProperty { get; set; }
}

public class DataBinding
{
    private object data;

    private Dictionary<string, FieldInfo> _fieldInfos;

    public DataBinding(object data)
    {
        this.data = data;
        _fieldInfos = new Dictionary<string, FieldInfo>();
    }

    private void _process()
    {
        var fields = data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var fieldInfo in fields)
        {
            _fieldInfos.Add(fieldInfo.Name, fieldInfo);
            
        }
        
        var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // foreach (var property in properties)
        // {
        //     property.GetAccessors()[0].a
        // }
        
    }

    public override string ToString()
    {
        var content = new StringBuilder();

        content.Append("Type:").Append(data.GetType()).Append("\n");

        var members = data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var member in members)
        {
            content.Append(" member:").Append(member.Name).Append("\n");
        }

        return content.ToString();
    }
}