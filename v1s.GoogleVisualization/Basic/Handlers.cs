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
using System.Text;
using System.IO;

namespace v1s.GoogleVisualization
{
    public static class GoogleVisualizationHandler
    {
        public static string GetGoogleVisualizationResponse(GoogleVisualizationResponse googleVisualizationResponse)
        {
            if (googleVisualizationResponse == null)
                throw new ArgumentException("The googleVisualizationResponse is required");

            if (googleVisualizationResponse.GoogleVisualizationRequest == null) // failsafe
                throw new ArgumentException("The GoogleVisualizationRequest property is required in the googleVisualizationResponse instance");

            StringBuilder result = new StringBuilder();

            result.Append(googleVisualizationResponse.GoogleVisualizationRequest.ResponseHandler);
            result.Append("({");
            result.Append(string.Format("version:'{0}',", googleVisualizationResponse.Version.ToString()));
            result.Append(string.Format("reqId:'{0}',", googleVisualizationResponse.RequestId.ToString()));
            result.Append(string.Format("status:'{0}'", Enum.GetName(typeof(GoogleVisualizationResponseStatus), googleVisualizationResponse.Status).ToLower()));
            if (googleVisualizationResponse.WarningInfo != null)
            {
                result.Append(",warnings:[{");

                foreach (WarningInfo warningInfo in googleVisualizationResponse.WarningInfo)
                {
                    result.Append(string.Format("reason:'{0}'", warningInfo.Reason));
                    if (string.IsNullOrWhiteSpace(warningInfo.ShortMessage) == false)
                        result.Append(string.Format(",message:'{0}'", warningInfo.ShortMessage));
                    if (string.IsNullOrWhiteSpace(warningInfo.DetailedMessage) == false)
                        result.Append(string.Format(",detailed_message:'{0}'", warningInfo.DetailedMessage));
                }
                result.Append("}]");
            }

            if (googleVisualizationResponse.ErrorInfo != null)
            {
                result.Append(",errors:[{");

                foreach (ErrorInfo errorInfo in googleVisualizationResponse.ErrorInfo)
                {
                    result.Append(string.Format("reason:'{0}'", errorInfo.Reason));
                    if (string.IsNullOrWhiteSpace(errorInfo.ShortMessage) == false)
                        result.Append(string.Format(",message:'{0}'", errorInfo.ShortMessage));
                    if (string.IsNullOrWhiteSpace(errorInfo.DetailedMessage) == false)
                        result.Append(string.Format(",detailed_message:'{0}'", errorInfo.DetailedMessage));
                }
                result.Append("}]");
            }
            if (string.IsNullOrWhiteSpace(googleVisualizationResponse.HashSig) == false)
                result.Append(string.Format(",sig:'{0}',", googleVisualizationResponse.HashSig));

            if (googleVisualizationResponse.GoogleVisualizationDataTable != null)
            {
                result.Append(string.Format(",table:{0}", JsonSerializeGoogleVisualizationDataTable(googleVisualizationResponse.GoogleVisualizationDataTable)));
            }

            result.Append("});");
     
            return result.ToString();
        }

