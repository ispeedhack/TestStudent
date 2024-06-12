using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace TestCreator.Tests.Helpers
{
    public static class JsonResultHelper
    {
        public static T GetValueFromJsonResult<T>(this JsonResult jsonResult, string propertyName)
        {
            var property =
                jsonResult.Value.GetType()
                    .GetProperties()
                    .FirstOrDefault(p => String.CompareOrdinal(p.Name, propertyName) == 0);

            if (null == property)
                throw new ArgumentException("{propertyName} not found", "propertyName");
            return (T)property.GetValue(jsonResult.Value, null);
        }

        public static IEnumerable<T> GetIEnumberableFromJsonResult<T>(this JsonResult jsonResult)
        {
            return jsonResult.Value as IEnumerable<T>;
        }

        public static T GetObjectFromJsonResult<T>(this JsonResult jsonResult) where T : class
        {
            return jsonResult.Value as T;
        }
    }
}
