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
            DateTime dateTime = DateTime.Now;

            for (int i = 0; i < numRecords; i++)
            {
                GoogleVisualizationDataTableRow row = dataTable.AddRow();
                List<GoogleVisualizationDataTableRowCell> cells = dataTable.AddCellForColumn(row, column2,random.Next(10,100), null);
                cells[0].CellValue = dateTime;
                dateTime = dateTime.AddDays(7);
            }

            googleVisualizationResponse.GoogleVisualizationDataTable = dataTable;

            return new GoogleVisualizationResult(googleVisualizationResponse);
        }

    }
}
