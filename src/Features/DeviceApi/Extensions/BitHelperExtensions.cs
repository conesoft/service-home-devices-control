using System.Collections;
using System;

namespace Conesoft.Services.HomeDevicesControl.Features.DeviceApi.Extensions;

static class BitHelperExtensions
{
    public static byte ToByte(this BitArray bits)
    {
        if (bits.Count != 8)
        {
            throw new ArgumentException(null, nameof(bits));
        }
        byte[] bytes = new byte[1];
        bits.CopyTo(bytes, 0);
        return bytes[0];
    }
    public static byte[] ToBytes(this BitArray bits)
    {
        byte[] bytes = new byte[bits.Length / 8];
        bits.CopyTo(bytes, 0);
        return bytes;
    }

    public static byte Reverse(this byte b)
    {
        int a = 0;
        for (int i = 0; i < 8; i++)
            if ((b & (1 << i)) != 0)
                a |= 1 << (7 - i);
        return (byte)a;
    }
}
