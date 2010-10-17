//    Copyright 2010 Panterlo AB, Jens Nylander, jens@panterlo.com   
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace v1s.GoogleVisualization
{
    public enum GoogleVisualizationResponseFormat
    {
        Json,
        Html,
        Csv,
        TsvExcel
    }

    public enum GoogleVisualizationResponseStatus
    {
        Ok,
        Warning,
        Error
    }

    public enum GoogleVisualizationWarningStandardReason
    {
        DataTruncated,
        Other
    }

    public enum GoogleVisualizationErrorStandardReason
    {
        NotModified,
        UserNotAuthenticated,
        UnknownDataSourceId,
        AccessDenied,
        UnsupportedQueryOption,
        InvalidQuery,
        InvalidRequest,
        InternalError,
        NotSupported,
        IllegalFormattingPatterns,
        Other
    }
}
