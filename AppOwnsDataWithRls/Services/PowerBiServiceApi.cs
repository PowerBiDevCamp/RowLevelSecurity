using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Rest;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;

namespace AppOwnsData.Services {

  public class EmbeddedReportViewModel {
    public string Id;
    public string Name;
    public string EmbedUrl;
    public string Token;
    public bool CustomizationEnabled;
  }

  public class PowerBiServiceApi {

    private ITokenAcquisition tokenAcquisition { get; }
    private string urlPowerBiServiceApiRoot { get; }
    private Guid WorkspaceId{ get; }
    private Guid RlsReportId { get; }

    public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition tokenAcquisition) {
      this.urlPowerBiServiceApiRoot = configuration["PowerBi:ServiceRootUrl"];
      this.WorkspaceId = new Guid(configuration["PowerBi:WorkspaceId"]);
      this.RlsReportId = new Guid(configuration["PowerBi:RlsReportId"]);
      this.tokenAcquisition = tokenAcquisition;
    }

    public const string powerbiApiDefaultScope = "https://analysis.windows.net/powerbi/api/.default";

    public string GetAccessToken() {
      return this.tokenAcquisition.GetAccessTokenForAppAsync(powerbiApiDefaultScope).Result;
    }

    public PowerBIClient GetPowerBiClient() {
      var tokenCredentials = new TokenCredentials(GetAccessToken(), "Bearer");
      return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
    }

    public async Task<EmbeddedReportViewModel> GetReportWithRls(string UserName, string[] Roles, bool CustomizationEnabled) {

      PowerBIClient pbiClient = GetPowerBiClient();

      // call to Power BI Service API to get embedding data
      var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId, RlsReportId);

      // generate read-only embed token for the report
      var datasetId = report.DatasetId;
      var datasetList = new List<string>() { report.DatasetId };

      // create EffectiveIdentity object
      var effectiveIdentity = new EffectiveIdentity(UserName, datasetList, Roles);

      // generate embed token
      TokenAccessLevel tokenAccessLevel = CustomizationEnabled ? TokenAccessLevel.Edit : TokenAccessLevel.View;
      var tokenRequest = new GenerateTokenRequest(tokenAccessLevel, datasetId, effectiveIdentity);
      var embedTokenResponse = await pbiClient.Reports.GenerateTokenAsync(WorkspaceId, RlsReportId, tokenRequest);
      var embedToken = embedTokenResponse.Token;

      // return report embedding data to caller
      return new EmbeddedReportViewModel {
        Id = report.Id.ToString(),
        EmbedUrl = report.EmbedUrl,
        Name = report.Name,
        Token = embedToken,
        CustomizationEnabled = CustomizationEnabled
      };
    }


    public async Task<EmbeddedReportViewModel> GetReportWithRlsV2(string UserName, string[] Roles, bool CustomizationEnabled) {

      PowerBIClient pbiClient = GetPowerBiClient();

      // call to Power BI Service API to get embedding data
      var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId, RlsReportId);

      // generate read-only embed token for the report
      var datasetId = report.DatasetId;

      // generate V2 embed token requests for dataset report and workspace
      IList<GenerateTokenRequestV2Dataset> datasetRequests = new List<GenerateTokenRequestV2Dataset>();
      datasetRequests.Add(new GenerateTokenRequestV2Dataset(datasetId));

      IList<GenerateTokenRequestV2Report> reportRequests = new List<GenerateTokenRequestV2Report>();
      reportRequests.Add(new GenerateTokenRequestV2Report(report.Id, allowEdit: CustomizationEnabled));

      var workspaceRequests = new List<GenerateTokenRequestV2TargetWorkspace>();
      if (CustomizationEnabled) {
        workspaceRequests.Add(new GenerateTokenRequestV2TargetWorkspace(WorkspaceId));
      }

      // create EffectiveIdentity object
      var datasetList = new List<string>() { report.DatasetId };
      IList<EffectiveIdentity> effectiveIdentities =
        new List<EffectiveIdentity> { new EffectiveIdentity(UserName, datasetList, Roles) };

      /// create top-level V2 token request
      GenerateTokenRequestV2 tokenRequest =
        new GenerateTokenRequestV2 {
          Datasets = datasetRequests,
          Reports = reportRequests,
          TargetWorkspaces = workspaceRequests,
          Identities = effectiveIdentities,
          LifetimeInMinutes = 10
        };

      // call to Power BI Service API and pass GenerateTokenRequestV2 object to generate embed token
      var embedTokenResponse = pbiClient.EmbedToken.GenerateToken(tokenRequest);
      var embedToken = embedTokenResponse.Token;

      // return report embedding data to caller
      return new EmbeddedReportViewModel {
        Id = report.Id.ToString(),
        EmbedUrl = report.EmbedUrl,
        Name = report.Name, 
        Token = embedToken,
        CustomizationEnabled = CustomizationEnabled
      };
    }

  }
}
