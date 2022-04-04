using NStack;
using System;

namespace Terminal.Gui
{
    public class UnityDriver : ConsoleDriver
    {
        public override int Cols => throw new NotImplementedException();

        public override int Rows => throw new NotImplementedException();

        public override int Left => throw new NotImplementedException();

        public override int Top => throw new NotImplementedException();

        public override IClipboard Clipboard => throw new NotImplementedException();

        public override bool HeightAsBuffer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        internal override int[,,] Contents => throw new NotImplementedException();

        public override void AddRune(Rune rune)
        {
            throw new NotImplementedException();
        }

        public override void AddStr(ustring str)
        {
            throw new NotImplementedException();
        }

        public override void CookMouse()
        {
            throw new NotImplementedException();
        }

        public override void End()
        {
            throw new NotImplementedException();
        }

        public override bool EnsureCursorVisibility()
        {
            throw new NotImplementedException();
        }

        public override Attribute GetAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool GetColors(int value, out Color foreground, out Color background)
        {
            throw new NotImplementedException();
        }

        public override bool GetCursorVisibility(out CursorVisibility visibility)
        {
            throw new NotImplementedException();
        }

        public override void Init(Action terminalResized)
        {
            throw new NotImplementedException();
        }

        public override Attribute MakeAttribute(Color fore, Color back)
        {
            throw new NotImplementedException();
        }

        public override void Move(int col, int row)
        {
            throw new NotImplementedException();
        }

        public override void PrepareToRun(MainLoop mainLoop, Action<KeyEvent> keyHandler, Action<KeyEvent> keyDownHandler, Action<KeyEvent> keyUpHandler, Action<MouseEvent> mouseHandler)
        {
            throw new NotImplementedException();
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        public override void SendKeys(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
        {
            throw new NotImplementedException();
        }

        public override void SetAttribute(Attribute c)
        {
            throw new NotImplementedException();
        }

        public override void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            throw new NotImplementedException();
        }

        public override void SetColors(short foregroundColorId, short backgroundColorId)
        {
            throw new NotImplementedException();
        }

        public override bool SetCursorVisibility(CursorVisibility visibility)
        {
            throw new NotImplementedException();
        }

        public override void StartReportingMouseMoves()
        {
            throw new NotImplementedException();
        }

        public override void StopReportingMouseMoves()
        {
            throw new NotImplementedException();
        }

        public override void Suspend()
        {
            throw new NotImplementedException();
        }

        public override void UncookMouse()
        {
            throw new NotImplementedException();
        }

        public override void UpdateCursor()
        {
            throw new NotImplementedException();
        }

        public override void UpdateScreen()
        {
            throw new NotImplementedException();
        }
    }
}