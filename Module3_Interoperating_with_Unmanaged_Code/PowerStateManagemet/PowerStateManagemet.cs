namespace PowerStateManagemet
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    [Guid("dfad8fd7-65f6-4974-b060-7dd9a143b75a")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class PowerStateManagemet : IPowerStateManagement
    {
        [DispId(1)]
        public uint CallNtPowerInformation(int informationLevel, IntPtr lpInputBuffer, int nInputBufferSize,
            IntPtr lpOutputBuffer,
            int nOutputBufferSize)
        {
            return PowerStateManagemetInterop.CallNtPowerInformation(informationLevel, lpInputBuffer,
                nInputBufferSize, lpOutputBuffer, nOutputBufferSize);
        }

        [DispId(2)]
        public bool SetSuspendState(bool bHibernate, bool bWakeupEventsDisabled)
        {
            return PowerStateManagemetInterop.SetSuspendState(bHibernate, false, bWakeupEventsDisabled);
        }
    }
}