using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Astove.BlurAdmin.WebApi.Core;
using System.Web.Http.Tracing;

namespace Astove.BlurAdmin.WebApi.Config
{
    public static class TraceConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            var traceWriter = new NLogTraceWriter();
            configuration.Services.Replace(typeof(ITraceWriter), traceWriter);
        }
    }
}
