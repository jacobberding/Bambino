using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Metadata;
using System.Web.Http.Metadata.Providers;

namespace Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.EnableCors();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "Api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(ModelMetadataProvider), new EmptyStringAllowedModelMetadataProvider());

        }

        public class EmptyStringAllowedModelMetadataProvider : DataAnnotationsModelMetadataProvider
        {
            protected override CachedDataAnnotationsModelMetadata CreateMetadataFromPrototype(CachedDataAnnotationsModelMetadata prototype, Func<object> modelAccessor)
            {
                var metadata = base.CreateMetadataFromPrototype(prototype, modelAccessor);
                metadata.ConvertEmptyStringToNull = false;
                return metadata;
            }

            protected override CachedDataAnnotationsModelMetadata CreateMetadataPrototype(IEnumerable<Attribute> attributes, Type containerType, Type modelType, string propertyName)
            {
                var metadata = base.CreateMetadataPrototype(attributes, containerType, modelType, propertyName);
                metadata.ConvertEmptyStringToNull = false;
                return metadata;
            }
        }

    }
}
