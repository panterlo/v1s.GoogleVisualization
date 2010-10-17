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
