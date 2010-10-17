using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace v1s.GoogleVisualization
{
    public class GoogleVisualizationRequest
    {
        /// <summary>
        /// Matches the tq parameter in the query string. This parameter is not required
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Required in request. Data source must handle. A numeric identifier for this request. 
        /// This is used so that if a client sends multiple requests before receiving a response,
        /// the data source can identify the response with the proper request. Send this value back in the response.
        /// </summary>
        public uint RequestId { get; private set; }

        /// <summary>
        /// Optional in request. Data source must handle. The version number of the Google Visualization protocol. Current version is 0.6. If not sent, assume the latest version.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// [Optional in request; Optional for data source to handle] A hash of the DataTable  sent to this client in any previous requests to this data source. 
        /// This is an optimization to avoid sending identical data to a client twice.
        /// </summary>
        public string HashSig { get; private set; }

        /// <summary>
        /// [Optional in request; Data source must handle] A string describing the format for the returned data. It can be any of the following values: 
        /// </summary>
        public GoogleVisualizationResponseFormat? ResponseFormat { get; private set; }

        /// <summary>
        /// [Optional in request; Data source must handle] The string name of the JavaScript handling function on the client page 
        /// that will be called with the response. If not included in the request, the value is "google.visualization.Query.setResponse".
        /// This will be sent back as part of the response;
        /// </summary>
        public string ResponseHandler { get; private set; }

        /// <summary>
        /// [Optional in request; Optional for data source to handle] If you specify out:csv  or out:tsv-excel, you can optionally request the file name specified here.
        /// </summary>
        public string OutFilename { get; private set; }

        /// <summary>
        /// All other parameters that are not included in the standard list is placed in this property
        /// </summary>
        public NameValueCollection CustomParameters { get; private set; }

        public GoogleVisualizationRequest(NameValueCollection queryParameters)
        {
            ResponseHandler = "google.visualization.Query.setResponse";

            foreach (string key in queryParameters.AllKeys)
            {
                switch (key.ToLower())
                {
                    case "tq":
                        if (string.IsNullOrWhiteSpace(queryParameters[key]) == false) Query = queryParameters[key];
                        continue;
                    case "tqx":
                        if (string.IsNullOrWhiteSpace(queryParameters[key]) == false)
                        {
                            string[] tqxContent = queryParameters[key].Split(';');
                            foreach (string tqxParameter in tqxContent)
                            {
                                string[] tqxUnit = tqxParameter.Split(':');
                                if (tqxUnit.Length < 1) // This means value is missing, continue on next parameter
                                    continue;

                                switch (tqxUnit[0].ToLower())
                                {
                                    case "reqid":
                                        if (tqxUnit.Length != 2) break;
                                        uint reqId;
                                        bool reqIdParseResult = uint.TryParse(tqxUnit[1], out reqId);
                                        if (reqIdParseResult == false)
                                            throw new ArgumentException("The supplied reqId is not valid, it must be an unsigned integer");
                                        RequestId = reqId;
                                        break;

                                    case "version":
                                        if (tqxUnit.Length != 2) break;
                                        Version version;
                                        bool versionParseResult = Version.TryParse(tqxUnit[1], out version);
                                        if (versionParseResult == false)
                                            throw new ArgumentException("The supplied version is not valid version number, version numbers should be in the form of x.x.x.x where x is an unsigned integer");
                                        Version = version;
                                        break;

                                    case "sig":
                                        if (tqxUnit.Length != 2) break;
                                        if (string.IsNullOrWhiteSpace(tqxUnit[1]) == false) HashSig = tqxUnit[1];
                                        break;

                                    case "out":
                                        if (tqxUnit.Length != 2) break;
                                        if (tqxUnit.Length != 2) break;
                                        if (tqxUnit[1].ToLower() == "tsv-excel")
                                            ResponseFormat = GoogleVisualizationResponseFormat.TsvExcel;
                                        else
                                        {
                                            GoogleVisualizationResponseFormat format;
                                            bool outParseResult = Enum.TryParse(tqxUnit[1], true, out format);
                                            if (outParseResult == false)
                                                throw new ArgumentException("The supplied out parameter is not valid, is must be json, html, csv or tsv-excel");
                                            ResponseFormat = format;
                                        }
                                        break;

                                    case "responsehandler":
                                        if (tqxUnit.Length != 2) break;
                                      
                                            if (string.IsNullOrWhiteSpace(tqxUnit[1]) == false)
                                                ResponseHandler = tqxUnit[1];
                                            else
                                                ResponseHandler = "google.visualization.Query.setResponse";
                                    
                                        break;

                                    case "outfilename ":
                                        if (tqxUnit.Length >= 2)
                                            OutFilename = tqxUnit[1];
                                        break;

                                    default:
                                        if (tqxUnit.Length > 2)
                                        {
                                            if (CustomParameters == null)
                                                CustomParameters = new NameValueCollection();

                                            string values = string.Empty;
                                            for (int i = 1; i < tqxUnit.Length; i++)
                                            {
                                                values = values + tqxUnit[i];
                                            }

                                            CustomParameters.Add(tqxUnit[0], values);
                                        }
                                        break;
                                }

                            }
                        }
                        break;
                }

            }

        }

        public GoogleVisualizationRequest()
        {
            ResponseHandler = "google.visualization.Query.setResponse";
        }
    }

    public class GoogleVisualizationResponse
    {
        private Version DefaultGoogleVisualizationVersion = new Version("0.6");

        private IList<WarningInfo> warningInfo = null;
        private IList<ErrorInfo> errorInfo = null;
        private GoogleVisualizationResponseStatus status = GoogleVisualizationResponseStatus.Ok;
        private GoogleVisualizationDataTable googleVisualizationDataTable = null;

        public Version Version { get; set; }

        public uint? RequestId { get; set; }

        public GoogleVisualizationResponseStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != GoogleVisualizationResponseStatus.Warning)
                {
                    if (warningInfo != null)
                        warningInfo = null;
                }
                else if (value != GoogleVisualizationResponseStatus.Error)
                {
                    if (errorInfo != null)
                        errorInfo = null;
                }


                status = value;
            }

        }

        public IList<WarningInfo> WarningInfo
        {
            get
            {
                return warningInfo;
            }
            set
            {
                if (Status != GoogleVisualizationResponseStatus.Warning)
                    throw new ArgumentException("The property WarningInfo can only be set if status is Warning");

                warningInfo = value;
            }

        }

        public IList<ErrorInfo> ErrorInfo
        {
            get
            {
                return errorInfo;
            }
            set
            {
                if (Status != GoogleVisualizationResponseStatus.Error)
                    throw new ArgumentException("The property ErrorInfo can only be set if status is Error");

                errorInfo = value;
            }

        }

        public string HashSig { get; set; }

        public GoogleVisualizationDataTable GoogleVisualizationDataTable
        {
            get
            {
                if (googleVisualizationDataTable != null && Status == GoogleVisualizationResponseStatus.Error)
                    return null;

                return googleVisualizationDataTable;
            }

            set
            {
                if (googleVisualizationDataTable != null && Status == GoogleVisualizationResponseStatus.Error)
                    throw new ArgumentException("You can't set a GoogleVisualizationDataTable when the status is set to Error");

                googleVisualizationDataTable = value;
            }
        }

        public GoogleVisualizationRequest GoogleVisualizationRequest { get; private set; }

        public GoogleVisualizationResponse(GoogleVisualizationRequest googleVisualizationRequest)
        {
            GoogleVisualizationRequest = googleVisualizationRequest;

            if (GoogleVisualizationRequest.Version == null)
                Version = DefaultGoogleVisualizationVersion;
            else
                Version = googleVisualizationRequest.Version;

            RequestId = googleVisualizationRequest.RequestId;
        }
    }

    public class WarningInfo
    {
        /// <summary>
        /// Required. A one-word string description of the warning. 
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Optional. A short quoted string explaining the problem, possibly used as a title for an alert box. This might be displayed to the user. HTML is not supported.
        /// </summary>
        public string ShortMessage { get; set; }

        /// <summary>
        /// Optional. A detailed quoted string message explaining the problem, and any possible solutions. The only HTML supported is the <a> element with 
        /// a single href attribute. Unicode encoding is supported. 
        /// </summary>
        public string DetailedMessage { get; set; }

        public WarningInfo(string reason, string shortMessage = null, string detailedMessage = null)
        {
            if (string.IsNullOrWhiteSpace(reason) == true)
                throw new ArgumentException("Reason is required");

            Reason = reason;
            ShortMessage = shortMessage;
            DetailedMessage = detailedMessage;
        }

        public WarningInfo(GoogleVisualizationWarningStandardReason reason, string shortMessage = null, string detailedMessage = null)
        {
            switch (reason)
            {
                case GoogleVisualizationWarningStandardReason.DataTruncated:
                    Reason = "data_truncated";
                    break;
                case GoogleVisualizationWarningStandardReason.Other:
                    Reason = "other";
                    break;
                default:
                    throw new ApplicationException("You must implement this standard reason in the WarningInfo constructor");
            }

            ShortMessage = shortMessage;
            DetailedMessage = detailedMessage;
        }
    }

    public class ErrorInfo
    {
        /// <summary>
        /// Required. A one-word string description of the warning. 
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Optional. A short quoted string explaining the problem, possibly used as a title for an alert box. This might be displayed to the user. HTML is not supported.
        /// </summary>
        public string ShortMessage { get; set; }

        /// <summary>
        /// Optional. A detailed quoted string message explaining the problem, and any possible solutions. The only HTML supported is the <a> element with 
        /// a single href attribute. Unicode encoding is supported. 
        /// </summary>
        public string DetailedMessage { get; set; }

        public ErrorInfo(string reason, string shortMessage = null, string detailedMessage = null)
        {
            if (string.IsNullOrWhiteSpace(reason) == true)
                throw new ArgumentException("Reason is required");

            Reason = reason;
            ShortMessage = shortMessage;
            DetailedMessage = detailedMessage;
        }

        public ErrorInfo(GoogleVisualizationErrorStandardReason reason, string shortMessage = null, string detailedMessage = null)
        {
            switch (reason)
            {
                case GoogleVisualizationErrorStandardReason.NotModified:
                    Reason = "not_modfied";
                    break;
                case GoogleVisualizationErrorStandardReason.UserNotAuthenticated:
                    Reason = "user_not_authenticated";
                    break;
                case GoogleVisualizationErrorStandardReason.UnknownDataSourceId:
                    Reason = "unknown_data_source_id";
                    break;
                case GoogleVisualizationErrorStandardReason.AccessDenied:
                    Reason = "access_denied";
                    break;
                case GoogleVisualizationErrorStandardReason.UnsupportedQueryOption:
                    Reason = "unsupported_query_option";
                    break;
                case GoogleVisualizationErrorStandardReason.InvalidQuery:
                    Reason = "invalid_query";
                    break;
                case GoogleVisualizationErrorStandardReason.InvalidRequest:
                    Reason = "invalid_request";
                    break;
                case GoogleVisualizationErrorStandardReason.InternalError:
                    Reason = "internal_error";
                    break;
                case GoogleVisualizationErrorStandardReason.NotSupported:
                    Reason = "not_supported";
                    break;
                case GoogleVisualizationErrorStandardReason.IllegalFormattingPatterns:
                    Reason = "illegal_formatting_patterns";
                    break;
                case GoogleVisualizationErrorStandardReason.Other:
                    Reason = "other";
                    break;
                default:
                    throw new ApplicationException("You must implement this standard reason in the ErrorInfo constructor");
            }


            ShortMessage = shortMessage;
            DetailedMessage = detailedMessage;
        }


    }
}
