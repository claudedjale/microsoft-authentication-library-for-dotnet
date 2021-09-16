﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Identity.Client.Internal;

namespace Microsoft.Identity.Client.Instance
{
    internal class AadAuthority : Authority
    {
        public const string DefaultTrustedHost = "login.microsoftonline.com";
        public const string AADCanonicalAuthorityTemplate = "https://{0}/{1}/";

        private const string TokenEndpointTemplate = "{0}oauth2/v2.0/token";
        private const string DeviceCodeEndpointTemplate = "{0}oauth2/v2.0/devicecode";
        private const string AuthorizationEndpointTemplate = "{0}oauth2/v2.0/authorize";

        private static readonly ISet<string> s_tenantlessTenantNames = new HashSet<string>(
          new[]
          {
                Constants.CommonTenant,
                Constants.OrganizationsTenant,
                Constants.ConsumerTenant
          },
          StringComparer.OrdinalIgnoreCase);

        internal AadAuthority(AuthorityInfo authorityInfo) : base(authorityInfo)
        {
            TenantId = AuthorityInfo.GetFirstPathSegment(AuthorityInfo.CanonicalAuthority);
        }

        internal override string TenantId { get; }

        internal bool IsWorkAndSchoolOnly()
        {
            return !TenantId.Equals(Constants.CommonTenant) &&
                  !TenantId.Equals(Constants.ConsumerTenant) &&
                  !TenantId.Equals(Constants.MsaTenantId);
        }

        internal bool IsConsumers()
        {
            return TenantId.Equals(Constants.ConsumerTenant) ||
                  TenantId.Equals(Constants.MsaTenantId);
        }

        internal bool IsCommonOrganizationsOrConsumersTenant()
        {
            return IsCommonOrganizationsOrConsumersTenant(TenantId);
        }

        internal static bool IsCommonOrganizationsOrConsumersTenant(string tenantId)
        {
            return !string.IsNullOrEmpty(tenantId) &&
                s_tenantlessTenantNames.Contains(tenantId);
        }

        internal bool IsCommonOrOrganizationsTenant()
        {
            return IsCommonOrOrganizationsTenant(TenantId);
        }

        internal static bool IsCommonOrOrganizationsTenant(string tenantId)
        {
            return !string.IsNullOrEmpty(tenantId) && 
                tenantId != Constants.ConsumerTenant &&
                s_tenantlessTenantNames.Contains(tenantId);
        }

        internal override string GetTenantedAuthority(string tenantId, bool forceSpecifiedTenant = false)
        {
            if (!string.IsNullOrEmpty(tenantId) &&
                (forceSpecifiedTenant || IsCommonOrganizationsOrConsumersTenant()))
            {
                var authorityUri = new Uri(AuthorityInfo.CanonicalAuthority);

                return string.Format(
                    CultureInfo.InvariantCulture,
                    AADCanonicalAuthorityTemplate,
                    authorityUri.Authority,
                    tenantId);
            }

            return AuthorityInfo.CanonicalAuthority;
        }

        internal override string GetTokenEndpoint()
        {
            string tokenEndpoint = string.Format(
                    CultureInfo.InvariantCulture,
                    TokenEndpointTemplate,
                    AuthorityInfo.CanonicalAuthority);

            return tokenEndpoint;
        }

        internal override string GetAuthorizationEndpoint()
        {
            string authorizationEndpoint = string.Format(CultureInfo.InvariantCulture,
                  AuthorizationEndpointTemplate,
                  AuthorityInfo.CanonicalAuthority);

            return authorizationEndpoint;
        }

        internal override string GetDeviceCodeEndpoint()
        {

            string deviceEndpoint = string.Format(
                CultureInfo.InvariantCulture,
                DeviceCodeEndpointTemplate,
                AuthorityInfo.CanonicalAuthority);

            return deviceEndpoint;
        }
    }
}
