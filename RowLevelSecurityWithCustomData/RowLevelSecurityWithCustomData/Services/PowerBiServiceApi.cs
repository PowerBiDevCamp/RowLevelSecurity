using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Rest;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RowLevelSecurityWithCustomData.Services {

  public class EmbeddedReportViewModel {
    public string Id;
    public string Name;
    public string EmbedUrl;
    public string Token;
    public string Role;
  }

  public class PowerBiServiceApi {

    private ITokenAcquisition tokenAcquisition { get; }
    private string urlPowerBiServiceApiRoot { get; }
    private Guid workspaceId { get; }
    private Guid reportId { get; }

    public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition tokenAcquisition) {
      this.urlPowerBiServiceApiRoot = configuration["PowerBi:ServiceRootUrl"];
      this.workspaceId = new Guid(configuration["PowerBi:WorkspaceId"]);
      this.reportId = new Guid(configuration["PowerBi:CustomDataReportId"]);
      this.tokenAcquisition = tokenAcquisition;
    }

    public const string powerbiApiDefaultScope = "https://analysis.windows.net/powerbi/api/.default";

    public string GetAccessToken() {
      return this.tokenAcquisition.GetAccessTokenForAppAsync(powerbiApiDefaultScope).Result;
    }

    public PowerBIClient GetPowerBiClient() {
      string accessToken = GetAccessToken();
      var tokenCredentials = new TokenCredentials(accessToken, "Bearer");
      return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
    }

    public async Task<EmbeddedReportViewModel> GetReport(RlsUserData rlsUserData) {

      PowerBIClient pbiClient = GetPowerBiClient();

      // call to Power BI Service API to get embedding data
      var report = await pbiClient.Reports.GetReportInGroupAsync(this.workspaceId, this.reportId);
      var datasetId = report.DatasetId;

      var tokenRequest = new GenerateTokenRequestV2 {
        Datasets = new List<GenerateTokenRequestV2Dataset>() {
          new GenerateTokenRequestV2Dataset(datasetId)
        },
        Reports = new List<GenerateTokenRequestV2Report>() {
          new GenerateTokenRequestV2Report(reportId, true)
        },
        TargetWorkspaces = new List<GenerateTokenRequestV2TargetWorkspace>() {
          new GenerateTokenRequestV2TargetWorkspace(workspaceId)
        },
        Identities = new List<EffectiveIdentity>() {
          new EffectiveIdentity {
            Username = rlsUserData.Username,
            Datasets = new List<string>() { datasetId },
            CustomData = rlsUserData.CustomData,
            Roles = new List<string>() { rlsUserData.Role }
          }
        },
        LifetimeInMinutes = 15,
      };

      var embedTokenResponse = await pbiClient.EmbedToken.GenerateTokenAsync(tokenRequest);
      var embedToken = embedTokenResponse.Token;

      // return report embedding data to caller
      return new EmbeddedReportViewModel {
        Id = report.Id.ToString(),
        EmbedUrl = report.EmbedUrl,
        Name = report.Name,
        Token = embedToken,
        Role = rlsUserData.Role
      };
    }


    public async Task<EmbeddedReportViewModel> GetReport2() {


      PowerBIClient pbiClient = GetPowerBiClient();

      // call to Power BI Service API to get embedding data
      var report = await pbiClient.Reports.GetReportInGroupAsync(this.workspaceId, this.reportId);
      var datasetId = report.DatasetId;

      var tokenRequest = new GenerateTokenRequestV2 {
        LifetimeInMinutes = 15,
        Datasets = new List<GenerateTokenRequestV2Dataset>() {
          new GenerateTokenRequestV2Dataset(datasetId)
        },
        Reports = new List<GenerateTokenRequestV2Report>() {
          new GenerateTokenRequestV2Report(reportId, true)
        },
        TargetWorkspaces = new List<GenerateTokenRequestV2TargetWorkspace>() {
          new GenerateTokenRequestV2TargetWorkspace(workspaceId)
        },
        Identities = new List<EffectiveIdentity>() {
          new EffectiveIdentity {
            Username = "user1.powerbidevcamp.net",
            Datasets = new List<string>() { datasetId },
            CustomData = "",
            Roles = new List<string>() { "StatesRole" }
          }
        }
      };

      var embedTokenResponse = await pbiClient.EmbedToken.GenerateTokenAsync(tokenRequest);
      var embedToken = embedTokenResponse.Token;

      // return report embedding data to caller
      return new EmbeddedReportViewModel {
        Id = report.Id.ToString(),
        EmbedUrl = report.EmbedUrl,
        Name = report.Name,
        Token = embedToken
      };
    }

    public async Task<EmbeddedReportViewModel> GetReport3() {

      PowerBIClient pbiClient = GetPowerBiClient();

      // call to Power BI Service API to get embedding data
      var report = await pbiClient.Reports.GetReportInGroupAsync(this.workspaceId, this.reportId);
      var datasetId = report.DatasetId;

      var tokenRequest = new GenerateTokenRequestV2 {
        LifetimeInMinutes = 15,
        Datasets = new List<GenerateTokenRequestV2Dataset>() {
          new GenerateTokenRequestV2Dataset(datasetId)
        },
        Reports = new List<GenerateTokenRequestV2Report>() {
          new GenerateTokenRequestV2Report(reportId, true)
        },
        TargetWorkspaces = new List<GenerateTokenRequestV2TargetWorkspace>() {
          new GenerateTokenRequestV2TargetWorkspace(workspaceId)
        },
        Identities = new List<EffectiveIdentity>() {
          new EffectiveIdentity {
            Username = "user1.powerbidevcamp.net",
            Datasets = new List<string>() { datasetId },
            CustomData = "",
            Roles = new List<string>() { "" }
          }
        }
      };

      var embedTokenResponse = await pbiClient.EmbedToken.GenerateTokenAsync(tokenRequest);
      var embedToken = embedTokenResponse.Token;

      // return report embedding data to caller
      return new EmbeddedReportViewModel {
        Id = report.Id.ToString(),
        EmbedUrl = report.EmbedUrl,
        Name = report.Name,
        Token = embedToken
      };
    }

    public class RlsUserData {
      public string Username;
      public string Role;
      public string CustomData;
    }

   



  }





}


