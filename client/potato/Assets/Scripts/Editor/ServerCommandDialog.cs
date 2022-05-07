using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ServerCommandDialog : EditorWindow
{
    [SerializeField] private int m_SelectedIndex = -1;

    private ListView logsPane;

    [MenuItem("Tools/ServerCommandDialog")]
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<ServerCommandDialog>();
        wnd.titleContent = new GUIContent("ServerCommandDialog");

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
            var rpc = Potato.RpcHolder.GetRpc<Potato.Diagnosis.Command.Rpc>();
            if (rpc == null)
            {
                allLogs.Add(new Log { Name = "RPC is not ready." });
                Debug.LogError("RPC is not ready.");
                return;
            }

            var splitCommand = commandTextField.text.Split(new string[]{ " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var request = new Potato.Diagnosis.Command.Request { Name = splitCommand.FirstOrDefault() };
            request.Arguments.AddRange(splitCommand.Skip(1));
            rpc.Request(request, (response) =>
            {
                allLogs.Add(new Log { Name = response.Result });
                logsPane.RefreshItems();
                logsPane.ScrollToItem(logsPane.itemsSource.Count - 1);
            });
        });
        commandSendButton.text = "Send Command";
        controlPane.Add(commandSendButton);


        logsPane = new ListView();
        splitView.Add(logsPane);

        logsPane.makeItem = () => new Label();
        logsPane.bindItem = (item, index) => { (item as Label).text = allLogs[index].Name; };
        logsPane.itemsSource = allLogs;
    }
}