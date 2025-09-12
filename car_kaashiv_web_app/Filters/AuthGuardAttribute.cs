using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;


namespace car_kaashiv_web_app.Filters
{
    public class AuthGuardAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
           var httpContext = context.HttpContext;
           var actionName = context.ActionDescriptor.RouteValues["action"];
           var controllerName = context.ActionDescriptor.RouteValues["controller"];
            //pages without need Authorization
            var skipActions = new[]
            {
               "Login",
               "Register",
               "Privacy",
               "/"
           };
            if (skipActions.Contains(actionName))
            {
                base.OnActionExecuting(context);
                return;
            }
           
            var UserPhone = httpContext.Session.GetString("UserPhone");
            // if session not found, redirect to Login
            if (string.IsNullOrEmpty(UserPhone))
            {
                context.Result = new RedirectToActionResult("Login", "User", null);

            }
            base.OnActionExecuting(context);
        }

    }
}
