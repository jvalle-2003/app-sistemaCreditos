using System;
using System.Web.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class NoLoginAccessAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Verificar si el usuario está autenticado
        if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        {
            // Si el usuario está autenticado, redirigirlo a la página de inicio de la aplicación
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Index" }));
        }
        else
        {
            base.OnActionExecuting(filterContext);
        }
    }
}
