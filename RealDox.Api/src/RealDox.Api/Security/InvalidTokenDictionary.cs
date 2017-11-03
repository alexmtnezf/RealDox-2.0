using System;
using System.Collections.Generic;

namespace RealDox.Api.Security
{
    /// <summary>
    /// Stores token IDs that have been revoked.
    /// </summary>
    public class InvalidTokenDictionary : Dictionary<string, DateTime>
    {
    }
}