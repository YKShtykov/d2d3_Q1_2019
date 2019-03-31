namespace PowerStateManagemet
{
    using System;
    using System.Runtime.InteropServices;

    public static class PowerStateManagemetInterop
    {
        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint CallNtPowerInformation(
            int InformationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            IntPtr lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(
            bool bHibernate,
            bool bForce,
            bool bWakeupEventsDisabled
        );
    }
}