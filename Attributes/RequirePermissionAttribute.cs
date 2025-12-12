using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectM.Services;
using System.Security.Claims;

namespace ProjectM.Attributes
{
    public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public RequirePermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Skip authorization if AllowAnonymous attribute is present
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
            {
                return;
            }

            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
            if (permissionService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Check if user has any of the required permissions
            var hasPermission = false;
            foreach (var permission in _permissions)
            {
                // Check JWT claims first for performance
                if (user.HasClaim("permission", permission))
                {
                    hasPermission = true;
                    break;
                }
            }

            // If not found in claims, check database (in case permissions changed)
            if (!hasPermission)
            {
                var task = Task.Run(async () => await CheckPermissionsAsync(permissionService, userId));
                task.Wait();
                hasPermission = task.Result;
            }

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }

        private async Task<bool> CheckPermissionsAsync(IPermissionService permissionService, Guid userId)
        {
            foreach (var permission in _permissions)
            {
                if (await permissionService.HasPermissionAsync(userId, permission))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
