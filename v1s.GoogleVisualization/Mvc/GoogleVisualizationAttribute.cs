using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace v1s.GoogleVisualization
{
    public class GoogleVisualizationAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            NameValueCollection queryStringNameValueCollection = filterContext.HttpContext.Request.QueryString;

            GoogleVisualizationRequest googleVisualizationRequest = new GoogleVisualizationRequest(queryStringNameValueCollection);

            filterContext.ActionParameters["googleVisualizationRequest"] = googleVisualizationRequest;

            base.OnActionExecuting(filterContext);
        }
    }
}