﻿// Copyright (c) Microsoft. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NuGetCredentialProvider.Util
{
    public class SessionTokenCache : ICache<Uri, string>
    {
        private static readonly object FileLock = new object();
        private readonly string cacheFilePath;

        public SessionTokenCache(string cacheFilePath)
        {
            this.cacheFilePath = cacheFilePath;
        }

        private Dictionary<Uri, string> Cache
        {
            get
            {
                lock (FileLock)
                {
                    return Deserialize(ReadFileBytes());
                }
            }
        }

        public string this[Uri key]
        {
            get => Cache[key];
            set
            {
                lock (FileLock)
                {
                    var cache = Cache;
                    cache[key] = value;
                    WriteFileBytes(Serialize(cache));
                }
            }
        }

        public bool ContainsKey(Uri key)
        {
            return Cache.ContainsKey(key);
        }

        public bool TryGetValue(Uri key, out string value)
        {
            return Cache.TryGetValue(key, out value);
        }

        public void Remove(Uri key)
        {
            lock (FileLock)
            {
                var cache = Cache;
                cache.Remove(key);
                WriteFileBytes(Serialize(cache));
            }
        }

        private Dictionary<Uri, string> Deserialize(byte[] data)
        {
            if (data == null)
            {
                return new Dictionary<Uri, string>();
            }

            var serialized = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<Dictionary<Uri, string>>(serialized);
        }

        private byte[] Serialize(Dictionary<Uri, string> data)
        {
            var serialized = JsonConvert.SerializeObject(data);
            return System.Text.Encoding.UTF8.GetBytes(serialized);
        }

        private byte[] ReadFileBytes()
        {
            return EncryptedFile.ReadFileBytes(cacheFilePath, readUnencrypted: true);
        }

        private void WriteFileBytes(byte[] bytes)
        {
            EncryptedFile.WriteFileBytes(cacheFilePath, bytes, writeUnencrypted: true);
        }
    }
}
