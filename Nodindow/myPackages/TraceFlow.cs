using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nodindow.myPackages
{
    static public class TraceFlow
    {
        static public Logs logs = new Logs();
        static public References references = new TraceFlow.References();
        public enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }
        public class LogEntry
        {
            public DateTime timestamp { get; set; }
            public LogLevel level { get; set; }
            public string message { get; set; }
            public Exception exception { get; set; }
            public StackFrame stackframe { get; set; }

            public LogEntry(LogLevel level, string message, Exception exception = null)
            {
                this.timestamp = DateTime.Now;
                this.level = level;
                this.message = message;
                this.exception = exception;
                this.stackframe = new StackTrace(true).GetFrame(1);
            }
            public void writeLog()
            {
                Console.WriteLine("__LogEntry__");
                Console.WriteLine(timestamp.ToString("dd/MM/yyyy HH:mm:ss") + $" Level: [{level}] Message: {message} File: {stackframe.GetFileName()} Line: {stackframe.GetFileLineNumber()} Method: {stackframe.GetMethod()}");
                if (exception != null)
                {
                    Console.WriteLine("Exception: " + exception.Message);
                    Console.WriteLine("StackTrace: " + exception.StackTrace);
                }
                Console.WriteLine("__End__");
            }
        }
        public class ReferenceObject
        {
            public DateTime timestamp { get; set; }
            public int id { get; set; }
            public object objRef { get; set; }
            public StackFrame stackframe { get; set; }
            public ReferenceObject(int id, object objRef)
            {
                this.id = id;
                this.objRef = objRef;
                this.stackframe = new StackTrace(true).GetFrame(1);
                this.timestamp = DateTime.Now;
            }
            public void writeLog()
            {
                Console.WriteLine(timestamp.ToString("dd/MM/yyyy HH:mm:ss") + $"Id: {this.id}" + $"File: {this.stackframe.GetFileName()} Line: {this.stackframe.GetFileLineNumber()} Method: {this.stackframe.GetMethod()}");
            }
        }
        public class Logs
        {
            public List<LogEntry> allLogs = new List<LogEntry>();
            public void add(LogEntry logEntry, bool writeLog = true)
            {
                this.allLogs.Add(logEntry);
                logEntry.writeLog();
            }
        }
        public class References
        {
            public List<ReferenceObject> allReferences = new List<ReferenceObject>();
            public void addRef(object objectRef)
            {
                this.allReferences.Add(new ReferenceObject(allReferences.Count, objectRef));
            }
            public List<ReferenceObject> getReferences(object obj, bool log = true)
            {
                List<ReferenceObject> referenceObject = this.allReferences.FindAll(reference => reference.objRef == obj);
                if (log == true)
                {
                    Console.WriteLine("__ReferenceObject__");
                    foreach (var item in referenceObject)
                    {
                        item.writeLog();
                    }
                    Console.WriteLine("__End__");
                }
                return referenceObject;
            }

        }
    }
}
