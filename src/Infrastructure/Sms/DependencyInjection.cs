using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Sms;
using Infrastructure.SmsSms;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Sms
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSmsServices(this IServiceCollection services)
        {
            services.AddSingleton<ISmsSender, FileSmsSender>();
            return services;
        }
    }
}