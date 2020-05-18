using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Brimborium.Extensions.Abstractions {
    public class TracedDisposableControl {
        private static TracedDisposableControl _Instance;
        public static TracedDisposableControl Instance => _Instance ?? (_Instance = new TracedDisposableControl());

        private bool _IsTraceEnabledForAll = false;
        private Dictionary<Type, bool> _IsTraceEnabledForType;

        public void SetTraceEnabledForAll(bool value) {
            this._IsTraceEnabledForAll = value;
        }

        public void SetTraceEnabledForType(System.Type type, bool value) {
            //for(int watchdog = 1000; watchdog > 0; watchdog--) {
            while (true) {
                Dictionary<Type, bool> oldDict = this._IsTraceEnabledForType;
                Dictionary<Type, bool> nextDict
                    = (this._IsTraceEnabledForType is null)
                    ? new Dictionary<Type, bool>()
                    : new Dictionary<Type, bool>(oldDict);
                nextDict[type] = value;
                var prevDict = System.Threading.Interlocked.CompareExchange(ref this._IsTraceEnabledForType, nextDict, oldDict);
                if (ReferenceEquals(prevDict, oldDict)) {
                    return;
                }
            }
        }

        public bool IsTraceEnabled(System.Type type) {
            if (this._IsTraceEnabledForAll) { return true; }
            var dict = this._IsTraceEnabledForType;
            if (type is object && dict is object) {
                if (dict.TryGetValue(type, out var result)) {
                    return result;
                }
            }
            return false;
        }

        private Action<ReportFinalizedInfo> _CurrentReportFinalized;

        public Action<ReportFinalizedInfo> CurrentReportFinalized {
            get {
                return this._CurrentReportFinalized;
            }

            set {
                this._CurrentReportFinalized = value;
            }
        }

        public static void DummyReportFinalized(ReportFinalizedInfo reportFinalizedInfo) {
        }

        public void ReportFinalized(ReportFinalizedInfo reportFinalizedInfo) {
            if (this.CurrentReportFinalized != null) {
                this.CurrentReportFinalized(reportFinalizedInfo);
            }
        }

        public static string GetStackTrace() {
            var sb = new StringBuilder();
            try {
                var st = new StackTrace(1, true);
                var frames = st.GetFrames();
                foreach (var frame in frames) {
                    string declaringTypeName = null;
                    string methodName = null;
                    string fileName = null;
                    int lineNumber = 0;
                    int columnNumber = 0;
                    try {
                        var method = frame.GetMethod();
                        declaringTypeName = method?.DeclaringType?.Name;
                        methodName = method?.Name;
                        fileName = frame.GetFileName();
                        lineNumber = frame.GetFileLineNumber();
                        columnNumber = frame.GetFileColumnNumber();
                    } catch { }
                    sb.Append(declaringTypeName).Append(".").Append(methodName)
                       .Append(" fileName:").Append(fileName).Append("@").Append(lineNumber).Append(":").Append(columnNumber)
                       .AppendLine();
                }
                return sb.ToString();
            } catch {
                sb.Append("??").AppendLine();
                return sb.ToString();
            }
        }
    }
    public struct ReportFinalizedInfo {
        public Type Type;
        public string CtorStackTrace;
    }
}