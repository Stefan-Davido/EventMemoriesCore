using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedItems
{
    public sealed class TenantProvider
    {
        public const string TenantIdHeaderName = "X-Tenant-Id";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private List<string> ignoredRoutes = new List<string>() { "auth" };

        public int GetTenantId()
        {
            if (_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments("/api/Auth"))
                return 0;

            var tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeaderName];
            
            if (string.IsNullOrEmpty(tenantIdHeader))
            {
                throw new Exception($"Missing header: {TenantIdHeaderName}");
            }
            
            if (!int.TryParse(tenantIdHeader, out int tenantId))
            {
                throw new Exception($"Invalid tenant ID: {tenantIdHeader}");
            }

            return tenantId;
        }
    }
}
