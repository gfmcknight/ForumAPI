﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Utilities
{
    /// <summary>
    /// A model that can be serialized into and deserialized from a JWT
    /// header.
    /// </summary>
    public class JWTHeader
    {
        [JsonRequired]
        public string Type { get; set; }
        [JsonRequired]
        public string Algorithm { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Compares with a JWT Header to determine if they have the values.
        /// </summary>
        /// <param name="obj">The JWT header to compare to.</param>
        /// <returns>True if the two JWT Headers are the same.</returns>
        public override bool Equals(object obj)
        {
            return obj is JWTHeader &&
                ((JWTHeader)obj).Algorithm == Algorithm &&
                ((JWTHeader)obj).Type == Type;
        }
    }
}
