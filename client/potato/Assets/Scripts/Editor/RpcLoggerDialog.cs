using Google.Protobuf.Reflection;
using Potato;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RpcLoggerDialog : EditorWindow
{
    private ListView logsPane;

    [MenuItem("Tools/RpcLoggerDialog")]
    public static void ShowRpcLoggerDialog()
    {
        EditorWindow wnd = GetWindow<RpcLoggerDialog>();
        wnd.titleContent = new GUIContent("RpcLoggerDialog");

        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    private class Log
    {
        public string Name;
    }

    public void CreateGUI()
    {
        var allLogs = new List<Log>();
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Vertical);

        rootVisualElement.Add(splitView);

        var controlPane = new ScrollView(ScrollViewMode.Vertical);
        splitView.Add(controlPane);
        var commandTextField = new TextField();
        controlPane.Add(commandTextField);
        var commandSendButton = new Button(() => {
            logsPane.itemsSource.Clear();
            logsPane.RefreshItems();
        });
        commandSendButton.text = "Clear";
        controlPane.Add(commandSendButton);

        Example example = FindObjectOfType<Example>();
        if (example)
        {
            example.OnReceiveMessage = (message => {
                MessageDescriptor desc = message.Descriptor;
                string info = $"<< {desc.FullName}\n";
                foreach (var field in desc.Fields.InDeclarationOrder())
                {
                    info += $"   {field.Name} = {field.Accessor.GetValue(message)}\n";
                }
                logsPane.itemsSource.Add(new Log { Name = info });
                logsPane.RefreshItems();
                logsPane.ScrollToItem(logsPane.itemsSource.Count - 1);
            });
        }

        var networkService = FindObjectOfType<Potato.Network.NetworkService>();
        example.OnRpcReady += () =>
        {
            Potato.RpcHolder.SubscribeRequest((message) =>
            {
                MessageDescriptor desc = message.Descriptor;
                string info = $">> {desc.FullName}\n";
                foreach (var field in desc.Fields.InDeclarationOrder())
                {
                    info += $"   {field.Name} = {field.Accessor.GetValue(message)}\n";
                }
                logsPane.itemsSource.Add(new Log { Name = info });
                logsPane.RefreshItems();
                logsPane.ScrollToItem(logsPane.itemsSource.Count - 1);
            });
        };

        logsPane = new ListView();
        logsPane.horizontalScrollingEnabled = true;
        splitView.Add(logsPane);

        logsPane.makeItem = () => new Label();
        logsPane.bindItem = (item, index) => { (item as Label).text = allLogs[index].Name; };
        logsPane.itemsSource = allLogs;
        logsPane.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
    }
}