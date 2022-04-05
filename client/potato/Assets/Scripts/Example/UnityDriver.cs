using NStack;
using System;
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
#pragma warning restore RCS1138 // Add summary to documentation comment.

        //
        // Summary:
        //     Gets or sets the width of the console window.
        //
        // Returns:
        //     The width of the console window measured in columns.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value of the System.Console.WindowWidth property or the value of the System.Console.WindowHeight
        //     property is less than or equal to 0.-or-The value of the System.Console.WindowHeight
        //     property plus the value of the System.Console.WindowTop property is greater than
        //     or equal to System.Int16.MaxValue.-or-The value of the System.Console.WindowWidth
        //     property or the value of the System.Console.WindowHeight property is greater
        //     than the largest possible window width or height for the current screen resolution
        //     and console font.
        //
        //   T:System.IO.IOException:
        //     Error reading or writing information.
#pragma warning disable RCS1138 // Add summary to documentation comment.

        /// <summary>
        /// Specifies the initial console width.
        /// </summary>
        public const int WIDTH = 80;

        /// <summary>
        /// Specifies the initial console height.
        /// </summary>
        public const int HEIGHT = 25;

        /// <summary>
        /// 
        /// </summary>
        public int WindowWidth { get; set; } = WIDTH;
        //
        // Summary:
        //     Gets a value that indicates whether output has been redirected from the standard
        //     output stream.
        //
        // Returns:
        //     true if output is redirected; otherwise, false.
        /// <summary>
        /// 
        /// </summary>
        public bool IsOutputRedirected { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the error output stream has been redirected
        //     from the standard error stream.
        //
        // Returns:
        //     true if error output is redirected; otherwise, false.
        /// <summary>
        /// 
        /// </summary>
        public static bool IsErrorRedirected { get; }
        //
        // Summary:
        //     Gets the standard input stream.
        //
        // Returns:
        //     A System.IO.TextReader that represents the standard input stream.
        /// <summary>
        /// 
        /// </summary>
        public TextReader In { get; }
        //
        // Summary:
        //     Gets the standard output stream.
        //
        // Returns:
        //     A System.IO.TextWriter that represents the standard output stream.
        /// <summary>
        /// 
        /// </summary>
        public TextWriter Out { get; }
        //
        // Summary:
        //     Gets the standard error output stream.
        //
        // Returns:
        //     A System.IO.TextWriter that represents the standard error output stream.
        /// <summary>
        /// 
        /// </summary>
        public TextWriter Error { get; }
        //
        // Summary:
        //     Gets or sets the encoding the console uses to read input.
        //
        // Returns:
        //     The encoding used to read console input.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The property value in a set operation is null.
        //
        //   T:System.IO.IOException:
        //     An error occurred during the execution of this operation.
        //
        //   T:System.Security.SecurityException:
        //     Your application does not have permission to perform this operation.
        /// <summary>
        /// 
        /// </summary>
        public Encoding InputEncoding { get; set; }
        //
        // Summary:
        //     Gets or sets the encoding the console uses to write output.
        //
        // Returns:
        //     The encoding used to write console output.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The property value in a set operation is null.
        //
        //   T:System.IO.IOException:
        //     An error occurred during the execution of this operation.
        //
        //   T:System.Security.SecurityException:
        //     Your application does not have permission to perform this operation.
        /// <summary>
        /// 
        /// </summary>
        public Encoding OutputEncoding { get; set; }
        //
        // Summary:
        //     Gets or sets the background color of the console.
        //
        // Returns:
        //     A value that specifies the background color of the console; that is, the color
        //     that appears behind each character. The default is black.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The color specified in a set operation is not a valid member of System.ConsoleColor.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; } = _defaultBackgroundColor;
        const ConsoleColor _defaultBackgroundColor = ConsoleColor.Black;

        //
        // Summary:
        //     Gets or sets the foreground color of the console.
        //
        // Returns:
        //     A System.ConsoleColor that specifies the foreground color of the console; that
        //     is, the color of each character that is displayed. The default is gray.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The color specified in a set operation is not a valid member of System.ConsoleColor.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; } = _defaultForegroundColor;
        const ConsoleColor _defaultForegroundColor = ConsoleColor.Gray;
        //
        // Summary:
        //     Gets or sets the height of the buffer area.
        //
        // Returns:
        //     The current height, in rows, of the buffer area.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value in a set operation is less than or equal to zero.-or- The value in
        //     a set operation is greater than or equal to System.Int16.MaxValue.-or- The value
        //     in a set operation is less than System.Console.WindowTop + System.Console.WindowHeight.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public int BufferHeight { get; set; } = HEIGHT;
        //
        // Summary:
        //     Gets or sets the width of the buffer area.
        //
        // Returns:
        //     The current width, in columns, of the buffer area.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value in a set operation is less than or equal to zero.-or- The value in
        //     a set operation is greater than or equal to System.Int16.MaxValue.-or- The value
        //     in a set operation is less than System.Console.WindowLeft + System.Console.WindowWidth.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public int BufferWidth { get; set; } = WIDTH;
        //
        // Summary:
        //     Gets or sets the height of the console window area.
        //
        // Returns:
        //     The height of the console window measured in rows.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value of the System.Console.WindowWidth property or the value of the System.Console.WindowHeight
        //     property is less than or equal to 0.-or-The value of the System.Console.WindowHeight
        //     property plus the value of the System.Console.WindowTop property is greater than
        //     or equal to System.Int16.MaxValue.-or-The value of the System.Console.WindowWidth
        //     property or the value of the System.Console.WindowHeight property is greater
        //     than the largest possible window width or height for the current screen resolution
        //     and console font.
        //
        //   T:System.IO.IOException:
        //     Error reading or writing information.
        /// <summary>
        /// 
        /// </summary>
        public int WindowHeight { get; set; } = HEIGHT;
        //
        // Summary:
        //     Gets or sets a value indicating whether the combination of the System.ConsoleModifiers.Control
        //     modifier key and System.ConsoleKey.C console key (Ctrl+C) is treated as ordinary
        //     input or as an interruption that is handled by the operating system.
        //
        // Returns:
        //     true if Ctrl+C is treated as ordinary input; otherwise, false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     Unable to get or set the input mode of the console input buffer.
        /// <summary>
        /// 
        /// </summary>
        public bool TreatControlCAsInput { get; set; }
        //
        // Summary:
        //     Gets the largest possible number of console window columns, based on the current
        //     font and screen resolution.
        //
        // Returns:
        //     The width of the largest possible console window measured in columns.
        /// <summary>
        /// 
        /// </summary>
        public int LargestWindowWidth { get; }
        //
        // Summary:
        //     Gets the largest possible number of console window rows, based on the current
        //     font and screen resolution.
        //
        // Returns:
        //     The height of the largest possible console window measured in rows.
        /// <summary>
        /// 
        /// </summary>
        public int LargestWindowHeight { get; }
        //
        // Summary:
        //     Gets or sets the leftmost position of the console window area relative to the
        //     screen buffer.
        //
        // Returns:
        //     The leftmost console window position measured in columns.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     In a set operation, the value to be assigned is less than zero.-or-As a result
        //     of the assignment, System.Console.WindowLeft plus System.Console.WindowWidth
        //     would exceed System.Console.BufferWidth.
        //
        //   T:System.IO.IOException:
        //     Error reading or writing information.
        /// <summary>
        /// 
        /// </summary>
        public int WindowLeft { get; set; }
        //
        // Summary:
        //     Gets or sets the top position of the console window area relative to the screen
        //     buffer.
        //
        // Returns:
        //     The uppermost console window position measured in rows.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     In a set operation, the value to be assigned is less than zero.-or-As a result
        //     of the assignment, System.Console.WindowTop plus System.Console.WindowHeight
        //     would exceed System.Console.BufferHeight.
        //
        //   T:System.IO.IOException:
        //     Error reading or writing information.
        /// <summary>
        /// 
        /// </summary>
        public int WindowTop { get; set; }
        //
        // Summary:
        //     Gets or sets the column position of the cursor within the buffer area.
        //
        // Returns:
        //     The current position, in columns, of the cursor.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value in a set operation is less than zero.-or- The value in a set operation
        //     is greater than or equal to System.Console.BufferWidth.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public int CursorLeft { get; set; }
        //
        // Summary:
        //     Gets or sets the row position of the cursor within the buffer area.
        //
        // Returns:
        //     The current position, in rows, of the cursor.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value in a set operation is less than zero.-or- The value in a set operation
        //     is greater than or equal to System.Console.BufferHeight.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public int CursorTop { get; set; }
        //
        // Summary:
        //     Gets or sets the height of the cursor within a character cell.
        //
        // Returns:
        //     The size of the cursor expressed as a percentage of the height of a character
        //     cell. The property value ranges from 1 to 100.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The value specified in a set operation is less than 1 or greater than 100.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public int CursorSize { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether the cursor is visible.
        //
        // Returns:
        //     true if the cursor is visible; otherwise, false.
        //
        // Exceptions:
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public bool CursorVisible { get; set; }
        //
        // Summary:
        //     Gets or sets the title to display in the console title bar.
        //
        // Returns:
        //     The string to be displayed in the title bar of the console. The maximum length
        //     of the title string is 24500 characters.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     In a get operation, the retrieved title is longer than 24500 characters.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     In a set operation, the specified title is longer than 24500 characters.
        //
        //   T:System.ArgumentNullException:
        //     In a set operation, the specified title is null.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether a key press is available in the input stream.
        //
        // Returns:
        //     true if a key press is available; otherwise, false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.InvalidOperationException:
        //     Standard input is redirected to a file instead of the keyboard.
        /// <summary>
        /// 
        /// </summary>
        public bool KeyAvailable { get; }
        //
        // Summary:
        //     Gets a value indicating whether the NUM LOCK keyboard toggle is turned on or
        //     turned off.
        //
        // Returns:
        //     true if NUM LOCK is turned on; false if NUM LOCK is turned off.
        /// <summary>
        /// 
        /// </summary>
        public bool NumberLock { get; }
        //
        // Summary:
        //     Gets a value indicating whether the CAPS LOCK keyboard toggle is turned on or
        //     turned off.
        //
        // Returns:
        //     true if CAPS LOCK is turned on; false if CAPS LOCK is turned off.
        /// <summary>
        /// 
        /// </summary>
        public bool CapsLock { get; }
        //
        // Summary:
        //     Gets a value that indicates whether input has been redirected from the standard
        //     input stream.
        //
        // Returns:
        //     true if input is redirected; otherwise, false.
        /// <summary>
        /// 
        /// </summary>
        public bool IsInputRedirected { get; }

        //
        // Summary:
        //     Plays the sound of a beep through the console speaker.
        //
        // Exceptions:
        //   T:System.Security.HostProtectionException:
        //     This method was executed on a server, such as SQL Server, that does not permit
        //     access to a user interface.
        /// <summary>
        /// 
        /// </summary>
        public void Beep()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Plays the sound of a beep of a specified frequency and duration through the console
        //     speaker.
        //
        // Parameters:
        //   frequency:
        //     The frequency of the beep, ranging from 37 to 32767 hertz.
        //
        //   duration:
        //     The duration of the beep measured in milliseconds.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     frequency is less than 37 or more than 32767 hertz.-or- duration is less than
        //     or equal to zero.
        //
        //   T:System.Security.HostProtectionException:
        //     This method was executed on a server, such as SQL Server, that does not permit
        //     access to the console.
        /// <summary>
        /// 
        /// </summary>
        public void Beep(int frequency, int duration)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Clears the console buffer and corresponding console window of display information.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _buffer = new char[BufferWidth, BufferHeight];
            SetCursorPosition(0, 0);
        }

        char[,] _buffer = new char[WIDTH, HEIGHT];

        //
        // Summary:
        //     Copies a specified source area of the screen buffer to a specified destination
        //     area.
        //
        // Parameters:
        //   sourceLeft:
        //     The leftmost column of the source area.
        //
        //   sourceTop:
        //     The topmost row of the source area.
        //
        //   sourceWidth:
        //     The number of columns in the source area.
        //
        //   sourceHeight:
        //     The number of rows in the source area.
        //
        //   targetLeft:
        //     The leftmost column of the destination area.
        //
        //   targetTop:
        //     The topmost row of the destination area.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     One or more of the parameters is less than zero.-or- sourceLeft or targetLeft
        //     is greater than or equal to System.Console.BufferWidth.-or- sourceTop or targetTop
        //     is greater than or equal to System.Console.BufferHeight.-or- sourceTop + sourceHeight
        //     is greater than or equal to System.Console.BufferHeight.-or- sourceLeft + sourceWidth
        //     is greater than or equal to System.Console.BufferWidth.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Copies a specified source area of the screen buffer to a specified destination
        //     area.
        //
        // Parameters:
        //   sourceLeft:
        //     The leftmost column of the source area.
        //
        //   sourceTop:
        //     The topmost row of the source area.
        //
        //   sourceWidth:
        //     The number of columns in the source area.
        //
        //   sourceHeight:
        //     The number of rows in the source area.
        //
        //   targetLeft:
        //     The leftmost column of the destination area.
        //
        //   targetTop:
        //     The topmost row of the destination area.
        //
        //   sourceChar:
        //     The character used to fill the source area.
        //
        //   sourceForeColor:
        //     The foreground color used to fill the source area.
        //
        //   sourceBackColor:
        //     The background color used to fill the source area.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     One or more of the parameters is less than zero.-or- sourceLeft or targetLeft
        //     is greater than or equal to System.Console.BufferWidth.-or- sourceTop or targetTop
        //     is greater than or equal to System.Console.BufferHeight.-or- sourceTop + sourceHeight
        //     is greater than or equal to System.Console.BufferHeight.-or- sourceLeft + sourceWidth
        //     is greater than or equal to System.Console.BufferWidth.
        //
        //   T:System.ArgumentException:
        //     One or both of the color parameters is not a member of the System.ConsoleColor
        //     enumeration.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Acquires the standard error stream.
        //
        // Returns:
        //     The standard error stream.
        /// <summary>
        /// 
        /// </summary>
        public Stream OpenStandardError()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Acquires the standard error stream, which is set to a specified buffer size.
        //
        // Parameters:
        //   bufferSize:
        //     The internal stream buffer size.
        //
        // Returns:
        //     The standard error stream.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     bufferSize is less than or equal to zero.
        /// <summary>
        /// 
        /// </summary>
        public Stream OpenStandardError(int bufferSize)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Acquires the standard input stream, which is set to a specified buffer size.
        //
        // Parameters:
        //   bufferSize:
        //     The internal stream buffer size.
        //
        // Returns:
        //     The standard input stream.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     bufferSize is less than or equal to zero.
        /// <summary>
        /// 
        /// </summary>
        public Stream OpenStandardInput(int bufferSize)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Acquires the standard input stream.
        //
        // Returns:
        //     The standard input stream.
        /// <summary>
        /// 
        /// </summary>
        public Stream OpenStandardInput()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Acquires the standard output stream, which is set to a specified buffer size.
        //
        // Parameters:
        //   bufferSize:
        //     The internal stream buffer size.
        //
        // Returns:
        //     The standard output stream.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     bufferSize is less than or equal to zero.
        /// <summary>
        /// 
        /// </summary>
        public Stream OpenStandardOutput(int bufferSize)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Acquires the standard output stream.
        //
        // Returns:
        //     The standard output stream.
        /// <summary>
        /// 
        /// </summary>
        public Stream OpenStandardOutput()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Reads the next character from the standard input stream.
        //
        // Returns:
        //     The next character from the input stream, or negative one (-1) if there are currently
        //     no more characters to be read.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public int Read()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Obtains the next character or function key pressed by the user. The pressed key
        //     is optionally displayed in the console window.
        //
        // Parameters:
        //   intercept:
        //     Determines whether to display the pressed key in the console window. true to
        //     not display the pressed key; otherwise, false.
        //
        // Returns:
        //     An object that describes the System.ConsoleKey constant and Unicode character,
        //     if any, that correspond to the pressed console key. The System.ConsoleKeyInfo
        //     object also describes, in a bitwise combination of System.ConsoleModifiers values,
        //     whether one or more Shift, Alt, or Ctrl modifier keys was pressed simultaneously
        //     with the console key.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Console.In property is redirected from some stream other than the
        //     console.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public Stack<ConsoleKeyInfo> MockKeyPresses = new Stack<ConsoleKeyInfo>();

        //
        // Summary:
        //     Obtains the next character or function key pressed by the user. The pressed key
        //     is displayed in the console window.
        //
        // Returns:
        //     An object that describes the System.ConsoleKey constant and Unicode character,
        //     if any, that correspond to the pressed console key. The System.ConsoleKeyInfo
        //     object also describes, in a bitwise combination of System.ConsoleModifiers values,
        //     whether one or more Shift, Alt, or Ctrl modifier keys was pressed simultaneously
        //     with the console key.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Console.In property is redirected from some stream other than the
        //     console.
        /// <summary>
        /// 
        /// </summary>
        public ConsoleKeyInfo ReadKey()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Reads the next line of characters from the standard input stream.
        //
        // Returns:
        //     The next line of characters from the input stream, or null if no more lines are
        //     available.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.OutOfMemoryException:
        //     There is insufficient memory to allocate a buffer for the returned string.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The number of characters in the next line of characters is greater than System.Int32.MaxValue.
        /// <summary>
        /// 
        /// </summary>
        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Sets the foreground and background console colors to their defaults.
        //
        // Exceptions:
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        public void ResetColor()
        {
            BackgroundColor = _defaultBackgroundColor;
            ForegroundColor = _defaultForegroundColor;
        }

        //
        // Summary:
        //     Sets the height and width of the screen buffer area to the specified values.
        //
        // Parameters:
        //   width:
        //     The width of the buffer area measured in columns.
        //
        //   height:
        //     The height of the buffer area measured in rows.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     height or width is less than or equal to zero.-or- height or width is greater
        //     than or equal to System.Int16.MaxValue.-or- width is less than System.Console.WindowLeft
        //     + System.Console.WindowWidth.-or- height is less than System.Console.WindowTop
        //     + System.Console.WindowHeight.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        public void SetBufferSize(int width, int height)
        {
            BufferWidth = width;
            BufferHeight = height;
        }

        //
        // Summary:
        //     Sets the position of the cursor.
        //
        // Parameters:
        //   left:
        //     The column position of the cursor. Columns are numbered from left to right starting
        //     at 0.
        //
        //   top:
        //     The row position of the cursor. Rows are numbered from top to bottom starting
        //     at 0.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     left or top is less than zero.-or- left is greater than or equal to System.Console.BufferWidth.-or-
        //     top is greater than or equal to System.Console.BufferHeight.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        public void SetCursorPosition(int left, int top)
        {
            CursorLeft = left;
            CursorTop = top;
            WindowLeft = Math.Max(Math.Min(left, BufferWidth - WindowWidth), 0);
            WindowTop = Math.Max(Math.Min(top, BufferHeight - WindowHeight), 0);
        }

        //
        // Summary:
        //     Sets the System.Console.Error property to the specified System.IO.TextWriter
        //     object.
        //
        // Parameters:
        //   newError:
        //     A stream that is the new standard error output.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     newError is null.
        //
        //   T:System.Security.SecurityException:
        //     The caller does not have the required permission.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        public void SetError(TextWriter newError)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Sets the System.Console.In property to the specified System.IO.TextReader object.
        //
        // Parameters:
        //   newIn:
        //     A stream that is the new standard input.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     newIn is null.
        //
        //   T:System.Security.SecurityException:
        //     The caller does not have the required permission.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        public void SetIn(TextReader newIn)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Sets the System.Console.Out property to the specified System.IO.TextWriter object.
        //
        // Parameters:
        //   newOut:
        //     A stream that is the new standard output.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     newOut is null.
        //
        //   T:System.Security.SecurityException:
        //     The caller does not have the required permission.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newOut"></param>
        public void SetOut(TextWriter newOut)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Sets the position of the console window relative to the screen buffer.
        //
        // Parameters:
        //   left:
        //     The column position of the upper left corner of the console window.
        //
        //   top:
        //     The row position of the upper left corner of the console window.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     left or top is less than zero.-or- left + System.Console.WindowWidth is greater
        //     than System.Console.BufferWidth.-or- top + System.Console.WindowHeight is greater
        //     than System.Console.BufferHeight.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void SetWindowPosition(int left, int top)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Sets the height and width of the console window to the specified values.
        //
        // Parameters:
        //   width:
        //     The width of the console window measured in columns.
        //
        //   height:
        //     The height of the console window measured in rows.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     width or height is less than or equal to zero.-or- width plus System.Console.WindowLeft
        //     or height plus System.Console.WindowTop is greater than or equal to System.Int16.MaxValue.
        //     -or- width or height is greater than the largest possible window width or height
        //     for the current screen resolution and console font.
        //
        //   T:System.Security.SecurityException:
        //     The user does not have permission to perform this action.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[SecuritySafeCritical]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetWindowSize(int width, int height)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the specified string value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified object to the standard output
        //     stream.
        //
        // Parameters:
        //   value:
        //     The value to write, or null.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(object value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit unsigned integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[CLSCompliant (false)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(ulong value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit signed integer value to
        //     the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        public void Write(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit signed integer value to
        //     the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified object to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     An object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public void Write(string format, object arg0)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit unsigned integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[CLSCompliant (false)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(uint value)
        {
            throw new NotImplementedException();
        }

        //[CLSCompliant (false)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public void Write(string format, object arg0, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified array of objects to the standard
        //     output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg:
        //     An array of objects to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format or arg is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public void Write(string format, params object[] arg)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified Boolean value to the standard
        //     output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(bool value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the specified Unicode character value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(char value)
        {
            _buffer[CursorLeft, CursorTop] = value;
        }

        //
        // Summary:
        //     Writes the specified array of Unicode characters to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     A Unicode character array.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public void Write(char[] buffer)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the specified subarray of Unicode characters to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     An array of Unicode characters.
        //
        //   index:
        //     The starting position in buffer.
        //
        //   count:
        //     The number of characters to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     index plus count specify a position that is not within buffer.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void Write(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        //   arg2:
        //     The third object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void Write(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified System.Decimal value to the standard
        //     output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(decimal value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified single-precision floating-point
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(float value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified double-precision floating-point
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(double value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the current line terminator to the standard output stream.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        public void WriteLine()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified single-precision floating-point
        //     value, followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(float value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit signed integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(int value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit unsigned integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[CLSCompliant (false)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(uint value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit signed integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(long value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit unsigned integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //[CLSCompliant (false)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(ulong value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified object, followed by the current
        //     line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(object value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the specified string value, followed by the current line terminator, to
        //     the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(string value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified object, followed by the current
        //     line terminator, to the standard output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     An object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public void WriteLine(string format, object arg0)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects, followed by the current
        //     line terminator, to the standard output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        //   arg2:
        //     The third object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        //[CLSCompliant (false)]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified array of objects, followed by
        //     the current line terminator, to the standard output stream using the specified
        //     format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg:
        //     An array of objects to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format or arg is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public void WriteLine(string format, params object[] arg)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the specified subarray of Unicode characters, followed by the current
        //     line terminator, to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     An array of Unicode characters.
        //
        //   index:
        //     The starting position in buffer.
        //
        //   count:
        //     The number of characters to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     index plus count specify a position that is not within buffer.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void WriteLine(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified System.Decimal value, followed
        //     by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(decimal value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the specified array of Unicode characters, followed by the current line
        //     terminator, to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     A Unicode character array.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteLine(char[] buffer)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the specified Unicode character, followed by the current line terminator,
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(char value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified Boolean value, followed by the
        //     current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(bool value)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects, followed by the current
        //     line terminator, to the standard output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        public void WriteLine(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Writes the text representation of the specified double-precision floating-point
        //     value, followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Assists with testing, the format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
        /// </summary>
        public override int[,,] Contents => contents;

        public UnityConsole UnityConsole;

        void UpdateOffscreen()
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
                contents[crow, ccol, 0] = (int)(uint)rune;
                contents[crow, ccol, 1] = currentAttribute;
                contents[crow, ccol, 2] = 1;
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

        public override void Refresh()
        {
            int rows = Rows;
            int cols = Cols;

            var savedRow = UnityConsole.CursorTop;
            var savedCol = UnityConsole.CursorLeft;
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
                            SetColor(color);

                        UnityConsole.Write((char)contents[row, col, 0]);
                        contents[row, col, 2] = 0;
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

        Action<KeyEvent> keyHandler;
        Action<KeyEvent> keyUpHandler;

        public override void PrepareToRun(MainLoop mainLoop, Action<KeyEvent> keyHandler, Action<KeyEvent> keyDownHandler, Action<KeyEvent> keyUpHandler, Action<MouseEvent> mouseHandler)
        {
            this.keyHandler = keyHandler;
            this.keyUpHandler = keyUpHandler;

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

            return false;
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
        MainLoop mainLoop;
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
            return false;
        }

        void IMainLoopDriver.MainIteration()
        {
        }
    }
}