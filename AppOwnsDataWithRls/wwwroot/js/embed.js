$(function () {

  // 1 - get DOM object for div that is report container 
  var reportContainer = document.getElementById("embed-container");

  // 2 - get report embedding data from view model
  var reportId = window.viewModel.reportId;
  var embedUrl = window.viewModel.embedUrl;
  var token = window.viewModel.token;
  var customizationEnabled = window.viewModel.customizationEnabled;

  console.log("customizationEnabled: ", customizationEnabled);

  // 3 - embed report using the Power BI JavaScript API.
  var models = window['powerbi-client'].models;

  var config = {
    type: 'report',
    id: reportId,
    embedUrl: embedUrl,
    accessToken: token,
    permissions: models.Permissions.All,
    tokenType: models.TokenType.Embed,
    viewMode: models.ViewMode.View,
    settings: {
      panes: {
        filters: { expanded: false, visible: true },
        pageNavigation: { visible: false }
      }
    }
  };

  // Embed the report and display it within the div container.
  var report = powerbi.embed(reportContainer, config);

  if (customizationEnabled) {
    var viewMode = "view";
    $("#toggleEdit").click(function () {
      // toggle between view and edit mode
      viewMode = (viewMode == "view") ? "edit" : "view";
      report.switchMode(viewMode);
      // show filter pane when entering edit mode
      var showFilterPane = (viewMode == "edit");
      report.updateSettings({
        "filterPaneEnabled": showFilterPane
      });
    });
    $("#fullScreen").click(function () {
      report.fullscreen();
    });
  }

  // 4 - add logic to resize embed container on window resize event
  var heightBuffer = 12;
  var newHeight = $(window).height() - ($("header").height() + heightBuffer);
  $("#embed-container").height(newHeight);
  $(window).resize(function () {
    var newHeight = $(window).height() - ($("header").height() + heightBuffer);
    $("#embed-container").height(newHeight);
  });

});