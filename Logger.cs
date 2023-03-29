using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
namespace OakLogger
{
    public enum LogItem
    {
        /// <summary>
        /// The text passed to <see cref="OLog.Print"/>.
        /// </summary>
        Text,
        /// <summary>
        /// Seconds since the program has started.
        /// </summary>
        TimeSinceStartup,
        /// <summary>
        /// The system's time in the MM/dd/yyyy HH:mm format (ISO 8601).
        /// </summary>
        SystemTime,
        /// <summary>
        /// The stack frame at which <see cref="OLog.Print"/> was called.
        /// </summary>
        StackFrame,
        /// <summary>
        /// <see cref="OLog.LogType">
        /// </summary>
        Type,
        /// <summary>
        /// The thread used when <see cref="OLog.Print"/> was called.
        /// </summary>
        ThreadID
    }
    public class LogOutput
    {
        /// <summary>
        /// This controls where the logger will output text, typically a <see cref="FileStream"/> or <see cref="Console.Out"/>.
        /// </summary>
        public TextWriter OutputStream;
        /// <summary>
        /// Seperator used for seperating output text and the <see cref="LogItem"/>s.
        /// </summary>
        public string Seperator = " ";
        /// <summary>
        /// Controls wheter the seperator should seperate each <see cref="LogItem"/>s.
        /// </summary>
        public bool SeperateLogItems = "false";
        /// <summary>
        /// This should be set to true if the output supports ANSI color codes.
        /// </summary>
        public bool UseColor = "false";
        ///<summary>
        ///Initializes a new instance of the <see cref="LogOutput"/> class.
        ///</summary>
        public LogOutput(TextWriter outputStream, string seperator = " ", bool seperateLogItems = false, bool useColor = false)
        {
            OutputStream = outputStream;
            Seperator = seperator;
            SeperateLogItems = seperateLogItems;
            UseColor = useColor;
        }
    }

    public class OLog
    {
        /// <summary>
        /// RGB Color value, each <see cref="byte"/> representing a color value of 0 to 255.
        /// </summary>
        public Tuple<byte, byte, byte> Color = new Tuple<byte, byte, byte>(0,0,0);
        public List<LogItem> LogItems;
        /// <summary>
        /// The displayed type of the log, usually WARNING,ERROR or SUCCESS. When displayed it will be surrounded with [brackets].
        /// </summary>
        public string LogType = "DEFAULT";
        ///<summary>
        ///If set to true <see cref="Print"> will end a line with a new line character 
        ///</summary>
        public bool ImplicitNewLine = true;
        public List<LogOutput> Outputs;
        ///<summary>
        ///Initializes a new instance of the <see cref="Olog"/> class with default values
        ///</summary>
        public OLog()
        {
            LogItems = new List<LogItem>() { LogItem.Text };
            Outputs = new List<LogOutput>();
        }

        ///<summary>
        ///Print formatted text to standard output (<see cref="LogToSTDOUT"/>) and/or a file (<see cref="LogToFile"/>) .
        ///</summary>
        public void Print(object toPrint)
        {
            for(int i = 0;i < Outputs.Count;i++)
            {
                Output(Outputs[i],toPrint);
                Outputs[i].OutputStream.Flush();
            }
            
        }
        
        private void Output(LogOutput logOutput,object toPrint)
        {
            for (int i = 0; i < LogItems.Count; i++)
                {
                    if (i > 0 && LogItems[i - 1] == LogItem.Text)
                    {
                        logOutput.OutputStream.Write(logOutput.Seperator);
                    }
                    switch (LogItems[i])
                    {
                        case LogItem.Type:
                            {
                                WriteType(logOutput.OutputStream, logOutput.UseColor);
                                break;
                            }
                        case LogItem.Text:
                            {
                                WriteText(logOutput.OutputStream, logOutput.Seperator,toPrint);
                                break;
                            }
                        case LogItem.TimeSinceStartup:
                            {
                                WriteTimeSinceStartup(logOutput.OutputStream, logOutput.UseColor);
                                break;
                            }
                        case LogItem.StackFrame:
                            {
                                WriteStackFrame(logOutput.OutputStream, logOutput.UseColor);
                                break;
                            }
                        case LogItem.ThreadID:
                            {
                                WriteThreadID(logOutput.OutputStream, logOutput.UseColor);
                                break;
                            }
                        case LogItem.SystemTime:
                            {
                                WriteSystemTime(logOutput.OutputStream, logOutput.UseColor);
                                break;
                            }
                    }
                    if (logOutput.SeperateLogItems && i < LogItems.Count - 2)
                    {
                        logOutput.OutputStream.Write(logOutput.Seperator);
                    }

                }
            if(ImplicitNewLine)
            {
                logOutput.OutputStream.Write('\n');
            }
        }
        private void WriteText(TextWriter output,string seperator,object toPrint)
        {
            if (LogItems.Count != 1)
            {
                output.Write(seperator);
            }
            output.Write(toPrint);
        }
        private void WriteType(TextWriter output,bool useColor)
        {
            if (useColor)
            {
                WriteColor(output,Color);
            }
            output.Write(String.Format("[{0}]", LogType));
            if (useColor)
            {
                output.Write("\x1b[0m");
            }
            
            
        }
        private void WriteTimeSinceStartup(TextWriter output,bool useColor)
        {
            if (useColor)
            {
                WriteColor(output,Color);
            }
            double seconds;
            seconds = Math.Round((double)DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime).Milliseconds / 1000d, 2);
            output.Write(String.Format("[{0}s]", seconds));
            if (useColor)
            {
                output.Write("\x1b[0m");
            }
        }
        private void WriteSystemTime(TextWriter output,bool useColor)
        {
            if (useColor)
            {
                WriteColor(output,Color);
            }
            string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            output.Write(String.Format("[{0}]", time));
            if (useColor)
            {
                output.Write("\x1b[0m");
            }
        }
        private void WriteStackFrame(TextWriter output,bool useColor)
        {
            if (useColor)
            {
                WriteColor(output,Color);
            }
            string stackframe = (new StackTrace()).ToString().Split("at ").Skip(4).ToArray<string>()[0];
            output.Write(String.Format("[{0}]", stackframe.Replace('\n', ' ').Trim()));
            if (useColor)
            {
                output.Write("\x1b[0m");
            }
        }
        private void WriteThreadID(TextWriter output,bool useColor)
        {
            if (useColor)
            {
                WriteColor(output,Color);
            }
            output.Write(String.Format("[#{0}]", Thread.CurrentThread.ManagedThreadId));
            if (useColor)
            {
                output.Write("\x1b[0m");
            }
        }
        private void WriteColor(TextWriter output,object color)
        {
            if (color as Tuple<byte, byte, byte> != null)
            {
                var Tcolor = color as Tuple<byte, byte, byte>;
                output.Write(String.Format("\x1b[48;2;{0};{1};{2}m", Tcolor.Item1, Tcolor.Item2, Tcolor.Item3));
            }
            else if (color is ConsoleColor)
            {
                Console.BackgroundColor = (ConsoleColor)color;
            }
        }
    }
}