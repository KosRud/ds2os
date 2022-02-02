﻿/*
 * Dark Souls 2 - Open Server
 * Copyright (C) 2021 Tim Leonard
 *
 * This program is free software; licensed under the MIT license. 
 * You should have received a copy of the license along with this program. 
 * If not, see <https://opensource.org/licenses/MIT>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loader
{
    public static class EncryptionUtils
    {
        const int TEA_BLOCK_SIZE = 8;

        // All this is super inefficient, should be rewritten when time allows.

        public static byte[] Tea32Encrypt(byte[] data, uint[] key)
        {
            int length_rounded_to_block_size = ((data.Length + (TEA_BLOCK_SIZE - 1)) / TEA_BLOCK_SIZE) * TEA_BLOCK_SIZE;

            byte[] output = new byte[length_rounded_to_block_size];

            for (int block_offset = 0; block_offset < data.Length; block_offset += TEA_BLOCK_SIZE)
            {
                Tea32EncryptBlock(data, block_offset, output, block_offset, key);
            }

            return output;
        }

        public static byte[] Tea32Decrypt(byte[] data, uint[] key)
        {
            byte[] output = new byte[data.Length];

            for (int block_offset = 0; block_offset < data.Length; block_offset += TEA_BLOCK_SIZE)
            {
                Tea32DecryptBlock(data, block_offset, output, block_offset, key);
            }

            return output;
        }

        public static void Tea32EncryptBlock(byte[] input, int input_offset, byte[] output, int output_offset, uint[] key)
        {
            uint[] v = {
                input_offset + 4 > input.Length ? 0 : BitConverter.ToUInt32(input, input_offset),
                input_offset + 8 > input.Length ? 0 : BitConverter.ToUInt32(input, input_offset + 4)
            };

            uint v0 = v[0], v1 = v[1], sum = 0, i;
            uint delta = 0x9E3779B9;
            uint k0 = key[0], k1 = key[1], k2 = key[2], k3 = key[3];
            for (i = 0; i < 32; i++)
            {
                sum += delta;
                v0 += ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                v1 += ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
            }
            v[0] = v0; v[1] = v1;

            ToBytes(v0, output, output_offset);
            ToBytes(v1, output, output_offset + 4);
        }

        public static void Tea32DecryptBlock(byte[] input, int input_offset, byte[] output, int output_offset, uint[] key)
        {
            uint[] v = { 
                BitConverter.ToUInt32(input, input_offset), 
                BitConverter.ToUInt32(input, input_offset + 4) 
            };

            uint v0 = v[0], v1 = v[1], sum = 0xC6EF3720, i;
            uint delta = 0x9E3779B9;
            uint k0 = key[0], k1 = key[1], k2 = key[2], k3 = key[3];  
            for (i = 0; i < 32; i++)
            {
                v1 -= ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                v0 -= ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                sum -= delta;
            }

            ToBytes(v0, output, output_offset);
            ToBytes(v1, output, output_offset + 4);
        }

        // Faster inplace version of BitCoverter.ToBytes
        static unsafe void ToBytes(uint value, byte[] array, int offset)
        {
            fixed (byte* ptr = &array[offset])
                *(uint*)ptr = value;
        }

        // Just for testing remove.
        public static byte[] RetailServerInfoBlob = new byte[] {
               0x40,
               0x77,
               0x0c,
               0x21,
               0x6d,
               0xf0,
               0xe3,
               0xf0,
               0xd1,
               0xd5,
               0x61,
               0x8a,
               0xe2,
               0x38,
               0x6d,
               0x0f,
               0x51,
               0x42,
               0x76,
               0xc3,
               0xf6,
               0x85,
               0xea,
               0x95,
               0x24,
               0xac,
               0x12,
               0x85,
               0x76,
               0xdc,
               0xd7,
               0x8c,
               0x51,
               0x79,
               0x33,
               0x97,
               0xdc,
               0xa4,
               0x98,
               0xdb,
               0xea,
               0x90,
               0x2f,
               0xd3,
               0xe7,
               0x49,
               0x2b,
               0xd1,
               0x9d,
               0x42,
               0x9d,
               0x94,
               0x1b,
               0x42,
               0xee,
               0x97,
               0x0a,
               0x9f,
               0x0e,
               0xfc,
               0xbe,
               0xbf,
               0x1a,
               0x2c,
               0xc9,
               0x6e,
               0x23,
               0x3d,
               0xda,
               0x77,
               0x0e,
               0xfd,
               0x7b,
               0x59,
               0xf4,
               0x5e,
               0x7c,
               0x3d,
               0x9c,
               0x31,
               0x69,
               0xf3,
               0x15,
               0xdd,
               0xeb,
               0x39,
               0x59,
               0xdc,
               0x1e,
               0x8f,
               0x6e,
               0x81,
               0x90,
               0x2a,
               0x15,
               0xce,
               0x9e,
               0xe0,
               0x79,
               0x5b,
               0x36,
               0x66,
               0x45,
               0xe6,
               0xf0,
               0xbc,
               0x2a,
               0x96,
               0xaa,
               0x57,
               0xef,
               0xbc,
               0xee,
               0xcf,
               0x7b,
               0xbf,
               0x5d,
               0xe3,
               0xa3,
               0xfc,
               0x42,
               0xa2,
               0x1c,
               0x8d,
               0xcb,
               0x15,
               0x62,
               0x73,
               0xcd,
               0xff,
               0xb5,
               0x5c,
               0x84,
               0x2c,
               0xf0,
               0xee,
               0x43,
               0x0d,
               0xe4,
               0x43,
               0xc0,
               0x77,
               0x48,
               0x65,
               0x12,
               0xf6,
               0x11,
               0x49,
               0x9f,
               0xf9,
               0x55,
               0x9e,
               0x3f,
               0x85,
               0x25,
               0x0c,
               0x60,
               0x3e,
               0x1f,
               0x85,
               0x9a,
               0x84,
               0xdb,
               0xfb,
               0x56,
               0x1a,
               0x6e,
               0xe5,
               0xc6,
               0x25,
               0xa0,
               0x49,
               0x5c,
               0xef,
               0x94,
               0xad,
               0xaf,
               0x73,
               0xb0,
               0xfb,
               0x55,
               0x40,
               0xea,
               0x6d,
               0xce,
               0x94,
               0x44,
               0x21,
               0xd2,
               0xc9,
               0x20,
               0x49,
               0xc5,
               0xcd,
               0x9b,
               0xf6,
               0x61,
               0x7e,
               0xd7,
               0xf7,
               0xc1,
               0x90,
               0x54,
               0x37,
               0x1a,
               0xb6,
               0xbc,
               0x3d,
               0xa0,
               0xa6,
               0xd5,
               0xe0,
               0x24,
               0xff,
               0x40,
               0x87,
               0xe7,
               0x3a,
               0x53,
               0xf6,
               0x2f,
               0xcf,
               0x80,
               0x32,
               0x18,
               0x09,
               0xae,
               0x8e,
               0x6c,
               0x6e,
               0x8a,
               0x75,
               0xb9,
               0x9c,
               0x9d,
               0xea,
               0x91,
               0x0e,
               0x74,
               0x54,
               0x61,
               0x6b,
               0x25,
               0xa8,
               0xb9,
               0xbb,
               0xc4,
               0x17,
               0x7b,
               0x82,
               0xea,
               0x27,
               0xf8,
               0x34,
               0x11,
               0x45,
               0x27,
               0x13,
               0xb7,
               0x13,
               0x66,
               0x54,
               0xd7,
               0xcb,
               0x90,
               0x3c,
               0x26,
               0x7c,
               0x99,
               0x47,
               0x09,
               0x1a,
               0xc1,
               0x6f,
               0x10,
               0x6e,
               0xc7,
               0xd4,
               0xb4,
               0x3e,
               0xb2,
               0x66,
               0xd3,
               0xde,
               0x61,
               0x81,
               0x3f,
               0x35,
               0x66,
               0x10,
               0x1b,
               0x4b,
               0xf6,
               0xfa,
               0xe8,
               0xf6,
               0x69,
               0xd0,
               0x48,
               0x65,
               0x06,
               0x70,
               0x53,
               0xec,
               0x7d,
               0xe3,
               0x33,
               0x3f,
               0x75,
               0xa9,
               0x15,
               0x49,
               0xf7,
               0x4f,
               0x4b,
               0xf2,
               0x27,
               0xa6,
               0xd8,
               0x51,
               0xb2,
               0xd0,
               0x49,
               0x84,
               0x20,
               0x8d,
               0xf8,
               0xd2,
               0xf3,
               0x83,
               0x0e,
               0x9a,
               0x73,
               0x20,
               0x2a,
               0x31,
               0xf8,
               0x0a,
               0x2f,
               0xdb,
               0x99,
               0x4b,
               0x4d,
               0x14,
               0xb0,
               0xb7,
               0xcf,
               0xa3,
               0x6a,
               0x45,
               0x10,
               0xc9,
               0xee,
               0xc9,
               0xc5,
               0x09,
               0x81,
               0x37,
               0x5e,
               0x23,
               0xa8,
               0xd0,
               0xd3,
               0x78,
               0xe2,
               0x71,
               0x00,
               0xab,
               0xa7,
               0x47,
               0x3c,
               0x35,
               0x75,
               0xcb,
               0xd4,
               0x16,
               0x2c,
               0x19,
               0x90,
               0xd4,
               0x09,
               0x63,
               0x68,
               0xe3,
               0x70,
               0xb4,
               0xb1,
               0xa6,
               0x0c,
               0x49,
               0x52,
               0xeb,
               0x47,
               0x22,
               0xfd,
               0xd2,
               0x5b,
               0x2a,
               0x3f,
               0xd0,
               0x02,
               0x7d,
               0x0e,
               0xb5,
               0xf4,
               0x5b,
               0x46,
               0x92,
               0x7f,
               0x3d,
               0x18,
               0xdf,
               0xbb,
               0x02,
               0x42,
               0xc9,
               0xa9,
               0x82,
               0xb2,
               0x67,
               0x86,
               0x07,
               0x92,
               0x0d,
               0x3c,
               0x9c,
               0xb7,
               0x27,
               0x63,
               0x71,
               0xd5,
               0xcb,
               0xc8,
               0x95,
               0x26,
               0x23,
               0x33,
               0x74,
               0x31,
               0x78,
               0xb3,
               0x65,
               0x92,
               0x9c,
               0x36,
               0x9f,
               0x68,
               0xdb,
               0xb5,
               0xdd,
               0x65,
               0xa8,
               0x8b,
               0x2f,
               0x45,
               0x39,
               0x6f,
               0x4d,
               0x98,
               0x9e,
               0x1e,
               0x96,
               0xc0,
               0x08,
               0xf8,
               0xc6,
               0x49,
               0x32,
               0x6a,
               0xfc,
               0xfd,
               0x04,
               0xe4,
               0x04,
               0x9e,
               0x3f,
               0xf7,
               0xab,
               0xed,
               0xf1,
               0x38,
               0xd0,
               0x53,
               0x5f,
               0x60,
               0x68,
               0x81,
               0x94,
               0xc3,
               0xfd,
               0x23,
               0x70,
               0x48,
               0x86,
               0x0f,
               0xa4,
               0xcd,
               0xb4,
               0xf2,
               0xa2,
               0x8b,
               0x4b,
               0x94,
               0xde,
               0xc2,
               0x3f,
               0xd0,
               0x48,
               0xae,
               0xad,
               0x92,
               0x25,
               0xac,
               0x3f,
               0x82,
               0xe4,
                0x59,
                0xea,
                0xb7,
                0xf3,
                0xef,
                0x11,
                0x9b,
                0x20,
                0xb9,
                0xee,
                0xbe,
                0x2a,
                0x17,
                0xd1,
                0x6b,
                0x0b,
                0x89,
                0xaf,
                0xf0,
                0x9f,
                0xc9,
                0xe8,
                0xe8,
                0x4f,
                0x81,
                0x44,
                0x07,
                0xc7,
                0xe9,
                0xfc,
                0x6d,
                0x6e,
                0x71,
                0xde,
                0x27,
                0xb4,
                0x7a,
                0x11,
                0xfd,
                0x6c,
                0x4c,
                0x61,
                0x73,
                0x00,
                0xb2,
                0xf5,
                0x73,
                0xe8,
                0x3c,
                0xe0,
                0xbf,
                0x43,
                0xb8,
                0x4e,
                0xf9,
                0xe4,
                0xa3,
                0x48,
                0x57,
                0x1c,
                0xa6,
                0x29,
                0xca,
                0x12,
                0xa6,
                0xb7,
                0x0b,
                0xdb,
                0x0a,
                0x52,
                0x60,
                0xd4,
                0x71,
                0xc8,
                0xd8,
                0xed,
                0x94,
                0x6b,
                0xea,
                0x33,
                0xdf,
                0x9d,
                0x0c,
                0x35,
                0x86,
                0xec,
                0xdb,
                0x91,
                0x5c,
                0x50,
                0x19,
                0x94,
                0x6f,
                0x33,
                0x88,
                0xb9,
                0x20,
                0x1c,
                0x4a,
                0x82,
                0x46,
                0xe0,
                0x8d,
                0x82,
                0x05,
                0xdd,
                0xc0,
                0x12,
                0x80,
                0x46,
                0x41,
                0x95,
                0x23,
                0xd0,
                0xa1,
                0x42,
                0x6c,
                0xc2,
                0x8e,
                0xf2,
                0xc6,
                0xc9,
                0x7e,
                0x31,
                0x02,
                0x4f,
                0x44,
                0xa4,
                0x59,
                0xf0,
                0x73,
                0xaa,
                0xc9,
                0x96,
                0x5f,
                0x67,
                0xf5,
                0x2e,
                0xff,
                0xab,
                0xb9,
                0xbe,
                0x38,
                0x1e,
                0xda,
                0x4e,
                0x85,
                0x6f,
                0x87,
                0x3d,
                0x71,
                0xa5,
                0x64,
                0x04,
                0xec,
                0x8c,
                0xb2,
                0xa0,
                0xe9,
                0x3b,
                0x59,
                0x78,
                0x90,
                0x24,
                0x7e,
                0xd1,
                0x1c,
                0x5e,
                0xac,
                0x9c,
                0xbe,
                0xf4,
                0x9b,
                0xa2,
                0xec,
                0xec,
                0xc5,
                0xc8,
                0x47,
                0x0d,
                0x9e,
                0xa1,
                0xd0,
                0x36,
                0xb0,
                0xd2,
                0xe0,
                0xd3,
                0xae,
                0x67,
                0xb0,
                0xd2,
                0x90,
                0xc6,
                0x43,
                0xab,
                0x0b,
                0xa5,
                0xdb,
                0x96,
                0x3a,
                0xa2,
                0xeb,
                0x1e,
                0x5c,
                0xe5,
                0x54,
                0x28,
                0xc5,
                0x65,
                0x06,
                0x65,
                0xae,
                0x3b,
                0x4a,
                0xe3,
                0xc5,
                0xa3,
                0xf3,
                0x01,
                0x70,
                0x82,
                0x8e,
                0xda,
                0x91,
                0x27,
                0xf4,
                0xda,
                0x95,
                0x35,
                0x28,
                0xc9,
                0x80,
                0x23,
                0x4a,
                0x77,
                0x8e,
                0x29,
                0xb4,
                0x58,
                0xc2,
                0xbc,
                0x87,
                0xde,
                0x0b,
                0xa4,
                0xf6,
                0xaa,
                0x29,
                0xb2,
                0x3c,
                0x73,
                0x46,
                0x03,
                0xf1,
                0xa8,
                0x8b,
                0x4f,
                0x9b,
                0x94,
                0xdb,
                0x2a,
                0x81,
                0xfd,
                0x0a,
                0x1b,
                0x37,
                0x5c,
                0x1e,
                0x24,
                0xaf,
                0xc6,
                0x37,
                0x18,
                0x3d,
                0x5b,
                0xec,
                0x25,
                0x08,
                0xf7,
                0xa1,
                0x40,
                0x74,
                0x36,
                0x9f,
                0xbf,
                0xda,
                0xc5,
                0xe5,
                0xc0,
                0x05,
                0x20,
                0x3f,
                0x72,
                0xc2,
                0xbc,
                0x23,
                0xa6,
                0x83,
                0xe5,
                0xf2,
                0x6e,
                0x52,
                0x8f,
                0x93,
                0xbb,
                0xf3,
                0x17,
                0x40,
                0x1b,
                0x3c,
                0xb1,
                0x52,
                0x38,
                0x41,
                0x8c,
                0x75,
                0x2a,
                0x08,
                0xd5,
                0xb0,
                0xfc,
                0xf7,
                0x47,
                0x55,
                0x01,
                0xe4,
                0x4d,
                0x7b,
                0x6e,
                0x61,
                0x50,
                0x38,
                0xc7,
                0x94,
                0x85,
                0x97,
                0x7f,
                0x83,
                0xf7,
                0xbc,
                0xe1,
                0x87,
                0x19,
                0x1f,
                0x62,
                0x3d,
                0xdb,
                0x1c,
                0x6c,
                0x55,
                0x09,
                0xa7,
                0xbf,
                0x1c,
                0xf3,
                0x67,
                0xac,
                0x5e,
                0x69,
                0x3a,
                0x56,
                0xe3,
                0x41,
                0xb8,
                0x98,
                0x67,
                0xdb,
                0xa2,
                0xe6,
                0x1a,
                0xc4,
                0x4e,
                0xa4,
                0xd6,
                0xb1,
                0xc4,
                0x05,
                0xfa,
                0x00,
                0x67,
                0x80,
                0x31,
                0xe8,
                0xd9,
                0x29,
                0xcb,
                0x84,
                0x8a,
                0xa8,
                0x61,
                0x7e,
                0xc2,
                0xa5,
                0x84,
                0x9c,
                0xa9,
                0x88,
                0x46,
                0x26,
                0xbb,
                0xbb,
                0x14,
                0x51,
                0x84,
                0x25,
                0x6b,
                0x95,
                0xc5,
                0xe7,
                0xd3,
                0x00,
                0x5f,
                0xe7,
                0x00,
                0x56,
                0x6d,
                0xea,
                0xf5,
                0xa4,
                0xf6,
                0xfe,
                0xde,
                0xd5,
                0x8c,
                0x3d,
                0x0b,
                0x1d,
                0xff,
                0x4c,
                0xe4,
                0xda,
                0xcb,
                0xb0,
                0x2d,
                0x81,
                0x90,
                0x36,
                0x5c,
                0x23,
                0x27,
                0x1c,
                0x32,
                0x4a,
                0x3a,
                0x2f,
                0xba,
                0x4f,
                0x5f,
                0x96,
                0xf1,
                0x9d,
                0x20,
                0x04,
                0xcc,
                0x48,
                0x05,
                0x56,
                0x24,
                0x34,
                0x3f,
                0x3b,
                0x62,
                0xd6,
                0xec,
                0x0d,
                0x7a,
                0xe8,
                0xf5,
                0x41,
                0xf5,
                0xfc,
                0x99,
                0x53,
                0xf8,
                0xbe,
                0x8d,
                0xff,
                0x81,
                0xb5,
                0xbf,
                0x77,
                0x03,
                0x04,
                0x84,
                0x63,
                0x92,
                0x63,
                0x1b,
                0x38,
                0x6d,
                0x7f,
                0xd0,
                0xc0,
                0x1f,
                0xc6,
                0xa6,
                0x42,
                0x73,
                0x32,
                0x4f,
                0x24,
                0x3c,
                0xd3,
                0x8e,
                0x65,
                0x85,
                0x9e,
                0xbf,
                0x8d,
                0x01,
                0x30,
                0x00,
                0x7a,
                0xb1,
                0x4c,
                0x1c,
                0xf3,
                0x4c,
                0x83,
                0x8b,
                0x7d,
                0x35,
                0xca,
                0xca,
                0xb4,
                0xd7,
                0x0b,
                0xe2,
                0x4f,
                0xd9,
                0x2b,
                0xe7,
                0x18,
                0xad,
                0xe6,
                0x80,
                0x8c,
                0x9e,
                0xa3,
                0x17,
                0xfe,
                0xf2,
                0x27,
                0x70,
                0xfe,
                0x3c,
                0x0b,
               /* 0xa8,
                0x2b,
                0x27,
                0xca,*/
        };
    }
}
