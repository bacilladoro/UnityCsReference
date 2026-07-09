// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;

namespace UnityEngine.Android
{
    public enum LaunchMode
    {
        /// <summary>
        /// <para>The standard "standard" launch mode of an activity, which can have multiple instances and can be instantiated in any task.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#LAUNCH_MODE_STANDARD">developer.android.com</seealso>
        /// </summary>
        Standard = 0,

        /// <summary>
        /// <para>The "singleTop" launch mode of an activity. If there is an existing instance of the activity class in the task that would handle the intent, the system routes the intent to that instance instead of creating a new instance.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#LAUNCH_MODE_SINGLE_TOP">developer.android.com</seealso>
        /// </summary>
        SingleTop = 1,

        /// <summary>
        /// <para>The "singleInstance" launch mode of an activity. The system creates the activity at the root of a new task and routes the intent to it. If the instance already exists, the system routes the intent to existing instance through a call to its onNewIntent() method, instead of creating a new instance.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#LAUNCH_MODE_SINGLE_INSTANCE">developer.android.com</seealso>
        /// </summary>
        SingleInstance = 2,

        /// <summary>
        /// <para>The "singleTask" launch mode of an activity. The system creates a new task and instantiates the activity at the root of the new task. However, if an instance of the activity already exists in a separate task, the system routes the intent to the existing instance through a call to its onNewIntent() method, instead of creating a new instance.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#LAUNCH_MODE_SINGLE_TASK">developer.android.com</seealso>
        /// </summary>
        SingleTask = 3,

        /// <summary>
        /// <para>The "singleInstancePerTask" launch mode of an activity. The activity can only be running as the root activity of the task, but multiple instances of the task with this activity can be created.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#LAUNCH_MODE_SINGLE_INSTANCE_PER_TASK">developer.android.com</seealso>
        /// </summary>
        SingleInstancePerTask = 4
    }

    public enum StartReason
    {
        /// <summary>
        /// <para>The process was started to handle an alarm.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_ALARM">developer.android.com</seealso>
        /// </summary>
        Alarm = 0,

        /// <summary>
        /// <para>The process was started to handle a backup.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_BACKUP">developer.android.com</seealso>
        /// </summary>
        Backup = 1,

        /// <summary>
        /// <para>The process was started to handle a boot complete broadcast.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_BOOT_COMPLETE">developer.android.com</seealso>
        /// </summary>
        BootComplete = 2,

        /// <summary>
        /// <para>The process was started to handle a broadcast.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_BROADCAST">developer.android.com</seealso>
        /// </summary>
        Broadcast = 3,

        /// <summary>
        /// <para>The process was started to handle a content provider.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_CONTENT_PROVIDER">developer.android.com</seealso>
        /// </summary>
        ContentProvider = 4,

        /// <summary>
        /// <para>The process was started to handle a job.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_JOB">developer.android.com</seealso>
        /// </summary>
        Job = 5,

        /// <summary>
        /// <para>The process was started from the launcher.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_LAUNCHER">developer.android.com</seealso>
        /// </summary>
        Launcher = 6,

        /// <summary>
        /// <para>The process was started from the launcher recents.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_LAUNCHER_RECENTS">developer.android.com</seealso>
        /// </summary>
        LauncherRecents = 7,

        /// <summary>
        /// <para>The process was started for some other reason.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_OTHER">developer.android.com</seealso>
        /// </summary>
        Other = 8,

        /// <summary>
        /// <para>The process was started to handle a push.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_PUSH">developer.android.com</seealso>
        /// </summary>
        Push = 9,

        /// <summary>
        /// <para>The process was started to handle a service.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_SERVICE">developer.android.com</seealso>
        /// </summary>
        Service = 10,

        /// <summary>
        /// <para>The process was started to handle a start activity.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_REASON_START_ACTIVITY">developer.android.com</seealso>
        /// </summary>
        StartActivity = 11
    }

    public enum StartType
    {
        /// <summary>
        /// <para>Start type was not set.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TYPE_UNSET">developer.android.com</seealso>
        /// </summary>
        Unset = 0,

        /// <summary>
        /// <para>Cold start - the process was started from scratch.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TYPE_COLD">developer.android.com</seealso>
        /// </summary>
        Cold = 1,

        /// <summary>
        /// <para>Warm start - the process was brought back from a stopped state.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TYPE_WARM">developer.android.com</seealso>
        /// </summary>
        Warm = 2,