        public static string JsonSerializeGoogleVisualizationDataTable(GoogleVisualizationDataTable googleVisualizationDataTable)
        {
            MemoryStream memoryStream = null;
            StreamWriter streamWriter = null;
            StreamReader streamReader = null;

            try
            {
                memoryStream = new MemoryStream();
                streamWriter = new StreamWriter(memoryStream);
                // Start
                streamWriter.Write('{');

                #region Writing Json columns
                streamWriter.Write("cols:[");

                for (int i = 0; i < googleVisualizationDataTable.Columns.Count(); i++)
                {
                    GoogleVisualizationDataTableColumn column = googleVisualizationDataTable.Columns[i];

                    streamWriter.Write('{');

                    if (string.IsNullOrWhiteSpace(column.ColumnId) == false) streamWriter.Write(string.Format("id: '{0}',", column.ColumnId));
                    if (string.IsNullOrWhiteSpace(column.Label) == false) streamWriter.Write(string.Format("label: '{0}',", column.Label));
                    if (string.IsNullOrWhiteSpace(column.Pattern) == false) streamWriter.Write(string.Format("pattern: '{0}',", column.Pattern));
                    if (column.CustomProperties != null)
                    {
                        if (column.CustomProperties.Count() > 0)
                        {
                            streamWriter.Write("p: {");
                            foreach (KeyValuePair<string, object> pair in column.CustomProperties)
                            {
                                if (pair.Value != null)
                                    streamWriter.Write(string.Format("{0}: '{1}',", pair.Key, pair.Value));
                            }
                            streamWriter.Write("},");
                        }
                    }

                    streamWriter.Write(string.Format("type: '{0}'", Enum.GetName(typeof(GoogleVisualizationDataTableColumnType), column.ColumnType).ToLower()));

                    if (i != googleVisualizationDataTable.Columns.Count() - 1)
                        streamWriter.Write("},");
                    else
                        streamWriter.Write('}');
                }
                streamWriter.Write("],");
                #endregion

                #region Writing Json rows
                streamWriter.Write("rows:[");

                for (int j = 0; j < googleVisualizationDataTable.Rows.Count(); j++)
                {
                    GoogleVisualizationDataTableRow row = googleVisualizationDataTable.Rows[j];

                    streamWriter.Write("{c:[");


                    for (int i = 0; i < row.Cells.Count(); i++)
                    {
                        GoogleVisualizationDataTableRowCell cell = row.Cells[i];
                        streamWriter.Write("{");
                        if (cell.CellValue != null) streamWriter.Write(string.Format("v: {0}", GetCellValueBasedOnColumnType(googleVisualizationDataTable.Columns[i].ColumnType, cell.CellValue)));
                        if (string.IsNullOrWhiteSpace(cell.CellValueAsString) == false) streamWriter.Write(string.Format(", f: '{0}'", cell.CellValueAsString));

                        if (cell.CustomProperties != null)
                        {
                            if (cell.CustomProperties.Count() > 0)
                            {
                                streamWriter.Write(", p: {");
                                foreach (KeyValuePair<string, object> pair in cell.CustomProperties)
                                {
                                    if (pair.Value != null)
                                        streamWriter.Write(string.Format("{0}: '{1}',", pair.Key, pair.Value));
                                }
                                streamWriter.Write("}");
                            }
                        }

                        if (i != row.Cells.Count() - 1)
                            streamWriter.Write("},");
                        else
                            streamWriter.Write('}');
                    }

                    if (j != googleVisualizationDataTable.Rows.Count() - 1)
                        streamWriter.Write("]},");
                    else
                        streamWriter.Write("]}");
                }
                streamWriter.Write("]");
                #endregion

                #region Writing custom properties

                if (googleVisualizationDataTable.CustomProperties != null)
                {
                    if (googleVisualizationDataTable.CustomProperties.Count() > 0)
                    {
                        streamWriter.Write(", p: {");
                        foreach (KeyValuePair<string, object> pair in googleVisualizationDataTable.CustomProperties)
                        {
                            if (pair.Value != null)
                                streamWriter.Write(string.Format("{0}: '{1}',", pair.Key, pair.Value));
                        }
                        streamWriter.Write("}");
                    }
                }
                #endregion

                // End
                streamWriter.Write('}');

                streamWriter.Flush();

                memoryStream.Seek(0, SeekOrigin.Begin); // Rewind stream
                streamReader = new StreamReader(memoryStream);
                string content = streamReader.ReadToEnd();
                return (content);
            }
            finally
            {
                //if (memoryStream != null) memoryStream.Close();
                //if (streamWriter != null) streamWriter.Close();
                //if (streamReader != null) streamReader.Close();

                //memoryStream.Dispose();
                //streamWriter.Dispose();
                //streamReader.Dispose();
            }

            return null;
        }

        public static void JsonSerializeGoogleVisualizationDataTable(GoogleVisualizationDataTable googleVisualizationDataTable, ref MemoryStream memoryStream)
        {
            if (memoryStream == null)
                throw new ArgumentException("You need to supply an instantiated memory stream and pass it by reference to this method");

            StreamWriter streamWriter = null;

            try
            {
                memoryStream = new MemoryStream();
                streamWriter = new StreamWriter(memoryStream);
                // Start
                streamWriter.Write('{');

                #region Writing Json columns
                streamWriter.Write("cols:[");

                for (int i = 0; i < googleVisualizationDataTable.Columns.Count(); i++)
                {
                    GoogleVisualizationDataTableColumn column = googleVisualizationDataTable.Columns[i];

                    streamWriter.Write('{');

                    if (string.IsNullOrWhiteSpace(column.ColumnId) == false) streamWriter.Write(string.Format("id: '{0}',", column.ColumnId));
                    if (string.IsNullOrWhiteSpace(column.Label) == false) streamWriter.Write(string.Format("label: '{0}',", column.Label));
                    if (string.IsNullOrWhiteSpace(column.Pattern) == false) streamWriter.Write(string.Format("pattern: '{0}',", column.Pattern));
                    if (column.CustomProperties != null)
                    {
                        if (column.CustomProperties.Count() > 0)
                        {
                            streamWriter.Write("p: {");
                            foreach (KeyValuePair<string, object> pair in column.CustomProperties)
                            {
                                if (pair.Value != null)
                                    streamWriter.Write(string.Format("{0}: '{1}',", pair.Key, pair.Value));
                            }
                            streamWriter.Write("},");
                        }
                    }

                    streamWriter.Write(string.Format("type: '{0}'", Enum.GetName(typeof(GoogleVisualizationDataTableColumnType), column.ColumnType).ToLower()));

                    if (i != googleVisualizationDataTable.Columns.Count() - 1)
                        streamWriter.Write("},");
                    else
                        streamWriter.Write('}');
                }
                streamWriter.Write("],");
                #endregion

                #region Writing Json rows
                streamWriter.Write("rows:[");

                for (int j = 0; j < googleVisualizationDataTable.Rows.Count(); j++)
                {
                    GoogleVisualizationDataTableRow row = googleVisualizationDataTable.Rows[j];

                    streamWriter.Write("{c:[");


                    for (int i = 0; i < row.Cells.Count(); i++)
                    {
                        GoogleVisualizationDataTableRowCell cell = row.Cells[i];
                        streamWriter.Write("{");
                        if (cell.CellValue != null) streamWriter.Write(string.Format("v: {0}", GetCellValueBasedOnColumnType(googleVisualizationDataTable.Columns[i].ColumnType, cell.CellValue)));
                        if (string.IsNullOrWhiteSpace(cell.CellValueAsString) == false) streamWriter.Write(string.Format(", f: '{0}'", cell.CellValueAsString));

                        if (cell.CustomProperties != null)
                        {
                            if (cell.CustomProperties.Count() > 0)
                            {
                                streamWriter.Write(", p: {");
                                foreach (KeyValuePair<string, object> pair in cell.CustomProperties)
                                {
                                    if (pair.Value != null)
                                        streamWriter.Write(string.Format("{0}: '{1}',", pair.Key, pair.Value));
                                }
                                streamWriter.Write("}");
                            }
                        }

                        if (i != row.Cells.Count() - 1)
                            streamWriter.Write("},");
                        else
                            streamWriter.Write('}');
                    }

                    if (j != googleVisualizationDataTable.Rows.Count() - 1)
                        streamWriter.Write("]},");
                    else
                        streamWriter.Write("]}");
                }
                streamWriter.Write("]");
                #endregion

                #region Writing custom properties

                if (googleVisualizationDataTable.CustomProperties != null)
                {
                    if (googleVisualizationDataTable.CustomProperties.Count() > 0)
                    {
                        streamWriter.Write(", p: {");
                        foreach (KeyValuePair<string, object> pair in googleVisualizationDataTable.CustomProperties)
                        {
                            if (pair.Value != null)
                                streamWriter.Write(string.Format("{0}: '{1}',", pair.Key, pair.Value));
                        }
                        streamWriter.Write("}");
                    }
                }
                #endregion

                // End
                streamWriter.Write('}');

                streamWriter.Flush();
            }
            finally
            {
                streamWriter.Close();
            }
        }

