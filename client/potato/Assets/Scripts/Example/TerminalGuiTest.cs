using UnityEngine;
using NStack;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using Terminal.Gui;

public static class KeyCodeExtension
{
    public static ConsoleKey ToConsoleKey(this KeyCode keyCode) => keyCode switch
    {
        KeyCode.Escape => ConsoleKey.Escape,
        KeyCode.Backspace => ConsoleKey.Backspace,
        KeyCode.Tab => ConsoleKey.Tab,
        KeyCode.Clear => ConsoleKey.Clear,
        KeyCode.Return => ConsoleKey.Enter,
        KeyCode.Pause => ConsoleKey.Pause,
        KeyCode.LeftArrow => ConsoleKey.LeftArrow,
        KeyCode.RightArrow => ConsoleKey.RightArrow,
        KeyCode.UpArrow => ConsoleKey.UpArrow,
        KeyCode.DownArrow => ConsoleKey.DownArrow,
        KeyCode.Space => ConsoleKey.Spacebar,
        KeyCode.A => ConsoleKey.A,
        KeyCode.B => ConsoleKey.B,
        KeyCode.C => ConsoleKey.C,
        KeyCode.D => ConsoleKey.D,
        KeyCode.E => ConsoleKey.E,
        KeyCode.F => ConsoleKey.F,
        KeyCode.G => ConsoleKey.G,
        KeyCode.H => ConsoleKey.H,
        KeyCode.I => ConsoleKey.I,
        KeyCode.J => ConsoleKey.J,
        KeyCode.K => ConsoleKey.K,
        KeyCode.L => ConsoleKey.L,
        KeyCode.M => ConsoleKey.M,
        KeyCode.N => ConsoleKey.N,
        KeyCode.O => ConsoleKey.O,
        KeyCode.P => ConsoleKey.P,
        KeyCode.Q => ConsoleKey.Q,
        KeyCode.R => ConsoleKey.R,
        KeyCode.S => ConsoleKey.S,
        KeyCode.T => ConsoleKey.T,
        KeyCode.U => ConsoleKey.U,
        KeyCode.V => ConsoleKey.V,
        KeyCode.W => ConsoleKey.W,
        KeyCode.X => ConsoleKey.X,
        KeyCode.Y => ConsoleKey.Y,
        KeyCode.Z => ConsoleKey.Z,
        KeyCode.F1 => ConsoleKey.F1,
        KeyCode.F2 => ConsoleKey.F2,
        KeyCode.F3 => ConsoleKey.F3,
        KeyCode.F4 => ConsoleKey.F4,
        KeyCode.F5 => ConsoleKey.F5,
        KeyCode.F6 => ConsoleKey.F6,
        KeyCode.F7 => ConsoleKey.F7,
        KeyCode.F8 => ConsoleKey.F8,
        KeyCode.F9 => ConsoleKey.F9,
        KeyCode.F10 => ConsoleKey.F10,
        KeyCode.F11 => ConsoleKey.F11,
        KeyCode.F12 => ConsoleKey.F12,
        _ => throw new InvalidOperationException()
    };
}

public class TerminalGuiTest : MonoBehaviour
{
    public Canvas canvas;
    public EventSystem eventSystem;

    public GameObject charForeground;
    public GameObject charBackground;


    private UnityDriver UnityDriver = new UnityDriver();
    private UnityMainLoop UnityMainLoop = new UnityMainLoop();

    private UniTask applicationTask;
    private CancellationTokenSource applicationCancellationTokenSource = new CancellationTokenSource();

    private CharForeground[,] foregrounds;
    private CharBackground[,] backgrounds;

    private bool ignoreDirtyFlag = true;

    private int[,,] diff;

