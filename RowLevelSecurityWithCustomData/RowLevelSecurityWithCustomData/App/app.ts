import * as $ from 'jquery';

import * as powerbi from "powerbi-client";
import * as models from "powerbi-models";

// ensure Power BI JavaScript API has loaded
require('powerbi-models');
require('powerbi-client');

export class ViewModel {
  reportId: string;
  embedUrl: string;
  token: string;
}


var resizeEmbedContainer = () => {
  var heightBuffer = 12;
  var newHeight = $(window).height() - ($("header").height() + heightBuffer);
  $("#embed-container").height(newHeight);
};

$(() => {

  var reportContainer: HTMLElement = document.getElementById("embed-container");
  var viewModel: ViewModel = window["viewModel"];

  var config: powerbi.IEmbedConfiguration = {
    type: 'report',
    id: viewModel.reportId,
    embedUrl: viewModel.embedUrl,
    accessToken: viewModel.token,
    permissions: models.Permissions.All,
    tokenType: models.TokenType.Embed,
    viewMode: models.ViewMode.View,
    settings: {
      panes: { pageNavigation: { visible: false } }
    }
  };

  var report: powerbi.Report = <powerbi.Report>window.powerbi.embed(reportContainer, config);

  resizeEmbedContainer();
  $(window).on("resize", resizeEmbedContainer);

});