        public static string GetCellValueBasedOnColumnType(GoogleVisualizationDataTableColumnType googleVisualizationDataTableColumnType, object value)
        {
            if (value == null)
                throw new ArgumentException("Value parameter is required");

            switch (googleVisualizationDataTableColumnType)
            {
                case GoogleVisualizationDataTableColumnType.Boolean:
                    if (value.GetType() != typeof(bool))
                        throw new ArgumentException("The value object is not a boolean type as indicated by the GoogleVisualizationDataTableColumnType");
                    bool boolValue = (bool)value;
                    if (boolValue == true)
                        return "'true'";
                    return "'false'";
                case GoogleVisualizationDataTableColumnType.Number:
                    // Programmers responsibility
                    return value.ToString();
                case GoogleVisualizationDataTableColumnType.String:
                    return "'" + (string)value + "'";
                case GoogleVisualizationDataTableColumnType.Date:
                    if (value.GetType() != typeof(DateTime))
                        throw new ArgumentException("The value object is not a DateTime type as indicated by the GoogleVisualizationDataTableColumnType");
                    {
                        DateTime dateTimeValue = (DateTime)value;

                        return string.Format("new Date({0},{1},{2})", dateTimeValue.Year, dateTimeValue.Month - 1, dateTimeValue.Day);
                    }
                case GoogleVisualizationDataTableColumnType.DateTime:
                    if (value.GetType() != typeof(DateTime))
                        throw new ArgumentException("The value object is not a DateTime type as indicated by the GoogleVisualizationDataTableColumnType");
                    {
                        DateTime dateTimeValue = (DateTime)value;

                        return string.Format("new Date({0},{1},{2},{3},{4},{5})", dateTimeValue.Year, dateTimeValue.Month - 1, dateTimeValue.Day, dateTimeValue.Hour, dateTimeValue.Minute, dateTimeValue.Second);
                    }
                case GoogleVisualizationDataTableColumnType.TimeOfDay:
                    if (value.GetType() != typeof(DateTime))
                        throw new ArgumentException("The value object is not a DateTime type as indicated by the GoogleVisualizationDataTableColumnType");
                    {
                        DateTime dateTimeValue = (DateTime)value;

                        return string.Format("[{0}, {1}, {2}]", dateTimeValue.Hour, dateTimeValue.Minute, dateTimeValue.Second);
                    }
                case GoogleVisualizationDataTableColumnType.TimeOfDayWithMilliSeconds:
                    if (value.GetType() != typeof(DateTime))
                        throw new ArgumentException("The value object is not a DateTime type as indicated by the GoogleVisualizationDataTableColumnType");
                    {
                        DateTime dateTimeValue = (DateTime)value;

                        return string.Format("[{0}, {1}, {2}, {4}]", dateTimeValue.Hour, dateTimeValue.Minute, dateTimeValue.Second, dateTimeValue.Millisecond);
                    }
                default:
                    throw new ArgumentException("This GoogleVisualizationDataTableColumnType is not implemented in GetCellValueBasedOnColumnType method which is required");
            }

        }
    }
}
