using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace v1s.GoogleVisualization
{
    public class GoogleVisualizationResult : ActionResult
    {
        public GoogleVisualizationResponse GoogleVisualizationResponse { get; set; }


        public GoogleVisualizationResult(GoogleVisualizationResponse googleVisualizationResponse)
        {
            GoogleVisualizationResponse = googleVisualizationResponse;
        }


        public override void ExecuteResult(ControllerContext context)
        {
             if (context == null)
                throw new ArgumentNullException("context");

             HttpResponseBase response = context.HttpContext.Response;
             response.ContentType = "text/plain";

             response.Write(GoogleVisualizationHandler.GetGoogleVisualizationResponse(GoogleVisualizationResponse));
        }
    }
}