    // Start is called before the first frame update
    void Start()
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        }

        UnityDriver.UnityConsole = new UnityConsole();
        Terminal.Gui.Application.Init(UnityDriver, UnityMainLoop);
        Terminal.Gui.Application.HeightAsBuffer = true;

        //Button[] buttons = new Button[] {
        //    new Button(ustring.Make("OK")),
        //    new Button(ustring.Make("Cancel")),
        //};
        //var dialog = new Dialog(ustring.Make("Authentication"), 50, 10, buttons);
        //Terminal.Gui.Application.Top.Add(dialog);

        //var login = new Label("Login: ") { X = 3, Y = 6 };
        //var password = new Label("Password: ")
        //{
        //    X = Pos.Left(login),
        //    Y = Pos.Bottom(login) + 1
        //};
        //var loginText = new TextField("")
        //{
        //    X = Pos.Right(password),
        //    Y = Pos.Top(login),
        //    Width = 40
        //};

        //var passText = new TextField("")
        //{
        //    Secret = true,
        //    X = Pos.Left(loginText),
        //    Y = Pos.Top(password),
        //    Width = Dim.Width(loginText)
        //};

        //dialog.Add(login, loginText, password, passText);

        //buttons[0].Clicked += () => {
        //    Debug.Log("Click OK");
        //};
        //buttons[0].MouseClick += (e) => {
        //    Debug.Log($"Click OK {e.MouseEvent}");
        //};

        //buttons[1].Clicked += () => {
        //    Debug.Log("Click Cancel");
        //};

        //dialog.KeyDown += (k) => {
        //    Debug.Log(k);
        //};

        {
            var top = Terminal.Gui.Application.Top;

            // Creates the top-level window to show
            var win = new Window("MyApp")
            {
                X = 0,
                Y = 1, // Leave one row for the toplevel menu

                // By using Dim.Fill(), it will automatically resize without manual intervention
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            top.Add(win);

            // Creates a menubar, the item "New" has a help menu.
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_New", "Creates new file", null),
                    new MenuItem ("_Close", "",null),
                    new MenuItem ("_Quit", "", () => { if (Quit ()) top.Running = false; })
                }),
                new MenuBarItem ("_Edit", new MenuItem [] {
                    new MenuItem ("_Copy", "", null),
                    new MenuItem ("C_ut", "", null),
                    new MenuItem ("_Paste", "", null)
                })
            });
            top.Add(menu);

            static bool Quit()
            {
                var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
                return n == 0;
            }

            var login = new Label("Login: ") { X = 3, Y = 2 };
            var password = new Label("Password: ")
            {
                X = Pos.Left(login),
                Y = Pos.Top(login) + 1
            };
            var loginText = new TextField("")
            {
                X = Pos.Right(password),
                Y = Pos.Top(login),
                Width = 40
            };
            var passText = new TextField("")
            {
                Secret = true,
                X = Pos.Left(loginText),
                Y = Pos.Top(password),
                Width = Dim.Width(loginText)
            };

            // Add some controls,
            win.Add(
                // The ones with my favorite layout system, Computed
                login, password, loginText, passText,

                // The ones laid out like an australopithecus, with Absolute positions:
                new CheckBox(3, 6, "Remember me"),
                new RadioGroup(3, 8, new ustring[] { "_Personal", "_Company" }, 0),
                new Button(3, 14, "Ok"),
                new Button(10, 14, "Cancel"),
                new Label(3, 18, "Press F9 or ESC plus 9 to activate the menubar")
            );
        }

        diff = new int[UnityDriver.Rows, UnityDriver.Cols, 2];
        foregrounds = new CharForeground[UnityDriver.Rows, UnityDriver.Cols];
        backgrounds = new CharBackground[UnityDriver.Rows, UnityDriver.Cols];
        for (int y = 0; y < UnityDriver.Rows; y++)
        {
            for (int x = 0; x < UnityDriver.Cols; x++)
            {
                backgrounds[y, x] = Instantiate(charBackground, canvas.transform).GetComponent<CharBackground>();
                backgrounds[y, x].transform.localPosition = new Vector3(-1920 / 2 + x * (1920 / 120), 1080 - (1080 / 2 + y * (1080 / 30)));
                backgrounds[y, x].Color = UnityEngine.Color.black;
                foregrounds[y, x] = Instantiate(charForeground, canvas.transform).GetComponent<CharForeground>();
                foregrounds[y, x].transform.localPosition = new Vector3(-1920 / 2 + x * (1920 / 120), 1080 - (1080 / 2 + y * (1080 / 30)));
                foregrounds[y, x].Character = '*';
                diff[y, x, 0] = '*';
                diff[y, x, 1] = 0;
            }
        }

        applicationTask = UniTask.RunOnThreadPool(() =>
        {
            try
            {
                Terminal.Gui.Application.Run();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }, true, applicationCancellationTokenSource.Token).ContinueWith(()=> {
            UnityDriver.SetWindowSize(UnityDriver.Cols, UnityDriver.Rows);
        });
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button1Pressed,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            //Debug.Log(mouseEvent);

            UnityDriver.mouseHandler(mouseEvent);
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button2Pressed,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            //Debug.Log(mouseEvent);

            UnityDriver.mouseHandler(mouseEvent);
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(true);
        }
    }

    public void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button1Released | MouseFlags.Button1Clicked,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            //Debug.Log(mouseEvent);

            UnityDriver.mouseHandler(mouseEvent);
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button2Released | MouseFlags.Button2Clicked,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            //Debug.Log(mouseEvent);

            UnityDriver.mouseHandler(mouseEvent);
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(true);
        }
    }

    private void OnDestroy()
    {
        applicationCancellationTokenSource.Cancel();
        Terminal.Gui.Application.Shutdown();
        applicationTask.Forget();
    }

    // Update is called once per frame
    void Update()
    {
        var v = UnityDriver.ContentsView;
        for (int y = 0; y < UnityDriver.Rows; y++)
        {
            for (int x = 0; x < UnityDriver.Cols; x++)
            {
                if (v[y, x, 0] == diff[y, x, 0] && v[y, x, 1] == diff[y, x, 1])
                {
                    continue;
                }

                diff[y, x, 0] = (char)v[y, x, 0];
                diff[y, x, 1] = v[y, x, 1];

                foregrounds[y, x].Character = (char)v[y, x, 0];
                UnityDriver.GetColors(v[y, x, 1], out Terminal.Gui.Color fc, out Terminal.Gui.Color bc);
                foregrounds[y, x].Color = UnityDriver.colorMap2[fc];
                backgrounds[y, x].Color = UnityDriver.colorMap2[bc];
            }
        }

        if (UnityDriver.GetCursorVisibility(out CursorVisibility cv))
        {
            if (cv != CursorVisibility.Invisible && ((int)(Time.realtimeSinceStartup * 2) & 1) == 0)
            {
                backgrounds[UnityDriver.UnityConsole.CursorTop, UnityDriver.UnityConsole.CursorLeft].Color = UnityEngine.Color.black;
            }
            else
            {
                UnityDriver.GetColors(v[UnityDriver.UnityConsole.CursorTop, UnityDriver.UnityConsole.CursorLeft, 1], out Terminal.Gui.Color _, out Terminal.Gui.Color bc);
                backgrounds[UnityDriver.UnityConsole.CursorTop, UnityDriver.UnityConsole.CursorLeft].Color = UnityDriver.colorMap2[bc];
            }
        }

        var inputString = Input.inputString;
        foreach (var keyCode in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
        {
            if (Input.GetKeyDown(keyCode))
            {
                try
                {
                    var consoleKey = keyCode.ToConsoleKey();
                    Terminal.Gui.Application.Driver.SendKeys(inputString.FirstOrDefault(), consoleKey, Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftAlt), Input.GetKey(KeyCode.LeftControl));
                    (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(true);
                }
                catch (Exception e)
                {
                    //Debug.LogException(e);
                }
            }
        }
    }
}
