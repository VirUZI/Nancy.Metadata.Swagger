using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nancy.Metadata.Swagger.Model;
using Newtonsoft.Json.Schema;
using NJsonSchema;

namespace Nancy.Metadata.Swagger.Fluent
{
    public static class SwaggerEndpointInfoExtensions
    {
        public static SwaggerEndpointInfo WithResponseModel<T>(this SwaggerEndpointInfo endpointInfo, HttpStatusCode statusCode, string description = null)
        {
            if (endpointInfo.ResponseInfos == null)
            {
                endpointInfo.ResponseInfos = new Dictionary<string, SwaggerResponseInfo>();
            }

            var statusCodeString = ((int)statusCode).ToString();
            endpointInfo.ResponseInfos[statusCodeString] = GenerateResponseInfo<T>(description);

            return endpointInfo;
        }

        public static SwaggerEndpointInfo WithDefaultResponse<T>(this SwaggerEndpointInfo endpointInfo)
        {
            return endpointInfo.WithResponseModel<T>(HttpStatusCode.OK);
        }

        public static SwaggerEndpointInfo WithResponse(this SwaggerEndpointInfo endpointInfo, HttpStatusCode statusCode, string description)
        {
            if (endpointInfo.ResponseInfos == null)
            {
                endpointInfo.ResponseInfos = new Dictionary<string, SwaggerResponseInfo>();
            }

            var statusCodeString = ((int)statusCode).ToString();
            endpointInfo.ResponseInfos[statusCodeString] = GenerateResponseInfo(description);

            return endpointInfo;
        }

        public static SwaggerEndpointInfo WithRequestParameter(this SwaggerEndpointInfo endpointInfo, string name,
            string type = "string", string format = null, bool required = true, string description = null,
            string loc = "path")
        {
            if (endpointInfo.RequestParameters == null)
            {
                endpointInfo.RequestParameters = new List<SwaggerRequestParameter>();
            }

            endpointInfo.RequestParameters.Add(new SwaggerRequestParameter
            {
                Required = required,
                Description = description,
                Format = format,
                In = loc,
                Name = name,
                Type = type
            });

            return endpointInfo;
        }

        public static SwaggerEndpointInfo WithRequestModel<T>(this SwaggerEndpointInfo endpointInfo, string name = "body", string description = null, bool required = true, string loc = "body")
        {
            if (endpointInfo.RequestParameters == null)
            {
                endpointInfo.RequestParameters = new List<SwaggerRequestParameter>();
            }

            endpointInfo.RequestParameters.Add(new SwaggerRequestParameter
            {
                Required = required,
                Description = description,
                In = loc,
                Name = name,
                Schema = GetSchema<T>()
            });

            return endpointInfo;
        }

        public static SwaggerEndpointInfo WithDescription(this SwaggerEndpointInfo endpointInfo, string description, params string[] tags)
        {
            if (endpointInfo.Tags == null)
            {
                if (tags.Length == 0)
                {
                    tags = new[] {"default"};
                }

                endpointInfo.Tags = tags;
            }

            endpointInfo.Description = description;

            return endpointInfo;
        }

        private static SwaggerResponseInfo GenerateResponseInfo<T>(string description)
        {
            return new SwaggerResponseInfo
            {
                Schema = GetSchema<T>(),
                Description = description
            };
        }

        private static SwaggerResponseInfo GenerateResponseInfo(string description)
        {
            return new SwaggerResponseInfo
            {
                Description = description
            };
        }

        private static JsonSchema4 GetSchema<T>()
        {
            return JsonSchema4.FromType(typeof(T));
        }
    }
}