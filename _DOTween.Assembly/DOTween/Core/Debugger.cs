﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/03 11:33
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core.Enums;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    /// <summary>
    /// Public so it can be used by lose scripts related to DOTween (like DOTweenAnimation)
    /// </summary>
    public static class Debugger
    {
        // Commented this to prevent DOTween.Init being called by eventual logs that might happen during ApplicationQuit
//        public static int logPriority { get { if (!DOTween.initialized) DOTween.Init(); return _logPriority; } }
        // 0: errors only - 1: default - 2: verbose
        public static int logPriority { get { return _logPriority; } }
        static int _logPriority;

        const string _LogPrefix = "<color=#0099bc><b>DOTWEEN ► </b></color>";

        #region Public Methods
        
        public static void Log(object message)
        {
            string txt = _LogPrefix + message;
            if (DOTween.onWillLog != null && !DOTween.onWillLog(LogType.Log, txt)) return;
            Debug.Log(txt);
        }
        public static void LogWarning(string message, Tween t = null)
        {
            string txt;
            /*if (DOTween.debugMode)*/ txt = _LogPrefix + GetDebugDataMessage(t) + message;
           /* else txt = _LogPrefix + message;*/
            Debug.Log("LogWarning 1 "+txt);
            if (DOTween.onWillLog != null && !DOTween.onWillLog(LogType.Warning, txt)) return;
            Debug.Log("LogWarning 2 "+txt);
            Debug.LogWarning(txt);
        }
        public static void LogError(string message, Tween t = null)
        {
            string txt;
           /* if (DOTween.debugMode) */ txt = _LogPrefix + GetDebugDataMessage(t) + message;
           /* else txt = _LogPrefix + message;*/
            if (DOTween.onWillLog != null && !DOTween.onWillLog(LogType.Error, txt)) return;
            Debug.LogError(txt);
        }
        public static void LogSafeModeCapturedError(object message, Tween t = null)
        {
            string txt;
            if (DOTween.debugMode)
            {
                txt = _LogPrefix + GetDebugDataMessage(t) + message;
            }
            else txt = _LogPrefix + message;
            if (DOTween.onWillLog != null && !DOTween.onWillLog(LogType.Log, txt)) return;
            switch (DOTween.safeModeLogBehaviour) {
            case SafeModeLogBehaviour.Normal:
                Debug.Log(txt);
                break;
            case SafeModeLogBehaviour.Warning:
                Debug.LogWarning(txt);
                break;
            case SafeModeLogBehaviour.Error:
                Debug.LogError(txt);
                break;
            }
        }

        public static void LogReport(object message)
        {
            string txt = string.Format("<color=#00B500FF>{0} REPORT ►</color> {1}", _LogPrefix, message);
            if (DOTween.onWillLog != null && !DOTween.onWillLog(LogType.Log, txt)) return;
            Debug.Log(txt);
        }

        public static void LogSafeModeReport(object message)
        {
            string txt = string.Format("<color=#ff7337>{0} SAFE MODE ►</color> {1}", _LogPrefix, message);
            if (DOTween.onWillLog != null && !DOTween.onWillLog(LogType.Log, txt)) return;
            Debug.LogWarning(txt);
        }

        public static void LogInvalidTween(Tween t)
        {
            LogWarning("This Tween has been killed and is now invalid");
        }

//        public static void LogNullTarget()
//        {
//            LogWarning("The target for this tween shortcut is null");
//        }

        public static void LogNestedTween(Tween t)
        {
            LogWarning("This Tween was added to a Sequence and can't be controlled directly", t);
        }

        public static void LogNullTween(Tween t)
        {
            LogWarning("Null Tween");
        }

        public static void LogNonPathTween(Tween t)
        {
            LogWarning("This Tween is not a path tween", t);
        }

        public static void LogMissingMaterialProperty(string propertyName)
        {
            LogWarning(string.Format("This material doesn't have a {0} property", propertyName));
        }
        public static void LogMissingMaterialProperty(int propertyId)
        {
            LogWarning(string.Format("This material doesn't have a {0} property ID", propertyId));
        }

        public static void LogRemoveActiveTweenError(string errorInfo, Tween t)
        {
            LogWarning(string.Format(
                "Error in RemoveActiveTween ({0}). It's been taken care of so no problems, but Daniele (DOTween's author) is trying to pinpoint it (it's very rare and he can't reproduce it) so it would be awesome if you could reproduce this log in a sample project and send it to him. Or even just write him the complete log that was generated by this message. Fixing this would make DOTween slightly faster. Thanks.", errorInfo
            ), t);
        }

        public static void LogAddActiveTweenError(string errorInfo, Tween t)
        {
            LogWarning(string.Format(
                "Error in AddActiveTween ({0}). It's been taken care of so no problems, but Daniele (DOTween's author) is trying to pinpoint it (it's very rare and he can't reproduce it) so it would be awesome if you could reproduce this log in a sample project and send it to him. Or even just write him the complete log that was generated by this message. Fixing this would make DOTween slightly faster. Thanks.", errorInfo
            ), t);
        }

        public static void SetLogPriority(LogBehaviour logBehaviour)
        {
            switch (logBehaviour) {
            case LogBehaviour.Default:
                _logPriority = 1;
                break;
            case LogBehaviour.Verbose:
                _logPriority = 2;
                break;
            default:
                _logPriority = 0;
                break;
            }
        }

        public static bool ShouldLogSafeModeCapturedError()
        {
            switch (DOTween.safeModeLogBehaviour) {
            case SafeModeLogBehaviour.None:
                return false;
            case SafeModeLogBehaviour.Normal:
            case SafeModeLogBehaviour.Warning:
                return _logPriority >= 1;
            default:
                return true;
            }
        }

        #endregion

        #region Methods

        static string GetDebugDataMessage(Tween t)
        {
            if(t == null){
                return $"The Tween is null!";
            }
            
            Debug.Log("MemberName "+ t.memberName);
            Debug.Log("lineNumber "+ t.lineNumber);
            Debug.Log("sourceFilePath "+ t.sourceFilePath);
            return $" Play called from: {t.memberName}@{t.lineNumber} in {t.sourceFilePath}";
        }

        static void AddDebugDataToMessage(ref string message, Tween t)
        {
            if (t == null) return;
            bool hasDebugTargetId = t.debugTargetId != null;
            bool hasStringId = t.stringId != null;
            bool hasIntId = t.intId != -999;
            if (hasDebugTargetId || hasStringId || hasIntId) {
                message += "DEBUG MODE INFO ► ";
                if (hasDebugTargetId) message += string.Format("[tween target: {0}]", t.debugTargetId);
                if (hasStringId) message += string.Format("[stringId: {0}]", t.stringId);
                if (hasIntId) message += string.Format("[intId: {0}]", t.intId);
                message += "\n";
            }
            message += $" Play called from: {t.memberName}@{t.lineNumber} in {t.sourceFilePath}";
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        internal static class Sequence
        {
            public static void LogAddToNullSequence()
            {
                LogWarning("You can't add elements to a NULL Sequence");
            }

            public static void LogAddToInactiveSequence()
            {
                LogWarning("You can't add elements to an inactive/killed Sequence");
            }

            public static void LogAddToLockedSequence()
            {
                LogWarning("The Sequence has started and is now locked, you can only elements to a Sequence before it starts");
            }

            public static void LogAddNullTween()
            {
                LogWarning("You can't add a NULL tween to a Sequence");
            }

            public static void LogAddInactiveTween(Tween t)
            {
                LogWarning("You can't add an inactive/killed tween to a Sequence", t);
            }

            public static void LogAddAlreadySequencedTween(Tween t)
            {
                LogWarning("You can't add a tween that is already nested into a Sequence to another Sequence", t);
            }
        }
    }
}