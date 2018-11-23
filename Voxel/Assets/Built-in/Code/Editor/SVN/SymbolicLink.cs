using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;


public static class SymbolicLink
{

    static void Main(string[] args)
    {
        using (FileStream fs = File.OpenRead(@"D:\temp\case\mytest.txt"))
        {
            StringBuilder path = new StringBuilder(512);
            GetFinalPathNameByHandle(fs.SafeFileHandle.DangerousGetHandle(), path, path.Capacity, 0);
            Console.WriteLine(path.ToString());
        }
    }

    public static string GetRealPath(string path)
    {
        using (SafeFileHandle fileHandle = getFileHandle(path))
        {
            if (fileHandle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            var pathbuilder = new StringBuilder(512);

            GetFinalPathNameByHandle(fileHandle.DangerousGetHandle(), pathbuilder, pathbuilder.Capacity, 0);

            return path.ToString();
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int GetFinalPathNameByHandle(IntPtr handle, [In, Out] StringBuilder path, int bufLen, int flags);


    private const uint genericReadAccess = 0x80000000;

    private const uint fileFlagsForOpenReparsePointAndBackupSemantics = 0x02200000;

    private const int ioctlCommandGetReparsePoint = 0x000900A8;

    private const uint openExisting = 0x3;

    private const uint pathNotAReparsePointError = 0x80071126;

    private const uint shareModeAll = 0x7; // Read, Write, Delete

    private const uint symLinkTag = 0xA000000C;

    private const int targetIsAFile = 0;

    private const int targetIsADirectory = 1;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern SafeFileHandle CreateFile(
    string lpFileName,
    uint dwDesiredAccess,
    uint dwShareMode,
    IntPtr lpSecurityAttributes,
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(
    IntPtr hDevice,
    uint dwIoControlCode,
    IntPtr lpInBuffer,
    int nInBufferSize,
    IntPtr lpOutBuffer,
    int nOutBufferSize,
    out int lpBytesReturned,
    IntPtr lpOverlapped);

    public static void CreateDirectoryLink(string linkPath, string targetPath)
    {
        if (!CreateSymbolicLink(linkPath, targetPath, targetIsADirectory) || Marshal.GetLastWin32Error() != 0)
        {
            try
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            catch (COMException exception)
            {
                throw new IOException(exception.Message, exception);
            }
        }
    }

    public static void CreateFileLink(string linkPath, string targetPath)
    {
        if (!CreateSymbolicLink(linkPath, targetPath, targetIsAFile))
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
    }

    public static bool Exists(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            return false;
        }
        string target = GetTarget(path);
        return target != null;
    }

    private static SafeFileHandle getFileHandle(string path)
    {
        return CreateFile(path, genericReadAccess, shareModeAll, IntPtr.Zero, openExisting,
        fileFlagsForOpenReparsePointAndBackupSemantics, IntPtr.Zero);
    }

    public static string GetTarget(string path)
    {
        SymbolicLinkReparseData reparseDataBuffer;

        using (SafeFileHandle fileHandle = getFileHandle(path))
        {
            if (fileHandle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            int outBufferSize = Marshal.SizeOf(typeof(SymbolicLinkReparseData));
            IntPtr outBuffer = IntPtr.Zero;
            try
            {
                outBuffer = Marshal.AllocHGlobal(outBufferSize);
                int bytesReturned;
                bool success = DeviceIoControl(
                fileHandle.DangerousGetHandle(), ioctlCommandGetReparsePoint, IntPtr.Zero, 0,
                outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero);

                fileHandle.Close();

                if (!success)
                {
                    var errorCode = Marshal.GetLastWin32Error();

                    if (errorCode == 4390)
                    {
                        throw new Exception("这个目录不是一个硬链接目录!");
                    }

                    if (((uint)Marshal.GetHRForLastWin32Error()) == pathNotAReparsePointError)
                    {
                        return null;
                    }
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                reparseDataBuffer = (SymbolicLinkReparseData)Marshal.PtrToStructure(
                outBuffer, typeof(SymbolicLinkReparseData));
            }
            finally
            {
                Marshal.FreeHGlobal(outBuffer);
            }
        }

        var target1 = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer).Trim();

        target1 = ResolvePath(target1);
        return target1;

        if (reparseDataBuffer.ReparseTag != symLinkTag)
        {
            return null;
        }

        string target = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
        reparseDataBuffer.PrintNameOffset, reparseDataBuffer.PrintNameLength);

        return target;
    }

    /// <summary>
    /// 解决一下路径
    /// </summary>
    /// <param name="target1"></param>
    /// <returns></returns>
    private static string ResolvePath(string target1)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < target1.Length; i++)
        {
			if (target1 [i] != '\0') {
				sb.Append (target1 [i]);
			} else {
				break;
			}
        }

        sb.Remove(0, 2);

        return sb.ToString();
    }
}
