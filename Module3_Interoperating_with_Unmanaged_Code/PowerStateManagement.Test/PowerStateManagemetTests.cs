using static PowerStateManagemet.PowerStateManagemetInterop;

namespace PowerStateManagement.Test
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ServiceStrructures;

    [TestClass]
    public class PowerStateManagemetTests
    {
        public IntPtr AllocateMemory(Type type)
        {
            return Marshal.AllocCoTaskMem(Marshal.SizeOf(type));
        }

        [TestMethod]
        public void CallNtPowerInformation_LastSleepTime_Test()
        {
            var lastSleep = AllocateMemory(typeof(ulong));

            var status = CallNtPowerInformation((int) InformationLevel.LastSleepTime,
                IntPtr.Zero, 0, lastSleep, Marshal.SizeOf(typeof(ulong)));

            var battStatus = (ulong) Marshal.ReadInt64(lastSleep, 0);
            Marshal.FreeCoTaskMem(lastSleep);
            Console.WriteLine($"{battStatus}");

            Assert.AreEqual(0, (int) status);
        }

        [TestMethod]
        public void CallNtPowerInformation_LastWakeTime_Test()
        {
            var lastWakeTime = AllocateMemory(typeof(ulong));

            var status = CallNtPowerInformation((int) InformationLevel.LastWakeTime,
                IntPtr.Zero, 0, lastWakeTime, Marshal.SizeOf(typeof(ulong)));

            var battStatus = (ulong) Marshal.ReadInt64(lastWakeTime, 0);
            Marshal.FreeCoTaskMem(lastWakeTime);
            Console.WriteLine($"{battStatus}");

            Assert.AreEqual(0, (int) status);
        }

        [TestMethod]
        public void CallNtPowerInformation_SystemBatteryState_Test()
        {
            var sbs = AllocateMemory(typeof(BateryStateStruct));

            var status = CallNtPowerInformation((int) InformationLevel.SystemBatteryState,
                IntPtr.Zero, 0, sbs, Marshal.SizeOf(typeof(BateryStateStruct)));

            var battStatus = (BateryStateStruct) Marshal.PtrToStructure(sbs, typeof(BateryStateStruct));
            Marshal.FreeCoTaskMem(sbs);
            Console.WriteLine($"{battStatus}");

            Assert.AreEqual(0, (int) status);
        }

        [TestMethod]
        public void CallNtPowerInformation_SystemPowerInformation_Test()
        {
            var powerInfo = AllocateMemory(typeof(SystemPowerInfoStruct));

            var status = CallNtPowerInformation((int) InformationLevel.SystemPowerInformation,
                IntPtr.Zero, 0, powerInfo, Marshal.SizeOf(typeof(SystemPowerInfoStruct)));

            var info = (SystemPowerInfoStruct) Marshal.PtrToStructure(powerInfo, typeof(SystemPowerInfoStruct));
            Marshal.FreeCoTaskMem(powerInfo);
            Console.WriteLine($"{info}");

            Assert.AreEqual(0, (int) status);
        }

        [TestMethod]
        public void CallNtPowerInformation_SetSuspendState_Test()
        {
            var bHibernate = true;
            var bForce = true;
            var bWakeupEventsDisabled = false;

            var status = SetSuspendState(bHibernate, bForce, bWakeupEventsDisabled);

            Console.WriteLine(status);
            Assert.IsTrue(status);
        }
    }
}