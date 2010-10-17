using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace v1s.GoogleVisualization
{
    public class GoogleVisualizationDataTable
    {
        public IList<GoogleVisualizationDataTableRow> Rows { get; set; }
        public IList<GoogleVisualizationDataTableColumn> Columns { get; set; }

        public GoogleVisualizationDataTableColumn AddColumn(GoogleVisualizationDataTableColumnType columnType, string label)
        {
            if (Columns == null)
                Columns = new List<GoogleVisualizationDataTableColumn>();

            GoogleVisualizationDataTableColumn googleVisualizationDataTableColumn = new GoogleVisualizationDataTableColumn();
            googleVisualizationDataTableColumn.ColumnType = columnType;
            googleVisualizationDataTableColumn.Label = label;

            Columns.Add(googleVisualizationDataTableColumn);

            return googleVisualizationDataTableColumn;
        }

        public GoogleVisualizationDataTableRow AddRow(object cellValue, string cellValueAsString)
        {
            if (Rows == null)
                Rows = new List<GoogleVisualizationDataTableRow>();

            GoogleVisualizationDataTableRow googleVisualizationDataTableRow = new GoogleVisualizationDataTableRow();
            googleVisualizationDataTableRow.Cells = new List<GoogleVisualizationDataTableRowCell>();

            GoogleVisualizationDataTableRowCell googleVisualizationDataTableRowCell = new GoogleVisualizationDataTableRowCell();
            googleVisualizationDataTableRowCell.CellValue = cellValue;
            googleVisualizationDataTableRowCell.CellValueAsString = cellValueAsString;
            googleVisualizationDataTableRow.Cells.Add(googleVisualizationDataTableRowCell);

            Rows.Add(googleVisualizationDataTableRow);

            return googleVisualizationDataTableRow;
        }

        public GoogleVisualizationDataTableRow AddRow()
        {
            if (Rows == null)
                Rows = new List<GoogleVisualizationDataTableRow>();

            GoogleVisualizationDataTableRow googleVisualizationDataTableRow = new GoogleVisualizationDataTableRow();

            Rows.Add(googleVisualizationDataTableRow);

            return googleVisualizationDataTableRow;
        }

        public GoogleVisualizationDataTableRowCell AddCell(GoogleVisualizationDataTableRow googleVisualizationDataTableRow, object cellValue, string cellValueAsString)
        {
            if (googleVisualizationDataTableRow == null)
                throw new Exception("This method requires googleVisualizationDataTableRow to be instantiated before calling");

            if (googleVisualizationDataTableRow.Cells == null)
               googleVisualizationDataTableRow.Cells = new List<GoogleVisualizationDataTableRowCell>();

            GoogleVisualizationDataTableRowCell googleVisualizationDataTableRowCell = new GoogleVisualizationDataTableRowCell();
            googleVisualizationDataTableRowCell.CellValue = cellValue;
            googleVisualizationDataTableRowCell.CellValueAsString = cellValueAsString;
            googleVisualizationDataTableRow.Cells.Add(googleVisualizationDataTableRowCell);


            return googleVisualizationDataTableRowCell;
        }

        /// <summary>
        /// Adds a cell with the supplied property values to the selected column and returns back all cells for the row, other cells are filled with null data
        /// </summary>
        /// <param name="googleVisualizationDataTableRow">The row to which the cell will be added</param>
        /// <param name="googleVisualizationDataTableColumn">The cell will be injected into this column and the other cells will be filled with null data</param>
        /// <param name="cellValue">The value for the cell</param>
        /// <param name="cellValueAsString">A string representing the cell value</param>
        /// <returns></returns>
        public List<GoogleVisualizationDataTableRowCell> AddCellForColumn(GoogleVisualizationDataTableRow googleVisualizationDataTableRow, GoogleVisualizationDataTableColumn googleVisualizationDataTableColumn, object cellValue, string cellValueAsString)
        {
            if (googleVisualizationDataTableRow == null)
                throw new Exception("This method requires googleVisualizationDataTableRow to be instantiated before calling");

            int indexOfColumn = Columns.IndexOf(googleVisualizationDataTableColumn);
           
            if (googleVisualizationDataTableRow.Cells == null)
                googleVisualizationDataTableRow.Cells = new List<GoogleVisualizationDataTableRowCell>();

            List<GoogleVisualizationDataTableRowCell> cells = new List<GoogleVisualizationDataTableRowCell>();

            for (int i = 0; i < Columns.Count(); i++)
            {
                GoogleVisualizationDataTableRowCell googleVisualizationDataTableRowCell = new GoogleVisualizationDataTableRowCell();

                if (i == indexOfColumn)
                {
                    googleVisualizationDataTableRowCell.CellValue = cellValue;
                    googleVisualizationDataTableRowCell.CellValueAsString = cellValueAsString;
                }
                else
                {
                    googleVisualizationDataTableRowCell.CellValue = null;
                    googleVisualizationDataTableRowCell.CellValueAsString = null;
                }
                googleVisualizationDataTableRow.Cells.Add(googleVisualizationDataTableRowCell);
                cells.Add(googleVisualizationDataTableRowCell);
            }

            return cells;
        }

        /// <summary>
        /// Optional. The p property is a map of custom values applied to the whole DataTable. These values can be of any JavaScript type.
        /// If your visualization supports any datatable-level properties, it will describe them; otherwise, this property 
        /// will be ignored. Example: p:{className: 'myDataTable'}.
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; set; }

 
    }

    public class GoogleVisualizationDataTableRow
    {
        public GoogleVisualizationDataTableRowCell AddCell(object cellValue, string cellValueAsString)
        {
            if (Cells == null)
                Cells = new List<GoogleVisualizationDataTableRowCell>();

            GoogleVisualizationDataTableRowCell googleVisualizationDataTableRowCell = new GoogleVisualizationDataTableRowCell();
            googleVisualizationDataTableRowCell.CellValue = cellValue;
            googleVisualizationDataTableRowCell.CellValueAsString = cellValueAsString;
            Cells.Add(googleVisualizationDataTableRowCell);


            return googleVisualizationDataTableRowCell;
        }

        /// <summary>
        /// Required. The cells in the row.
        /// </summary>
        public IList<GoogleVisualizationDataTableRowCell> Cells { get; set; }

        /// <summary>
        /// Optional. The p property is a map of custom values applied to the whole DataTable. These values can be of any JavaScript type. 
        /// If your visualization supports any datatable-level properties, it will describe them; otherwise, this property will be 
        /// ignored. Example: p:{className: 'myDataTable'}.
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; set; }

        internal GoogleVisualizationDataTableRow()
        { }
    }

    public class GoogleVisualizationDataTableColumn
    {
        /// <summary>
        /// Required. Data type of the data in the column. 
        /// </summary>
        public GoogleVisualizationDataTableColumnType ColumnType { get; set; }

        /// <summary>
        /// Optional. String ID of the column. Must be unique in the table. Use basic alphanumeric characters. Example: col_1
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// Optional. String value that some visualizations display for this column.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Optional. String pattern that was used by a data source to format numeric, date, or time column values. 
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Optional. An object that is a map of custom values applied to the cell. These values can be of any JavaScript type
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; set; }

        internal GoogleVisualizationDataTableColumn()
        { }
    }

    public enum GoogleVisualizationDataTableColumnType
    {
        Boolean,
        Number,
        String,
        Date,
        DateTime,
        TimeOfDay,
        TimeOfDayWithMilliSeconds
    }

    public class GoogleVisualizationDataTableRowCell
    {
        /// <summary>
        /// Optional. The cell value. The data type should match the column data type. If null, the whole object should be empty and have neither CellValue nor CellValueAsString 
        /// properties.
        /// </summary>
        public object CellValue { get; set; }

        /// <summary>
        /// Optional. A string version of the v value, formatted for display. The values should match, so if you specify Date(2008, 0, 1) for v, you should specify "January 1, 2008" 
        /// or some such string for this property. This value is not checked against the v value. The visualization will not use this value for calculation, only as a label for 
        /// display. If omitted, a string version of v will be used.
        /// </summary>
        public string CellValueAsString { get; set; }

        /// <summary>
        /// Optional. An object that is a map of custom values applied to the cell. These values can be of any JavaScript type. If your visualization supports 
        /// any cell-level properties, it will describe them; otherwise, this property will be ignored. Example:  p:{style: 'border: 1px solid green;'}.
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; set; }

        internal GoogleVisualizationDataTableRowCell()
        { }
    }
}
