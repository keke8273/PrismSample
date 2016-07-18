
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

//Based on the CRC calculator from the hydrix common code /include/util/crc32_calculator_win.h

using System;
using DataLinkLayer.Diagnostics;

namespace DataLinkLayer.Utils
{

   public static class CRC32
   {
       #region Constructors

       /// <summary>
       /// Static class constructor
       /// </summary>
       static CRC32()
       {
           InitTable();
       }

       #endregion

       #region Private Data

       /// <summary>
       /// The generator polynomial 
       /// </summary>
       private const UInt32 _poly = 0x04C11DB7;

       /// <summary>
       /// Initial value
       /// </summary>
       private static UInt32 _initialValue = 0xFFFFFFFF;

       /// <summary>
       /// The lookup table
       /// </summary>
       private static UInt32[] _table = new UInt32[256];

       #endregion Private Data

       #region Private Methods

       /// <summary>
       /// Initialise the polynomial table
       /// </summary>
       private static void InitTable()
       {
           // pre calculate the CRC table.
           UInt32 mask;
           UInt32 count;
           var crc = 0x80000000;

           for (mask = 1; mask < _table.Length; mask <<= 1)
           {
               crc = (crc << 1) ^ ((crc & 0x80000000) > 0 ? _poly : 0);
               for (count = 0; count < mask; ++count)
               {
                   _table[mask + count] = crc ^ _table[count];
               }
           }

       }

       /// <summary>
       /// helper to calculate the crc by adding a new 32 bit word 
       /// </summary>
       /// <param name="rval">Is the currently accumulated crc value</param>
       /// <param name="newWord">Is the 32 bit word to calculate the crc with</param>
       /// <returns>The new value for the calculated crc value after adding the word</returns>
       private static UInt32 addWord(UInt32 rval, UInt32 newWord)
       {
           rval ^= newWord;
           rval = (rval << 8) ^ _table[rval >> 24];
           rval = (rval << 8) ^ _table[rval >> 24];
           rval = (rval << 8) ^ _table[rval >> 24];
           rval = (rval << 8) ^ _table[rval >> 24];
           
           return rval;
       }
       
       #endregion Private Methods

       #region Public Methods

       /// <summary>
       /// Computes the checksum of the specified array of bytes
       /// </summary>
       /// <param name="bytes">the byte array containing the data across which to calculate the CRC</param>
       /// <param name="offset">An offset into the bytes array to start computing from</param>
       /// <returns>a Uint32 containing the checksum</returns>
       public static UInt32 ComputeChecksum(byte[] bytes, int offset, int len)
       {
           if ((bytes == null) || (bytes.Length == 0))
           {
               throw new ArgumentNullException("The data supplied is null or empty");
           }
           var rval = _initialValue;
           
           UInt32 newVal;
           for (var i = offset; i < len; i+= sizeof(UInt32))
           {
               try
               {
                   if (i + sizeof(UInt32) < bytes.Length)
                   {
                       newVal = BitConverter.ToUInt32(bytes, i);
                   }
                   else
                   {
                       var padded = new byte[4];
                       Array.Copy(bytes, i, padded, 0, len - i);
                       newVal = BitConverter.ToUInt32(padded, 0);
                   }
                   rval = addWord(rval, newVal);
               }
               catch(ArgumentException ex)
               {
                   Logger.LogException(Logger.BDMSwitch, ex, null);
                   break;
               }
           }

           return (rval);
       }

       /// <summary>
       /// Computes the checksum of the specified array of bytes
       /// </summary>
       /// <param name="bytes">the byte array containing the data across which to calculate the CRC</param>
       /// <returns>a Uint32 containing the checksum</returns>
       public static UInt32 ComputeChecksum(byte[] bytes)
       {
           if ((bytes == null) || (bytes.Length == 0))
           {
               throw new ArgumentNullException("The data supplied is null or empty");
           }
           return ComputeChecksum(bytes, 0, bytes.Length);
       }

       /// <summary>
       /// Calculates the checksum of the specified array of bytes and returns the CRC
       /// as an array of bytes (for streaming, etc)
       /// </summary>
       /// <param name="bytes">the byte array containing the data across which to calculate the CRC</param>
       /// <returns>a byte array containing the CRC</returns>
       public static byte[] ComputeChecksumBytes(byte[] bytes)
       {
           var crc = ComputeChecksum(bytes);
           return BitConverter.GetBytes(crc);
       }

       #endregion Public Methods
   }
}