using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

[ComVisible(false)]
public class FilterCategory     // uuids.h  :  CLSID_*
{
    /// <summary> CLSID_AudioInputDeviceCategory, audio capture category </summary>
    public static readonly Guid AudioInputDevice = new Guid(0x33d9a762, 0x90c8, 0x11d0, 0xbd, 0x43, 0x00, 0xa0, 0xc9, 0x11, 0xce, 0x86);

    /// <summary> CLSID_VideoInputDeviceCategory, video capture category </summary>
    public static readonly Guid VideoInputDevice = new Guid(0x860BB310, 0x5D01, 0x11d0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);
}

[ComVisible(false)]
public class Clsid      // uuids.h  :  CLSID_*
{
    /// <summary> CLSID_SystemDeviceEnum for ICreateDevEnum </summary>
    public static readonly Guid SystemDeviceEnum = new Guid(0x62BE5D10, 0x60EB, 0x11d0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

    /// <summary> CLSID_FilterGraph, filter Graph </summary>
    public static readonly Guid FilterGraph = new Guid(0xe436ebb3, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

    /// <summary> CLSID_CaptureGraphBuilder2, new Capture graph building </summary>
    public static readonly Guid CaptureGraphBuilder2 = new Guid(0xBF87B6E1, 0x8C27, 0x11d0, 0xB3, 0xF0, 0x0, 0xAA, 0x00, 0x37, 0x61, 0xC5);

    /// <summary> CLSID_SampleGrabber, Sample Grabber filter </summary>
    public static readonly Guid SampleGrabber = new Guid(0xC1F400A0, 0x3F08, 0x11D3, 0x9F, 0x0B, 0x00, 0x60, 0x08, 0x03, 0x9E, 0x37);

    /// <summary> CLSID_DvdGraphBuilder,  DVD graph builder </summary>
    public static readonly Guid DvdGraphBuilder = new Guid(0xFCC152B7, 0xF372, 0x11d0, 0x8E, 0x00, 0x00, 0xC0, 0x4F, 0xD7, 0xC0, 0x8B);

}

[ComVisible(false)]
public class DevEnum
{
    public static bool GetDevicesOfCat(Guid cat, out ArrayList devs)
    {
        devs = null;
        int hr;
        object comObj = null;
        ICreateDevEnum enumDev = null;
        IEnumMoniker enumMon = null;
        IMoniker[] mon = new IMoniker[1];
        try
        {
            Type srvType = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
            if (srvType == null)
                throw new NotImplementedException("System Device Enumerator");

            comObj = Activator.CreateInstance(srvType);
            enumDev = (ICreateDevEnum)comObj;
            hr = enumDev.CreateClassEnumerator(ref cat, out enumMon, 0);
            if (hr != 0)
                throw new NotSupportedException("No devices of the category");

            int /*f, */count = 0;
            IntPtr ptr = new IntPtr();
            do
            {
                hr = enumMon.Next(1, mon, ptr);
                if ((hr != 0) || (mon[0] == null))
                    break;
                DsDevice dev = new DsDevice();
                GetFriendlyName(mon[0], ref dev.Name, ref dev.Path);
                if (devs == null)
                    devs = new ArrayList();
                dev.id = count;
                dev.Mon = mon[0]; mon[0] = null;
                devs.Add(dev); dev = null;
                count++;
            }
            while (true);

            return count > 0;
        }
        catch (Exception)
        {
            if (devs != null)
            {
                foreach (DsDevice d in devs)
                    d.Dispose();
                devs = null;
            }
            return false;
        }
        finally
        {
            enumDev = null;
            if (mon[0] != null)
                Marshal.ReleaseComObject(mon[0]); mon[0] = null;
            if (enumMon != null)
                Marshal.ReleaseComObject(enumMon); enumMon = null;
            if (comObj != null)
                Marshal.ReleaseComObject(comObj); comObj = null;
        }

    }

    private static bool GetFriendlyName(IMoniker mon, ref string devname, ref string devpath)
    {
        object bagObj = null;
        IPropertyBag bag = null;
        try
        {
            Guid bagId = typeof(IPropertyBag).GUID;
            mon.BindToStorage(null, null, ref bagId, out bagObj);
            bag = (IPropertyBag)bagObj;
            object val = "";
            int hr = bag.Read("Description", ref val, IntPtr.Zero);
            if (hr != 0)
                hr = bag.Read("FriendlyName", ref val, IntPtr.Zero);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
            devname = val as string;
            if ((devname == null) || (devname.Length < 1))
                throw new NotImplementedException("Device FriendlyName");
            val = "";
            hr = bag.Read("DevicePath", ref val, IntPtr.Zero);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
            devpath = val as string;
            if ((devpath == null) || (devpath.Length < 1))
                throw new NotImplementedException("Device Path");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            bag = null;
            if (bagObj != null)
                Marshal.ReleaseComObject(bagObj); bagObj = null;
        }
    }
}


[ComVisible(false)]
public class DsDevice : IDisposable
{
    public int id;
    public string Name;
    public string Path;
    public IMoniker Mon;


    public void Dispose()
    {
        if (Mon != null)
            Marshal.ReleaseComObject(Mon); Mon = null;
    }
}

[ComVisible(true), ComImport,
Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICreateDevEnum
{
    [PreserveSig]
    int CreateClassEnumerator(
    [In]                                            ref Guid pType,
    [Out]                                       out IEnumMoniker ppEnumMoniker,
    [In]                                            int dwFlags);
}



[ComVisible(true), ComImport,
Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IPropertyBag
{
    [PreserveSig]
    int Read(
    [In, MarshalAs(UnmanagedType.LPWStr)]           string pszPropName,
    [In, Out, MarshalAs(UnmanagedType.Struct)]  ref object pVar,
    IntPtr pErrorLog);

    [PreserveSig]
    int Write(
    [In, MarshalAs(UnmanagedType.LPWStr)]           string pszPropName,
    [In, MarshalAs(UnmanagedType.Struct)]       ref object pVar);
}