        /// <summary>
        /// <para>Hot start - the process was already running in the background.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TYPE_HOT">developer.android.com</seealso>
        /// </summary>
        Hot = 3
    }

    public enum StartupState
    {
        /// <summary>
        /// <para>The startup was started.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#STARTUP_STATE_STARTED">developer.android.com</seealso>
        /// </summary>
        Started = 0,

        /// <summary>
        /// <para>The startup encountered an error.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#STARTUP_STATE_ERROR">developer.android.com</seealso>
        /// </summary>
        Error = 1,

        /// <summary>
        /// <para>The first frame was drawn.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#STARTUP_STATE_FIRST_FRAME_DRAWN">developer.android.com</seealso>
        /// </summary>
        FirstFrameDrawn = 2
    }

    public enum StartComponent
    {
        /// <summary>
        /// <para>The component that was started was an Activity.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_COMPONENT_ACTIVITY">developer.android.com</seealso>
        /// </summary>
        Activity = 1,

        /// <summary>
        /// <para>The component that was started was a Broadcast receiver.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_COMPONENT_BROADCAST">developer.android.com</seealso>
        /// </summary>
        Broadcast = 2,

        /// <summary>
        /// <para>The component that was started was a Content provider.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_COMPONENT_CONTENT_PROVIDER">developer.android.com</seealso>
        /// </summary>
        ContentProvider = 3,

        /// <summary>
        /// <para>The component that was started was a Service.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_COMPONENT_SERVICE">developer.android.com</seealso>
        /// </summary>
        Service = 4,

        /// <summary>
        /// <para>The component that was started was something else.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_COMPONENT_OTHER">developer.android.com</seealso>
        /// </summary>
        Other = 5
    }

    public enum StartTimestamp
    {
        /// <summary>
        /// <para>Clock monotonic timestamp of launch started.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_LAUNCH">developer.android.com</seealso>
        /// </summary>
        Launch = 0,

        /// <summary>
        /// <para>Clock monotonic timestamp of process fork.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_FORK">developer.android.com</seealso>
        /// </summary>
        Fork = 1,

        /// <summary>
        /// <para>Clock monotonic timestamp of Application onCreate called.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_APPLICATION_ONCREATE">developer.android.com</seealso>
        /// </summary>
        ApplicationOnCreate = 2,

        /// <summary>
        /// <para>Clock monotonic timestamp of bindApplication called.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_BIND_APPLICATION">developer.android.com</seealso>
        /// </summary>
        BindApplication = 3,

        /// <summary>
        /// <para>Clock monotonic timestamp of first frame drawn.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_FIRST_FRAME">developer.android.com</seealso>
        /// </summary>
        FirstFrame = 4,

        /// <summary>
        /// <para>Clock monotonic timestamp of reportFullyDrawn called by application.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_FULLY_DRAWN">developer.android.com</seealso>
        /// </summary>
        FullyDrawn = 5,

        /// <summary>
        /// <para>Clock monotonic timestamp of initial renderthread frame.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_INITIAL_RENDERTHREAD_FRAME">developer.android.com</seealso>
        /// </summary>
        InitialRenderthreadFrame = 6,

        /// <summary>
        /// <para>Clock monotonic timestamp of surfaceflinger composition complete.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_SURFACEFLINGER_COMPOSITION_COMPLETE">developer.android.com</seealso>
        /// </summary>
        SurfaceflingerCompositionComplete = 7,

        /// <summary>
        /// <para>The end of the range, beginning with 0, reserved for system timestamps.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_RESERVED_RANGE_SYSTEM">developer.android.com</seealso>
        /// </summary>
        ReservedRangeSystem = 20,

        /// <summary>
        /// <para>The beginning of the range reserved for developer supplied timestamps.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_RESERVED_RANGE_DEVELOPER_START">developer.android.com</seealso>
        /// </summary>
        ReservedRangeDeveloperStart = 21,

        /// <summary>
        /// <para>The end of the range reserved for developer supplied timestamps.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#START_TIMESTAMP_RESERVED_RANGE_DEVELOPER">developer.android.com</seealso>
        /// </summary>
        ReservedRangeDeveloper = 30
    }

    public interface IApplicationStartInfo
    {
        /// <summary>
        /// <para>Return the process id of the process that was started.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getPid()">developer.android.com</seealso>
        /// </summary>
        /// <returns>int</returns>
        int pid { get; }

