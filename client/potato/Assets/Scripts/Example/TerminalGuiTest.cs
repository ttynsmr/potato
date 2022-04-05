using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Terminal.Gui;
using NStack;

public class TerminalGuiTest : MonoBehaviour
{
    public Text text;

    private UnityDriver UnityDriver = new UnityDriver();
    private UnityMainLoop UnityMainLoop = new UnityMainLoop();

    // Start is called before the first frame update
    void Start()
    {
        UnityDriver.UnityConsole = new UnityConsole();
        Terminal.Gui.Application.Init(UnityDriver, UnityMainLoop);

        Terminal.Gui.Button[] btn = new Terminal.Gui.Button[1] { new Terminal.Gui.Button(ustring.Make("OK")) };
        var dialog = new Dialog(ustring.Make("Potato test"), 10, 10, btn);
        Terminal.Gui.Application.Top.Add(dialog);
    }

    // Update is called once per frame
    void Update()
    {
        Terminal.Gui.Application.RunLoop(Terminal.Gui.Application.Begin(Terminal.Gui.Application.Top), false);
    }
}
