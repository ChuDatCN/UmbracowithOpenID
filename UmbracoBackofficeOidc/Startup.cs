﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Website.Models;
using Umbraco.Extensions;
using Umbraco9;

namespace UmbracoBackofficeOidc
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <remarks>
        /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337
        /// </remarks>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
#pragma warning disable IDE0022 // Use expression body for methods
            
            var scheme = $"{Constants.Security.BackOfficeExternalAuthenticationTypePrefix}oidc";

            services.AddUmbraco(_env, _config)
                .AddBackOffice()  //  Cung  cấp components cần tiết của BackOffice
                .AddBackOfficeExternalLogins(loginsBuilder =>                 // Cung cấp cấu hình login bên ngoài của umbraco
                    loginsBuilder.AddBackOfficeLogin(authBuilder =>                         // Đưa ra các cách thức Authen để login , vd ở dưới là OpenID
                            authBuilder.AddOpenIdConnect(scheme, "OpenID Connect", options =>      
                            {
                                options.Authority = "https://localhost:5001";  // Khi call phương thức login = OpenID  gọi địa chỉ này
                                options.ClientId = "umbraco-backoffice";        // khai báo clientID
                                options.ClientSecret = "secret";                

                                options.CallbackPath = "/signin-oidc";          // 
                                options.ResponseType = "code";
                                options.ResponseMode = "query";
                                options.UsePkce = true;
                                // get user identity
                                options.Scope.Add("email");
                                options.GetClaimsFromUserInfoEndpoint = true;
                            }),
                        providerOptions => 
                            { 
                                providerOptions.Icon = "fa fa-openid";
                            }))
                .AddWebsite()
                .AddComposers()
                .Build();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
            })
             .AddCookie("cookie");

            services.ConfigureOptions<OpenIdConnectBackOfficeExternalLoginProviderOptions>();

#pragma warning restore IDE0022 // Use expression body for methods
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseUmbraco()
                .WithMiddleware(u =>
                {
                    u.UseBackOffice();
                    u.UseWebsite();
                })
                .WithEndpoints(u =>
                {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                    u.UseWebsiteEndpoints();
                });
        }
}
}
