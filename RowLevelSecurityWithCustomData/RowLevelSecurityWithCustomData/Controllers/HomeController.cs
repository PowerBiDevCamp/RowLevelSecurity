using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RowLevelSecurityWithCustomData.Models;
using RowLevelSecurityWithCustomData.Services;
using static RowLevelSecurityWithCustomData.Services.PowerBiServiceApi;

namespace RowLevelSecurityWithCustomData.Controllers {

  [AllowAnonymous]
  public class HomeController : Controller {

    private PowerBiServiceApi powerBiServiceApi;

    public HomeController(PowerBiServiceApi powerBiServiceApi) {
      this.powerBiServiceApi = powerBiServiceApi;
    }

    public async Task<IActionResult> Index() {

      var user = new RlsUserData {
        Username = "Johnnie B Goode",
        Role = "StatesRole",
        CustomData = "|MA|CT|NY|NJ|PA|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    public async Task<IActionResult> User2() {

      var user = new RlsUserData {
        Username = "Willie Makeit",
        Role = "StatesRole",
        CustomData = "|VM|NH|MA|RI|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    public async Task<IActionResult> User3() {

      var user = new RlsUserData {
        Username = "Wendy Westcoast",
        Role = "StatesRole",
        CustomData = "|WA|OR|CA|AZ|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    public async Task<IActionResult> User4() {

      var user = new RlsUserData {
        Username = "Saul Goodman",
        Role = "RegionsRole",
        CustomData = "|Eastern Region|Central Region|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    public async Task<IActionResult> User5() {

      var user = new RlsUserData {
        Username = "Saul Goodman",
        Role = "RegionsRole",
        CustomData = "|Eastern Region|Central Region|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    public async Task<IActionResult> User6() {

      var user = new RlsUserData {
        Username = "Saul Goodman",
        Role = "RegionsRole",
        CustomData = "|Eastern Region|Central Region|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    public async Task<IActionResult> User7() {

      var user = new RlsUserData {
        Username = "Saul Goodman",
        Role = "RegionsRole",
        CustomData = "|Eastern Region|Central Region|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    public async Task<IActionResult> User8() {

      var user = new RlsUserData {
        Username = "Saul Goodman",
        Role = "RegionsRole",
        CustomData = "|Eastern Region|Central Region|"
      };

      return View(await powerBiServiceApi.GetReport(user));
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
