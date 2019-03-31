namespace PowerStateManagemet
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("4be60bb2-45a4-4ef2-bab9-5ade2088af63")]
    internal interface IPowerStateManagement
    {
        uint CallNtPowerInformation(int informationLevel, IntPtr lpInputBuffer, int nInputBufferSize,
            IntPtr lpOutputBuffer, int nOutputBufferSize);

        bool SetSuspendState(bool bHibernate, bool bWakeupEventsDisabled);
    }
}