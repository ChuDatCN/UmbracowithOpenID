// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.
using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources    => // Khai báo các API  cần được bảo vệ
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("scope1"),
                new ApiScope("scope2"),
            };

        public static IEnumerable<Client> Clients => // Khai báo các kiểu tài khoản Client 
            new Client[]                               // Phân quyền truy cập resource cho các domain khác nhau tại đây.    
            {
                new Client
                {
                    ClientId = "umbraco-backoffice",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RedirectUris = { "https://localhost:44362/signin-oidc","https://localhost:44363/signin-oidc" },
                    AllowedScopes = { "openid", "profile", "email" }
                }
            };
    }
}