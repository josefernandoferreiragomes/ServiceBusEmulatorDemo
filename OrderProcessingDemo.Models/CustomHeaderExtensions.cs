using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingDemo.Models
{
    public static class CustomHeaderExtensions
    {
        public static IFunctionsHostBuilder AddCustomHeaderHandler(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IHttpHeaderHandler, CustomHeaderHandler>();
            return builder;
        }
    }
}
