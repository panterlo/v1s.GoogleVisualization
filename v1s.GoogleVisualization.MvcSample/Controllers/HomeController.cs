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
using System.Linq;
using System.Web;
using System.Web.Mvc;

using v1s.GoogleVisualization;

namespace v1s.GoogleVisualization.MvcSample.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [GoogleVisualizationAttribute]
        public GoogleVisualizationResult GetSomeGoogleVisualizationData(GoogleVisualizationRequest googleVisualizationRequest, int numRecords, string someParam)
        {
            // Let make the response, construct the response by passing the request
            GoogleVisualizationResponse googleVisualizationResponse = new GoogleVisualizationResponse(googleVisualizationRequest);

            // We need a Google Visualization DataTable to store the data
            GoogleVisualizationDataTable dataTable = new GoogleVisualizationDataTable();
            
            // Add a column representing date time
            GoogleVisualizationDataTableColumn column1 = dataTable.AddColumn(GoogleVisualizationDataTableColumnType.DateTime, "date");

            // Add a column representing a number
            GoogleVisualizationDataTableColumn column2 = dataTable.AddColumn(GoogleVisualizationDataTableColumnType.Number, "Serie1");


            // Now we need to add the data
            Random random = new Random();

            // Just use the now date plus 7 days for each iteration
            DateTime dateTime = DateTime.Now;

            // Fake some data records
            for (int i = 0; i < numRecords; i++)
            {
                // Add new row to the datatable
                GoogleVisualizationDataTableRow row = dataTable.AddRow();

                // Add cell by column parameter
                List<GoogleVisualizationDataTableRowCell> cells = dataTable.AddCellForColumn(row, column2,random.Next(10,100), null);

                // But you can enter the column index instead when inserting cell values
                cells[0].CellValue = dateTime;

                // Add 7 days for next iteration
                dateTime = dateTime.AddDays(7);
            }

            // Set the datatable in the response
            googleVisualizationResponse.GoogleVisualizationDataTable = dataTable;

            // Send back response
            return new GoogleVisualizationResult(googleVisualizationResponse);
        }

    }
}
