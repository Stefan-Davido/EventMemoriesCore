using System;
using System.Collections.Generic;
using System.Text;

namespace SharedItems
{
    public static class InternalClaimTypes
    {
        private const string BaseType = "http://schemas.eventmemories.com/claims";
        public const string TenantId = BaseType + "/tenantid";
        public const string UserId = BaseType + "/userId";
        public const string EventId = BaseType + "/eventId";
    }
}
