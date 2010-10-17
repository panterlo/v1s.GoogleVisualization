/*  Copyright 2010 Panterlo AB, Jens Nylander, jens@panterlo.com
 *  http://www.panterlo.com | +46 70 421 20 50
 *   
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.

 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.

 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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