using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Nomis.Api.Common.Providers
{
    /// <summary>
    /// Провайдер для проверки, является ли тип контроллером.
    /// </summary>
    public class InternalControllerFeatureProvider :
        ControllerFeatureProvider
    {
        /// <summary>
        /// Суффикс названия контроллера.
        /// </summary>
        private const string ControllerSuffix = "Controller";

        /// <inheritdoc/>
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
            {
                return false;
            }

            return typeInfo.Name.EndsWith(ControllerSuffix, StringComparison.OrdinalIgnoreCase) ||
                   typeInfo.IsDefined(typeof(ControllerAttribute));
        }
    }
}