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
        KeyCode.PageDown => ConsoleKey.PageDown,
        KeyCode.PageUp => ConsoleKey.PageUp,
        KeyCode.Home => ConsoleKey.Home,
        KeyCode.End => ConsoleKey.End,
        KeyCode.Insert => ConsoleKey.Insert,
        KeyCode.Delete => ConsoleKey.Delete,
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

    private CharBackground Caret;

    private bool ignoreDirtyFlag = true;

    private int[,,] diff;

    public Vector3 GetTerminalCursorPosition(int x, int y)
    {
        return new Vector3(
            -canvas.pixelRect.width / 2 + x * (canvas.pixelRect.width / 120),
            canvas.pixelRect.height - (canvas.pixelRect.height / 2 + y * (1080 / 30))
            );
    }

    void StartApp()
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

    void StartApp2()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        }

        //applicationTask = UniTask.RunOnThreadPool(() =>
        //{
        //    try
        //    {
        //        Terminal.Gui.Application.Run();
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogException(e);
        //    }
        //}, true, applicationCancellationTokenSource.Token).ContinueWith(() => {
        //    UnityDriver.SetWindowSize(UnityDriver.Cols, UnityDriver.Rows);
        //});
        applicationTask = UniTask.RunOnThreadPool(() =>
        {
            try
            {
                UICatalog.UICatalogApp.Main(()=> {
                    var c = UniTask.RunOnThreadPool(async () =>
                    {

                        await UniTask.SwitchToMainThread();
                        UnityDriver.UnityConsole = new UnityConsole();
                        Terminal.Gui.Application.Init(UnityDriver, UnityMainLoop);
                        Terminal.Gui.Application.HeightAsBuffer = true;

                        if (foregrounds != null)
                        {
                            for (int y = 0; y < UnityDriver.Rows; y++)
                            {
                                for (int x = 0; x < UnityDriver.Cols; x++)
                                {
                                    GameObject.Destroy(backgrounds[y, x].gameObject);
                                    GameObject.Destroy(foregrounds[y, x].gameObject);
                                }
                            }
                        }
                        if (Caret)
                        {
                            GameObject.Destroy(Caret.gameObject);
                        }

                        diff = new int[UnityDriver.Rows, UnityDriver.Cols, 2];
                        foregrounds = new CharForeground[UnityDriver.Rows, UnityDriver.Cols];
                        backgrounds = new CharBackground[UnityDriver.Rows, UnityDriver.Cols];
                        for (int y = 0; y < UnityDriver.Rows; y++)
                        {
                            for (int x = 0; x < UnityDriver.Cols; x++)
                            {
                                backgrounds[y, x] = Instantiate(charBackground, canvas.transform).GetComponent<CharBackground>();
                                backgrounds[y, x].transform.localPosition = GetTerminalCursorPosition(x, y);
                                backgrounds[y, x].Color = UnityEngine.Color.black;
                            }
                        }
                        for (int y = 0; y < UnityDriver.Rows; y++)
                        {
                            for (int x = 0; x < UnityDriver.Cols; x++)
                            {
                                foregrounds[y, x] = Instantiate(charForeground, canvas.transform).GetComponent<CharForeground>();
                                foregrounds[y, x].transform.localPosition = GetTerminalCursorPosition(x, y);
                                foregrounds[y, x].Character = '*';
                                diff[y, x, 0] = '*';
                                diff[y, x, 1] = 0;
                            }
                        }
                        Caret = Instantiate(charBackground, canvas.transform).GetComponent<CharBackground>();
                        Caret.transform.localPosition = GetTerminalCursorPosition(0, 0);
                        Caret.Color = UnityEngine.Color.magenta;
                        await UniTask.SwitchToThreadPool();
                    });
                    bool wait = true;
                    c.AsTask().Wait();
                    });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }, true, applicationCancellationTokenSource.Token).ContinueWith(() => {
            UnityDriver.SetWindowSize(UnityDriver.Cols, UnityDriver.Rows);
        });
    }

    public void OnMouseDown()
    {
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        MouseFlags metas = (shift ? MouseFlags.ButtonShift : 0) | (alt ? MouseFlags.ButtonAlt : 0) | (ctrl ? MouseFlags.ButtonCtrl : 0);

        if (Input.GetMouseButtonDown(0))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button1Pressed | metas,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            //Debug.Log(mouseEvent);

            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() => { UnityDriver.mouseHandler(mouseEvent); });
        }
        if (Input.GetMouseButtonDown(1))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button2Pressed | metas,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            //Debug.Log(mouseEvent);

            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(()=> { UnityDriver.mouseHandler(mouseEvent); });
        }
    }

    public void OnMouseDrag()
    {
        var mouseEvent = new MouseEvent
        {
            Flags = MouseFlags.Button2Pressed | MouseFlags.ReportMousePosition,
            OfX = 0,
            OfY = 0,
            View = null,
            X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
            Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
        };

        (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() => { UnityDriver.mouseHandler(mouseEvent); });
    }

    public void OnMouseUp()
    {
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        MouseFlags metas = (shift ? MouseFlags.ButtonShift : 0) | (alt ? MouseFlags.ButtonAlt : 0) | (ctrl ? MouseFlags.ButtonCtrl : 0);

        if (Input.GetMouseButtonUp(0))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button1Released | metas,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() => { UnityDriver.mouseHandler(mouseEvent); });

            mouseEvent.Flags = MouseFlags.Button1Clicked | metas;
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() => { UnityDriver.mouseHandler(mouseEvent); });
        }
        if (Input.GetMouseButtonUp(1))
        {
            var mouseEvent = new MouseEvent
            {
                Flags = MouseFlags.Button2Released | metas,
                OfX = 0,
                OfY = 0,
                View = null,
                X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
            };
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() => { UnityDriver.mouseHandler(mouseEvent); });

            mouseEvent.Flags = MouseFlags.Button2Clicked | metas;
            (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() => { UnityDriver.mouseHandler(mouseEvent); });
        }
    }

    private void OnDestroy()
    {
        applicationCancellationTokenSource.Cancel();
        Terminal.Gui.Application.Shutdown();
        applicationTask.Forget();
    }

    float lastInputTime = Time.realtimeSinceStartup;
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
            if (cv != CursorVisibility.Invisible && ((int)((Time.realtimeSinceStartup - lastInputTime) * 2) & 1) == 0)
            {
                Caret.transform.localPosition = GetTerminalCursorPosition(UnityDriver.UnityConsole.CursorLeft, UnityDriver.UnityConsole.CursorTop);
                Caret.gameObject.SetActive(true);
            }
            else
            {
                Caret.gameObject.SetActive(false);
            }
        }

        {
            if (Input.mouseScrollDelta.y != 0)
            {
                var mouseEvent = new MouseEvent
                {
                    Flags = Input.mouseScrollDelta.y < 0 ? MouseFlags.WheeledDown : MouseFlags.WheeledUp,
                    OfX = 0,
                    OfY = 0,
                    View = null,
                    X = (int)((Input.mousePosition.x / Screen.width) * UnityDriver.Cols),
                    Y = (int)(((Screen.height - Input.mousePosition.y) / Screen.height) * UnityDriver.Rows),
                };
                //Debug.Log(mouseEvent);

                (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() => { UnityDriver.mouseHandler(mouseEvent); });
            }
        }

        var inputString = Input.inputString;
        foreach (var keyCode in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
        {
            if (Input.GetKeyDown(keyCode))
            {
                try
                {
                    ConsoleKey consoleKey = ConsoleKey.NoName;
                    try
                    {
                        consoleKey = keyCode.ToConsoleKey();
                    }
                    catch (Exception)
                    {
                    }
                    char c = inputString.FirstOrDefault();
                    bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                    bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    if (c != '\0')
                    {
                        (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() =>
                        {
                            Terminal.Gui.Application.Driver.SendKeys(c, consoleKey, false, alt, ctrl);
                        });
                        Debug.Log($"KeyCode:[{keyCode}] ConsoleKey:[{consoleKey}] c:[{c}] shift:{shift} alt:{alt} ctrl:{ctrl}");
                    }
                    else
                    {
                        (Terminal.Gui.Application.MainLoop.Driver as UnityMainLoop)?.InputQueue.Add(() =>
                        {
                            Terminal.Gui.Application.Driver.SendKeys(c, consoleKey, shift, alt, ctrl);
                        });
                        Debug.Log($"KeyCode:[{keyCode}] ConsoleKey:[{consoleKey}] c:[\\0] shift:{shift} alt:{alt} ctrl:{ctrl}");
                    }

                    lastInputTime = Time.realtimeSinceStartup;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
