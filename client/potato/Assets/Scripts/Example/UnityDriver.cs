using NStack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Terminal.Gui
{
    public class UnityConsole
    {
        public const int WIDTH = 120;
        public const int HEIGHT = 30;

        public int WindowWidth { get; set; } = WIDTH;
        public bool IsOutputRedirected { get; }
        public static bool IsErrorRedirected { get; }
        public TextReader In { get; }
        public TextWriter Out { get; }
        public TextWriter Error { get; }
        public Encoding InputEncoding { get; set; }
        public Encoding OutputEncoding { get; set; }
        public ConsoleColor BackgroundColor { get; set; } = _defaultBackgroundColor;
        const ConsoleColor _defaultBackgroundColor = ConsoleColor.Black;

        public ConsoleColor ForegroundColor { get; set; } = _defaultForegroundColor;
        const ConsoleColor _defaultForegroundColor = ConsoleColor.Gray;
        public int BufferHeight { get; set; } = HEIGHT;
        public int BufferWidth { get; set; } = WIDTH;
        public int WindowHeight { get; set; } = HEIGHT;
        public bool TreatControlCAsInput { get; set; }
        public int LargestWindowWidth { get; }
        public int LargestWindowHeight { get; }
        public int WindowLeft { get; set; }
        public int WindowTop { get; set; }
        public int CursorLeft { get; set; }
        public int CursorTop { get; set; }
        public int CursorSize { get; set; }
        public bool CursorVisible { get; set; }
        public string Title { get; set; }
        public bool KeyAvailable { get; }
        public bool NumberLock { get; }
        public bool CapsLock { get; }
        public bool IsInputRedirected { get; }

        public void Beep()
        {
            throw new NotImplementedException();
        }
        public void Beep(int frequency, int duration)
        {
            throw new NotImplementedException();
        }
        public void Clear()
        {
            _buffer = new char[BufferWidth, BufferHeight];
            SetCursorPosition(0, 0);
        }

        char[,] _buffer = new char[WIDTH, HEIGHT];

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            throw new NotImplementedException();
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
        {
            throw new NotImplementedException();
        }

        public Stream OpenStandardError()
        {
            throw new NotImplementedException();
        }

        public Stream OpenStandardError(int bufferSize)
        {
            throw new NotImplementedException();
        }

        public Stream OpenStandardInput(int bufferSize)
        {
            throw new NotImplementedException();
        }

        public Stream OpenStandardInput()
        {
            throw new NotImplementedException();
        }

        public Stream OpenStandardOutput(int bufferSize)
        {
            throw new NotImplementedException();
        }

        public Stream OpenStandardOutput()
        {
            throw new NotImplementedException();
        }

        public int Read()
        {
            throw new NotImplementedException();
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            if (MockKeyPresses.Count > 0)
            {
                return MockKeyPresses.Pop();
            }
            else
            {
                return new ConsoleKeyInfo('\0', (ConsoleKey)'\0', false, false, false);
            }
        }

        public Stack<ConsoleKeyInfo> MockKeyPresses = new Stack<ConsoleKeyInfo>();

        public ConsoleKeyInfo ReadKey()
        {
            throw new NotImplementedException();
        }
        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void ResetColor()
        {
            BackgroundColor = _defaultBackgroundColor;
            ForegroundColor = _defaultForegroundColor;
        }
        public void SetBufferSize(int width, int height)
        {
            BufferWidth = width;
            BufferHeight = height;
        }
        public void SetCursorPosition(int left, int top)
        {
            CursorLeft = left;
            CursorTop = top;
            WindowLeft = Math.Max(Math.Min(left, BufferWidth - WindowWidth), 0);
            WindowTop = Math.Max(Math.Min(top, BufferHeight - WindowHeight), 0);
        }

        public void SetError(TextWriter newError)
        {
            throw new NotImplementedException();
        }

        public void SetIn(TextReader newIn)
        {
            throw new NotImplementedException();
        }
        public void SetOut(TextWriter newOut)
        {
            throw new NotImplementedException();
        }
        public void SetWindowPosition(int left, int top)
        {
            throw new NotImplementedException();
        }

        public void SetWindowSize(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void Write(string value)
        {
            throw new NotImplementedException();
        }

        public void Write(object value)
        {
            throw new NotImplementedException();
        }

        public void Write(ulong value)
        {
            throw new NotImplementedException();
        }

        public void Write(long value)
        {
            throw new NotImplementedException();
        }

        public void Write(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        public void Write(int value)
        {
            throw new NotImplementedException();
        }

        public void Write(string format, object arg0)
        {
            throw new NotImplementedException();
        }

        public void Write(uint value)
        {
            throw new NotImplementedException();
        }

        public void Write(string format, object arg0, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void Write(string format, params object[] arg)
        {
            throw new NotImplementedException();
        }

        public void Write(bool value)
        {
            throw new NotImplementedException();
        }

        public void Write(char value)
        {
            _buffer[CursorLeft, CursorTop] = value;
        }

        public void Write(char[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Write(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public void Write(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void Write(decimal value)
        {
            throw new NotImplementedException();
        }

        public void Write(float value)
        {
            throw new NotImplementedException();
        }
        public void Write(double value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine()
        {
            throw new NotImplementedException();
        }

        public void WriteLine(float value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(int value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(uint value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(long value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(ulong value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(object value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string format, object arg0)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string format, params object[] arg)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(decimal value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(char[] buffer)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(char value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(bool value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(double value)
        {
            throw new NotImplementedException();
        }

    }

    public class UnityDriver : ConsoleDriver
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        int cols, rows, left, top;
        public override int Cols => cols;
        public override int Rows => rows;
        // Only handling left here because not all terminals has a horizontal scroll bar.
        public override int Left => 0;
        public override int Top => 0;
        public override bool HeightAsBuffer { get; set; }
        public override IClipboard Clipboard { get; }

        // The format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
        int[,,] contents;
        bool[] dirtyLine;

        private readonly object copyLock = new object();

        /// <summary>
        /// Assists with testing, the format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
        /// </summary>
        public override int[,,] Contents => contents;
        public int[,,] ContentsView
        {
            get
            {
                lock (copyLock)
                {
                    return contents.Clone() as int[,,];
                }
            }
        }

        public UnityConsole UnityConsole;

        void UpdateOffscreen()
        {
            lock (copyLock)
            {
                int cols = Cols;
                int rows = Rows;

                contents = new int[rows, cols, 3];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        contents[r, c, 0] = ' ';
                        contents[r, c, 1] = MakeColor(ConsoleColor.Gray, ConsoleColor.Black);
                        contents[r, c, 2] = 0;
                    }
                }
                dirtyLine = new bool[rows];
                for (int row = 0; row < rows; row++)
                    dirtyLine[row] = true;
            }
        }

        static bool sync = false;

        public UnityDriver()
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    Clipboard = new WindowsClipboard();
            //}
            //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            //{
            //    Clipboard = new MacOSXClipboard();
            //}
            //else
            //{
            //    if (CursesDriver.Is_WSL_Platform())
            //    {
            //        Clipboard = new WSLClipboard();
            //    }
            //    else
            //    {
            //        Clipboard = new CursesClipboard();
            //    }
            //}
        }

        bool needMove;
        // Current row, and current col, tracked by Move/AddCh only
        int ccol, crow;
        public override void Move(int col, int row)
        {
            ccol = col;
            crow = row;

            if (Clip.Contains(col, row))
            {
                UnityConsole.CursorTop = row;
                UnityConsole.CursorLeft = col;
                needMove = false;
            }
            else
            {
                UnityConsole.CursorTop = Clip.Y;
                UnityConsole.CursorLeft = Clip.X;
                needMove = true;
            }

        }

        public override void AddRune(Rune rune)
        {
            rune = MakePrintable(rune);
            if (Clip.Contains(ccol, crow))
            {
                if (needMove)
                {
                    //MockConsole.CursorLeft = ccol;
                    //MockConsole.CursorTop = crow;
                    needMove = false;
                }
                lock (copyLock)
                {
                    contents[crow, ccol, 0] = (int)(uint)rune;
                    contents[crow, ccol, 1] = currentAttribute;
                    contents[crow, ccol, 2] = 1;
                }
                dirtyLine[crow] = true;
            }
            else
                needMove = true;
            ccol++;
            //if (ccol == Cols) {
            //	ccol = 0;
            //	if (crow + 1 < Rows)
            //		crow++;
            //}
            if (sync)
                UpdateScreen();
        }

        public override void AddStr(ustring str)
        {
            foreach (var rune in str)
                AddRune(rune);
        }

        public override void End()
        {
            UnityConsole.ResetColor();
            UnityConsole.Clear();
        }

        static Attribute MakeColor(ConsoleColor f, ConsoleColor b)
        {
            // Encode the colors into the int value.
            return new Attribute(
                value: ((((int)f) & 0xffff) << 16) | (((int)b) & 0xffff),
                foreground: (Color)f,
                background: (Color)b
                );
        }

        public override void Init(Action terminalResized)
        {
            TerminalResized = terminalResized;

            cols = UnityConsole.WindowWidth = UnityConsole.BufferWidth = UnityConsole.WIDTH;
            rows = UnityConsole.WindowHeight = UnityConsole.BufferHeight = UnityConsole.HEIGHT;
            UnityConsole.Clear();
            ResizeScreen();
            UpdateOffScreen();

            Colors.TopLevel = new ColorScheme();
            Colors.Base = new ColorScheme();
            Colors.Dialog = new ColorScheme();
            Colors.Menu = new ColorScheme();
            Colors.Error = new ColorScheme();
            Clip = new Rect(0, 0, Cols, Rows);

            Colors.TopLevel.Normal = MakeColor(ConsoleColor.Green, ConsoleColor.Black);
            Colors.TopLevel.Focus = MakeColor(ConsoleColor.White, ConsoleColor.DarkCyan);
            Colors.TopLevel.HotNormal = MakeColor(ConsoleColor.DarkYellow, ConsoleColor.Black);
            Colors.TopLevel.HotFocus = MakeColor(ConsoleColor.DarkBlue, ConsoleColor.DarkCyan);
            Colors.TopLevel.Disabled = MakeColor(ConsoleColor.DarkGray, ConsoleColor.Black);

            Colors.Base.Normal = MakeColor(ConsoleColor.White, ConsoleColor.Blue);
            Colors.Base.Focus = MakeColor(ConsoleColor.Black, ConsoleColor.Cyan);
            Colors.Base.HotNormal = MakeColor(ConsoleColor.Yellow, ConsoleColor.Blue);
            Colors.Base.HotFocus = MakeColor(ConsoleColor.Yellow, ConsoleColor.Cyan);
            Colors.Base.Disabled = MakeColor(ConsoleColor.DarkGray, ConsoleColor.DarkBlue);

            // Focused,
            //    Selected, Hot: Yellow on Black
            //    Selected, text: white on black
            //    Unselected, hot: yellow on cyan
            //    unselected, text: same as unfocused
            Colors.Menu.HotFocus = MakeColor(ConsoleColor.Yellow, ConsoleColor.Black);
            Colors.Menu.Focus = MakeColor(ConsoleColor.White, ConsoleColor.Black);
            Colors.Menu.HotNormal = MakeColor(ConsoleColor.Yellow, ConsoleColor.Cyan);
            Colors.Menu.Normal = MakeColor(ConsoleColor.White, ConsoleColor.Cyan);
            Colors.Menu.Disabled = MakeColor(ConsoleColor.DarkGray, ConsoleColor.Cyan);

            Colors.Dialog.Normal = MakeColor(ConsoleColor.Black, ConsoleColor.Gray);
            Colors.Dialog.Focus = MakeColor(ConsoleColor.Black, ConsoleColor.Cyan);
            Colors.Dialog.HotNormal = MakeColor(ConsoleColor.Blue, ConsoleColor.Gray);
            Colors.Dialog.HotFocus = MakeColor(ConsoleColor.Blue, ConsoleColor.Cyan);
            Colors.Dialog.Disabled = MakeColor(ConsoleColor.DarkGray, ConsoleColor.Gray);

            Colors.Error.Normal = MakeColor(ConsoleColor.White, ConsoleColor.Red);
            Colors.Error.Focus = MakeColor(ConsoleColor.Black, ConsoleColor.Gray);
            Colors.Error.HotNormal = MakeColor(ConsoleColor.Yellow, ConsoleColor.Red);
            Colors.Error.HotFocus = Colors.Error.HotNormal;
            Colors.Error.Disabled = MakeColor(ConsoleColor.DarkGray, ConsoleColor.White);

            //MockConsole.Clear ();
        }

        public override Attribute MakeAttribute(Color fore, Color back)
        {
            return MakeColor((ConsoleColor)fore, (ConsoleColor)back);
        }

        int redrawColor = -1;
        void SetColor(int color)
        {
            redrawColor = color;
            IEnumerable<int> values = Enum.GetValues(typeof(ConsoleColor))
                  .OfType<ConsoleColor>()
                  .Select(s => (int)s);
            if (values.Contains(color & 0xffff))
            {
                UnityConsole.BackgroundColor = (ConsoleColor)(color & 0xffff);
            }
            if (values.Contains((color >> 16) & 0xffff))
            {
                UnityConsole.ForegroundColor = (ConsoleColor)((color >> 16) & 0xffff);
            }
        }

        public override void UpdateScreen()
        {
            int top = Top;
            int left = Left;
            int rows = Math.Min(Console.WindowHeight + top, Rows);
            int cols = Cols;

            UnityConsole.CursorTop = 0;
            UnityConsole.CursorLeft = 0;
            lock (copyLock)
            {
                for (int row = top; row < rows; row++)
                {
                    dirtyLine[row] = false;
                    for (int col = left; col < cols; col++)
                    {
                        contents[row, col, 2] = 0;
                        var color = contents[row, col, 1];
                        if (color != redrawColor)
                            SetColor(color);
                        UnityConsole.Write((char)contents[row, col, 0]);
                    }
                }
            }
        }

        static UnityEngine.Color ToOpaque(UnityEngine.Color c)
        {
            return new UnityEngine.Color(c.r, c.g, c.b, 1);
        }

        public Dictionary<ConsoleColor, UnityEngine.Color> colorMap = new Dictionary<ConsoleColor, UnityEngine.Color> {
            { ConsoleColor.Black,        ToOpaque(UnityEngine.Color.black) },
            { ConsoleColor.DarkBlue,     ToOpaque(UnityEngine.Color.blue / 2)  },
            { ConsoleColor.DarkGreen,    ToOpaque(UnityEngine.Color.green / 2) },
            { ConsoleColor.DarkCyan,     ToOpaque(UnityEngine.Color.cyan / 2) },
            { ConsoleColor.DarkRed,      ToOpaque(UnityEngine.Color.red / 2) },
            { ConsoleColor.DarkMagenta,  ToOpaque(UnityEngine.Color.magenta / 2) },
            { ConsoleColor.DarkYellow,   ToOpaque(UnityEngine.Color.yellow / 2) },
            { ConsoleColor.Gray,         ToOpaque(UnityEngine.Color.white / 2) },
            { ConsoleColor.DarkGray,     ToOpaque(UnityEngine.Color.white / 3) },
            { ConsoleColor.Blue,         ToOpaque(UnityEngine.Color.blue) },
            { ConsoleColor.Green,        ToOpaque(UnityEngine.Color.green) },
            { ConsoleColor.Cyan,         ToOpaque(UnityEngine.Color.cyan) },
            { ConsoleColor.Red,          ToOpaque(UnityEngine.Color.red) },
            { ConsoleColor.Magenta,      ToOpaque(UnityEngine.Color.magenta) },
            { ConsoleColor.Yellow,       ToOpaque(UnityEngine.Color.yellow) },
            { ConsoleColor.White,        ToOpaque(UnityEngine.Color.white) },
        };

        public Dictionary<Color, UnityEngine.Color> colorMap2 = new Dictionary<Color, UnityEngine.Color> {
            { Color.Black,        ToOpaque(UnityEngine.Color.black) },
            { Color.Blue,         ToOpaque(UnityEngine.Color.blue / 2)  },
            { Color.Green,        ToOpaque(UnityEngine.Color.green / 2) },
            { Color.Cyan,         ToOpaque(UnityEngine.Color.cyan / 2) },
            { Color.Red,          ToOpaque(UnityEngine.Color.red / 2) },
            { Color.Magenta,      ToOpaque(UnityEngine.Color.magenta / 2) },
            { Color.Brown,        ToOpaque(UnityEngine.Color.yellow / 2) },
            { Color.Gray,         ToOpaque(UnityEngine.Color.white / 2) },
            { Color.DarkGray,     ToOpaque(UnityEngine.Color.white / 3) },
            { Color.BrightBlue,   ToOpaque(UnityEngine.Color.blue) },
            { Color.BrightGreen,  ToOpaque(UnityEngine.Color.green) },
            { Color.BrightCyan,   ToOpaque(UnityEngine.Color.cyan) },
            { Color.BrightRed,    ToOpaque(UnityEngine.Color.red) },
            { Color.BrightMagenta,ToOpaque(UnityEngine.Color.magenta) },
            { Color.BrightYellow, ToOpaque(UnityEngine.Color.yellow) },
            { Color.White,        ToOpaque(UnityEngine.Color.white) },
        };

        public override void Refresh()
        {
            int rows = Rows;
            int cols = Cols;

            var savedRow = UnityConsole.CursorTop;
            var savedCol = UnityConsole.CursorLeft;
            lock (copyLock)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (!dirtyLine[row])
                        continue;
                    dirtyLine[row] = false;
                    for (int col = 0; col < cols; col++)
                    {
                        if (contents[row, col, 2] != 1)
                            continue;

                        UnityConsole.CursorTop = row;
                        UnityConsole.CursorLeft = col;
                        for (; col < cols && contents[row, col, 2] == 1; col++)
                        {
                            var color = contents[row, col, 1];
                            if (color != redrawColor)
                            {
                                SetColor(color);
                            }

                            UnityConsole.Write((char)contents[row, col, 0]);
                            contents[row, col, 2] = 0;
                        }
                    }
                }
            }

            UnityConsole.CursorTop = savedRow;
            UnityConsole.CursorLeft = savedCol;
        }

        Attribute currentAttribute;
        public override void SetAttribute(Attribute c)
        {
            currentAttribute = c;
        }

        Key MapKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    return MapKeyModifiers(keyInfo, Key.Esc);
                case ConsoleKey.Tab:
                    return keyInfo.Modifiers == ConsoleModifiers.Shift ? Key.BackTab : Key.Tab;
                case ConsoleKey.Home:
                    return MapKeyModifiers(keyInfo, Key.Home);
                case ConsoleKey.End:
                    return MapKeyModifiers(keyInfo, Key.End);
                case ConsoleKey.LeftArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorLeft);
                case ConsoleKey.RightArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorRight);
                case ConsoleKey.UpArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorUp);
                case ConsoleKey.DownArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorDown);
                case ConsoleKey.PageUp:
                    return MapKeyModifiers(keyInfo, Key.PageUp);
                case ConsoleKey.PageDown:
                    return MapKeyModifiers(keyInfo, Key.PageDown);
                case ConsoleKey.Enter:
                    return MapKeyModifiers(keyInfo, Key.Enter);
                case ConsoleKey.Spacebar:
                    return MapKeyModifiers(keyInfo, keyInfo.KeyChar == 0 ? Key.Space : (Key)keyInfo.KeyChar);
                case ConsoleKey.Backspace:
                    return MapKeyModifiers(keyInfo, Key.Backspace);
                case ConsoleKey.Delete:
                    return MapKeyModifiers(keyInfo, Key.DeleteChar);
                case ConsoleKey.Insert:
                    return MapKeyModifiers(keyInfo, Key.InsertChar);

                case ConsoleKey.Oem1:
                case ConsoleKey.Oem2:
                case ConsoleKey.Oem3:
                case ConsoleKey.Oem4:
                case ConsoleKey.Oem5:
                case ConsoleKey.Oem6:
                case ConsoleKey.Oem7:
                case ConsoleKey.Oem8:
                case ConsoleKey.Oem102:
                case ConsoleKey.OemPeriod:
                case ConsoleKey.OemComma:
                case ConsoleKey.OemPlus:
                case ConsoleKey.OemMinus:
                    if (keyInfo.KeyChar == 0)
                        return Key.Unknown;

                    return (Key)((uint)keyInfo.KeyChar);
            }

            var key = keyInfo.Key;
            if (key >= ConsoleKey.A && key <= ConsoleKey.Z)
            {
                var delta = key - ConsoleKey.A;
                if (keyInfo.Modifiers == ConsoleModifiers.Control)
                {
                    return (Key)(((uint)Key.CtrlMask) | ((uint)Key.A + delta));
                }
                if (keyInfo.Modifiers == ConsoleModifiers.Alt)
                {
                    return (Key)(((uint)Key.AltMask) | ((uint)Key.A + delta));
                }
                if ((keyInfo.Modifiers & (ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
                {
                    if (keyInfo.KeyChar == 0)
                    {
                        return (Key)(((uint)Key.AltMask | (uint)Key.CtrlMask) | ((uint)Key.A + delta));
                    }
                    else
                    {
                        return (Key)((uint)keyInfo.KeyChar);
                    }
                }
                return (Key)((uint)keyInfo.KeyChar);
            }
            if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
            {
                var delta = key - ConsoleKey.D0;
                if (keyInfo.Modifiers == ConsoleModifiers.Alt)
                {
                    return (Key)(((uint)Key.AltMask) | ((uint)Key.D0 + delta));
                }
                if (keyInfo.Modifiers == ConsoleModifiers.Control)
                {
                    return (Key)(((uint)Key.CtrlMask) | ((uint)Key.D0 + delta));
                }
                if (keyInfo.KeyChar == 0 || keyInfo.KeyChar == 30)
                {
                    return MapKeyModifiers(keyInfo, (Key)((uint)Key.D0 + delta));
                }
                return (Key)((uint)keyInfo.KeyChar);
            }
            if (key >= ConsoleKey.F1 && key <= ConsoleKey.F12)
            {
                var delta = key - ConsoleKey.F1;
                if ((keyInfo.Modifiers & (ConsoleModifiers.Shift | ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
                {
                    return MapKeyModifiers(keyInfo, (Key)((uint)Key.F1 + delta));
                }

                return (Key)((uint)Key.F1 + delta);
            }
            if (keyInfo.KeyChar != 0)
            {
                return MapKeyModifiers(keyInfo, (Key)((uint)keyInfo.KeyChar));
            }

            return (Key)(0xffffffff);
        }

        KeyModifiers keyModifiers;

        private Key MapKeyModifiers(ConsoleKeyInfo keyInfo, Key key)
        {
            Key keyMod = new Key();
            if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
                keyMod = Key.ShiftMask;
            if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                keyMod |= Key.CtrlMask;
            if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
                keyMod |= Key.AltMask;

            return keyMod != Key.Null ? keyMod | key : key;
        }

        private Action<KeyEvent> keyHandler;
        private Action<KeyEvent> keyDownHandler;
        private Action<KeyEvent> keyUpHandler;
        public Action<MouseEvent> mouseHandler;

        public override void PrepareToRun(MainLoop mainLoop, Action<KeyEvent> keyHandler, Action<KeyEvent> keyDownHandler, Action<KeyEvent> keyUpHandler, Action<MouseEvent> mouseHandler)
        {
            this.keyHandler = keyHandler;
            this.keyDownHandler = keyDownHandler;
            this.keyUpHandler = keyUpHandler;
            this.mouseHandler = mouseHandler;

            // Note: Net doesn't support keydown/up events and thus any passed keyDown/UpHandlers will never be called
            //(mainLoop.Driver as UnityMainLoop).KeyPressed += (consoleKey) => ProcessInput(consoleKey);
        }

        void ProcessInput(ConsoleKeyInfo consoleKey)
        {
            keyModifiers = new KeyModifiers();
            var map = MapKey(consoleKey);
            if (map == (Key)0xffffffff)
                return;

            if (consoleKey.Modifiers.HasFlag(ConsoleModifiers.Alt))
            {
                keyModifiers.Alt = true;
            }
            if (consoleKey.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                keyModifiers.Shift = true;
            }
            if (consoleKey.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                keyModifiers.Ctrl = true;
            }

            keyHandler(new KeyEvent(map, keyModifiers));
            keyUpHandler(new KeyEvent(map, keyModifiers));
        }

        public override Attribute GetAttribute()
        {
            return currentAttribute;
        }

        /// <inheritdoc/>
        public override bool GetCursorVisibility(out CursorVisibility visibility)
        {
            if (UnityConsole.CursorVisible)
            {
                visibility = CursorVisibility.Default;
            }
            else
            {
                visibility = CursorVisibility.Invisible;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool SetCursorVisibility(CursorVisibility visibility)
        {
            if (visibility == CursorVisibility.Invisible)
            {
                UnityConsole.CursorVisible = false;
            }
            else
            {
                UnityConsole.CursorVisible = true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool EnsureCursorVisibility()
        {
            return false;
        }

        public override void SendKeys(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
        {
            ProcessInput(new ConsoleKeyInfo(keyChar, key, shift, alt, control));
        }

        public void SetBufferSize(int width, int height)
        {
            cols = UnityConsole.WindowWidth = UnityConsole.BufferWidth = width;
            rows = UnityConsole.WindowHeight = UnityConsole.BufferHeight = height;
            ProcessResize();
        }

        public void SetWindowSize(int width, int height)
        {
            UnityConsole.WindowWidth = width;
            UnityConsole.WindowHeight = height;
            if (width > cols || !HeightAsBuffer)
            {
                cols = UnityConsole.BufferWidth = width;
            }
            if (height > rows || !HeightAsBuffer)
            {
                rows = UnityConsole.BufferHeight = height;
            }
            ProcessResize();
        }

        public void SetWindowPosition(int left, int top)
        {
            if (HeightAsBuffer)
            {
                this.left = UnityConsole.WindowLeft = Math.Max(Math.Min(left, Cols - UnityConsole.WindowWidth), 0);
                this.top = UnityConsole.WindowTop = Math.Max(Math.Min(top, Rows - Console.WindowHeight), 0);
            }
            else if (this.left > 0 || this.top > 0)
            {
                this.left = UnityConsole.WindowLeft = 0;
                this.top = UnityConsole.WindowTop = 0;
            }
        }

        void ProcessResize()
        {
            ResizeScreen();
            UpdateOffScreen();
            TerminalResized?.Invoke();
        }

        void ResizeScreen()
        {
            if (!HeightAsBuffer)
            {
                if (UnityConsole.WindowHeight > 0)
                {
                    // Can raise an exception while is still resizing.
                    try
                    {
                        UnityConsole.CursorTop = 0;
                        UnityConsole.CursorLeft = 0;
                        UnityConsole.WindowTop = 0;
                        UnityConsole.WindowLeft = 0;
                    }
                    catch (System.IO.IOException)
                    {
                        return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    UnityConsole.WindowLeft = Math.Max(Math.Min(left, Cols - Console.WindowWidth), 0);
                    UnityConsole.WindowTop = Math.Max(Math.Min(top, Rows - Console.WindowHeight), 0);
                }
                catch (Exception)
                {
                    return;
                }
            }

            Clip = new Rect(0, 0, Cols, Rows);

            contents = new int[Rows, Cols, 3];
            dirtyLine = new bool[Rows];
        }

        void UpdateOffScreen()
        {
            // Can raise an exception while is still resizing.
            try
            {
                lock (copyLock)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            contents[row, c, 0] = ' ';
                            contents[row, c, 1] = (ushort)Colors.TopLevel.Normal;
                            contents[row, c, 2] = 0;
                            dirtyLine[row] = true;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        public override bool GetColors(int value, out Color foreground, out Color background)
        {
            bool hasColor = false;
            foreground = default;
            background = default;
            IEnumerable<int> values = Enum.GetValues(typeof(ConsoleColor))
                  .OfType<ConsoleColor>()
                  .Select(s => (int)s);
            if (values.Contains(value & 0xffff))
            {
                hasColor = true;
                background = (Color)(ConsoleColor)(value & 0xffff);
            }
            if (values.Contains((value >> 16) & 0xffff))
            {
                hasColor = true;
                foreground = (Color)(ConsoleColor)((value >> 16) & 0xffff);
            }
            return hasColor;
        }

        #region Unused
        public override void UpdateCursor()
        {
            //
        }

        public override void StartReportingMouseMoves()
        {
        }

        public override void StopReportingMouseMoves()
        {
        }

        public override void Suspend()
        {
        }

        public override void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
        }

        public override void SetColors(short foregroundColorId, short backgroundColorId)
        {
            throw new NotImplementedException();
        }

        public override void CookMouse()
        {
        }

        public override void UncookMouse()
        {
        }

        #endregion
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    public class UnityMainLoop : IMainLoopDriver
    {
        public BlockingCollection<bool> InputQueue { get; private set; } = new BlockingCollection<bool>();

        public UnityMainLoop()
        {
        }

        void IMainLoopDriver.Setup(MainLoop mainLoop)
        {
        }

        void IMainLoopDriver.Wakeup()
        {
        }

        bool IMainLoopDriver.EventsPending(bool wait)
        {
            return InputQueue.Take();
        }

        void IMainLoopDriver.MainIteration()
        {
        }
    }
}