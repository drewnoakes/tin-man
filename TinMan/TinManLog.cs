﻿/*
 * Created by Drew, 10/06/2010 03:26.
 */
using System;

namespace TinMan
{
    public sealed class Log {
        public static Action<string,object[]> InfoAction { get; set; }
        public static Action<string,object[]> VerboseAction { get; set; }
        public static Action<string,object[]> WarnAction { get; set; }
        public static Action<string,object[],Exception> ErrorAction { get; set; }
        
        private static readonly Log _instance = new Log();
        
        public static Log Create() {
            // maintain a single instance for now.  in future we might do something fancier
            // like using reflection on the callstack within this method
            return _instance;
        }
        
        static Log() {
            VerboseAction = (m,i) => WriteConsole(string.Format(m,i), ConsoleColor.Gray, ConsoleColor.Black);
            InfoAction = (m,i) => WriteConsole(string.Format(m,i), ConsoleColor.White, ConsoleColor.Black);
            WarnAction = (m,i) => WriteConsole(string.Format(m,i), ConsoleColor.Magenta, ConsoleColor.Black);
            ErrorAction = (m,i,ex) => WriteConsole(string.Format(m,i)+Environment.NewLine+ex, ConsoleColor.White, ConsoleColor.Red);
        }
        
        private static object _consoleLock = new object();
        private static void WriteConsole(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
            lock (_consoleLock) {
                var oldForeColor = Console.ForegroundColor;
                var oldBackColor = Console.BackgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
                Console.WriteLine(message);
                Console.ForegroundColor = oldForeColor;
                Console.BackgroundColor = oldBackColor;
            }
        }
        
        private Log()
        {}
        
        public void Verbose(string format, params object[] items) {
            VerboseAction(format, items);
        }
        
        public void Info(string format, params object[] items) {
            InfoAction(format, items);
        }
        
        public void Warn(string format, params object[] items) {
            WarnAction(format, items);
        }
        
        public void Error(string format, params object[] items) {
            ErrorAction(format, items, null);
        }
        
        public void Error(Exception exception, string format, params object[] items) {
            ErrorAction(format, items, exception);
        }
    }
}