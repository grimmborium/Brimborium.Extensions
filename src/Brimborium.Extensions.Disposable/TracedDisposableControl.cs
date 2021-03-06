﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Brimborium.Extensions.Disposable {
    public class TracedDisposableControl {
        private static TracedDisposableControl _Instance;
        public static TracedDisposableControl Instance => _Instance ?? (_Instance = new TracedDisposableControl());

        private bool _IsTraceEnabledForAll = false;
        private Dictionary<Type, bool> _IsTraceEnabledForType;

        public void SetTraceEnabledForAll(bool value) {
            this._IsTraceEnabledForAll = value;
        }

        public void SetTraceEnabledForType(System.Type type, bool value) {
            InterlockedUtilty.SetNextValue(
                ref this._IsTraceEnabledForType,
                (type: type, value: value),
                (Dictionary<Type, bool>? oldDict, (Type type, bool value) arg) => {
                    var nextDict
                        = (oldDict is null)
                            ? new Dictionary<Type, bool>()
                            : new Dictionary<Type, bool>(oldDict);
                    nextDict[arg.type] = arg.value;
                    return nextDict;
                },
                null);
        }

        public static void ReportFinalized(
            TracedDisposableControl tracedDisposableControl,
            ReportFinalizedInfo reportFinalizedInfo) {
            try {
                (tracedDisposableControl ?? TracedDisposableControl.Instance)?
                    .ReportFinalized(reportFinalizedInfo);
            } catch {
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
            try {
                if (this.CurrentReportFinalized != null) {
                    this.CurrentReportFinalized(reportFinalizedInfo);
                }
            } catch {
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
                return sb.ToString();
            }
        }
    }
}