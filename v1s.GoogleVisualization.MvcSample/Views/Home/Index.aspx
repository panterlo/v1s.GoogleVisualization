<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/DefaultViewPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
v1s.GoogleVisualization MVC Sample
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h3>Annotated Timeline "Chart" Example</h3>
<hr />
<p>
This sample makes an asyncronhous request to the HomeController method GetSomeGoogleVisualizationData passing the sample number of records.
</p>
<div>
<p>
<label>Sample number of records:</label><br />
<select class="half" id="NumRecords">
<option value="1000" selected="selected">1 000 records</option>
<option value="20000">10 000 records</option>
<option value="30000">30 000 records</option>
</select>
</p>
<p>
<button type="button" class="btn" id="GetVisualizationData">Get Visualization Sample Data</button>
</p>
<p>&nbsp;</p>
</div>

<div id="visualization" style="width: 100%; height: 400px; font-family: Arial;border: 0 none;"></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
<!--Load the AJAX API-->
<script type="text/javascript" src="http://www.google.com/jsapi"></script>

  <script type="text/javascript">
      google.load('visualization', '1', { 'packages': ['annotatedtimeline'] });

      function drawVisualization() {
          var numRecords = $("#NumRecords").val();
          var query = new google.visualization.Query(' <%: Url.Action("GetSomeGoogleVisualizationData","Home", new { someParam = "paramValue" }) %>' + '&numRecords=' + numRecords);
          query.send(handleQueryResponse);
      }

      function handleQueryResponse(response) {
          if (response.isError()) {
              alert('Error in query: ' + response.getMessage() + ' ' + response.getDetailedMessage());
              return;
          }

          var data = response.getDataTable();
          var annotatedtimeline = new google.visualization.AnnotatedTimeLine(
          document.getElementById('visualization'));
          annotatedtimeline.draw(data, { 'displayAnnotations': false, 'displayZoomButtons': true, 'displayLegendValues': true, 'displayLegendDots': false });

      }

      google.setOnLoadCallback(drawVisualization);

      $(document).ready(function () {
          $("#GetVisualizationData").click(function () { drawVisualization(); });
      });
</script>
</asp:Content>
