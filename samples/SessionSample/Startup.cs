﻿using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;

namespace SessionSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCachingServices();
            services.AddSessionServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSession();
            // app.UseInMemorySession();
            // app.UseDistributedSession(new RedisCache(new RedisCacheOptions() { Configuration = "localhost" }));

            app.Map("/session", subApp =>
            {
                subApp.Run(async context =>
                {
                    int visits = 0;
                    visits = context.Session.GetInt("visits") ?? 0;
                    context.Session.SetInt("visits", ++visits);
                    await context.Response.WriteAsync("Counting: You have visited our page this many times: " + visits);
                });
            });

            app.Run(async context =>
            {
                int visits = 0;
                visits = context.Session.GetInt("visits") ?? 0;
                await context.Response.WriteAsync("<html><body>");
                if (visits == 0)
                {
                    await context.Response.WriteAsync("Your session has not been established.<br>");
                    await context.Response.WriteAsync(DateTime.Now + "<br>");
                    await context.Response.WriteAsync("<a href=\"/session\">Establish session</a>.<br>");
                }
                else
                {
                    context.Session.SetInt("visits", ++visits);
                    await context.Response.WriteAsync("Your session has was located, you've visited the site this many times: " + visits);
                }
                await context.Response.WriteAsync("</body></html>");
            });
        }
    }
}