        /// <summary>
        /// <para>Return the defining kernel user identifier, maybe different from getRealUid() and getPackageUid(), if an external service has the android:useAppZygote set to true and was bound with the flag Context.BIND_EXTERNAL_SERVICE - in this case, this field here will be the kernel user identifier of the external service provider.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getDefiningUid()">developer.android.com</seealso>
        /// </summary>
        /// <returns>int</returns>
        int definingUid { get; }

        /// <summary>
        /// <para>Similar to getRealUid(), it's the kernel user identifier that is assigned at the package installation time.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getPackageUid()">developer.android.com</seealso>
        /// </summary>
        /// <returns>int</returns>
        int packageUid { get; }

        /// <summary>
        /// <para>Return the kernel user identifier of the process, most of the time the system uses this to do access control checks. It's typically the uid of the package where the component is running from, except the case of external service, where getDefiningUid() will be the same as the package uid of the component.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getRealUid()">developer.android.com</seealso>
        /// </summary>
        /// <returns>int</returns>
        int realUid { get; }

        /// <summary>
        /// <para>Return the actual process name it was running with.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getProcessName()">developer.android.com</seealso>
        /// </summary>
        /// <returns>string</returns>
        string processName { get; }

        /// <summary>
        /// <para>Return the reason code for why the process was started.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getReason()">developer.android.com</seealso>
        /// </summary>
        /// <returns>StartReason</returns>
        StartReason reason { get; }

        /// <summary>
        /// <para>Return the type of start, which can be one of "cold", "warm" or "hot".</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getStartType()">developer.android.com</seealso>
        /// </summary>
        /// <returns>StartType</returns>
        StartType startType { get; }

        /// <summary>
        /// <para>Return the startup state of the process.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getStartupState()">developer.android.com</seealso>
        /// </summary>
        /// <returns>StartupState</returns>
        StartupState startupState { get; }

        /// <summary>
        /// <para>Return the launch mode that was used to start the activity, if this start was initiated with an activity launch; return 0 otherwise.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getLaunchMode()">developer.android.com</seealso>
        /// </summary>
        /// <returns>LaunchMode</returns>
        LaunchMode launchMode { get; }

        /// <summary>
        /// <para>Return the URI string representation of the intent used to launch the activity (via Intent.toUri(0)), if this start was initiated with an activity launch; return null otherwise.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getIntent()">developer.android.com</seealso>
        /// </summary>
        /// <returns>URI string of the launch Intent, or null if there was no intent.</returns>
        string intentUri { get; }

        /// <summary>
        /// <para>Return the timestamps (see <see href="https://developer.android.com/reference/android/os/SystemClock#uptimeMillis()">SystemClock.uptimeMillis</see>) collected during the startup of the application, keyed by the integer constants with the START_TIMESTAMP_ prefix in this class. The system could collect a specific timestamp only if the conditions of the specific startup transition are met.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getStartupTimestamps()">developer.android.com</seealso>
        /// </summary>
        /// <returns>A read-only dictionary mapping StartTimestamp type to timestamp values in milliseconds. This value cannot be null.</returns>
        IReadOnlyDictionary<StartTimestamp, long> startupTimestamps { get; }

        /// <summary>
        /// <para>Return whether the process was in a force-stopped state when it was started.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#wasForceStopped()">developer.android.com</seealso>
        /// </summary>
        /// <returns>bool</returns>
        bool wasForceStopped { get; }

        /// <summary>
        /// <para>Return the component that was started. Available only on API level 36+, returns 0 on earlier versions.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ApplicationStartInfo#getStartComponent()">developer.android.com</seealso>
        /// </summary>
        /// <returns>StartComponent. Returns <c>0</c> on API &lt; 36.</returns>
        StartComponent startComponent { get; }
    }


    public static class ApplicationStartInfoProvider
    {
        /// <summary>
        /// <para>Return a list of ApplicationStartInfo records containing the reasons for the most recent app starts.</para>
        /// <seealso href="https://developer.android.com/reference/android/app/ActivityManager#getHistoricalProcessStartReasons(int)">developer.android.com</seealso>
        /// </summary>
        /// <param name="maxNum">The maximum number of results to be returned; a value of 0 means to ignore this parameter and return all matching records. Value is 0 or greater.</param>
        /// <returns>IApplicationStartInfo[] a list of ApplicationStartInfo records matching the criteria, sorted in the order from most recent to least recent. This value cannot be null.</returns>
        public static IApplicationStartInfo[] GetHistoricalProcessStartReasons(int maxNum = 0)
        {
            IApplicationStartInfo[] result = null;
            if (result == null)
                result = Array.Empty<IApplicationStartInfo>();

            return result;
        }
    }
}
