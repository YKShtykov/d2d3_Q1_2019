namespace PowerStateManagement.Test.ServiceStrructures
{
    internal struct SystemPowerInfoStruct
    {
        public uint MaxIdlenessAllowed;
        public uint Idleness;
        public uint TimeRemaining;
        public byte CoolingMode;
    }
}