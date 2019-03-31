namespace PowerStateManagement.Test.ServiceStrructures
{
    using System.Runtime.InteropServices;

    internal struct BateryStateStruct
    {
        [MarshalAs(UnmanagedType.I1)] private bool AcOnLine;
        [MarshalAs(UnmanagedType.I1)] private bool BatteryPresent;
        [MarshalAs(UnmanagedType.I1)] private bool Charging;
        [MarshalAs(UnmanagedType.I1)] private bool Discharging;
        [MarshalAs(UnmanagedType.I1)] private bool Spare1;
        [MarshalAs(UnmanagedType.I1)] private bool Spare2;
        [MarshalAs(UnmanagedType.I1)] private bool Spare3;
        private byte Tag;
        private uint MaxCapacity;
        private uint RemainingCapacity;
        private uint Rate;
        private uint EstimatedTime;
        private uint DefaultAlert1;
        private uint DefaultAlert2;
    }
}