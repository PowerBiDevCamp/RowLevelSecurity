using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppOwnsData.Models;
using AppOwnsData.Services;

namespace AppOwnsData.Controllers {

  [Authorize]
  public class HomeController : Controller {

    private PowerBiServiceApi powerBiServiceApi;

    public HomeController(PowerBiServiceApi powerBiServiceApi) {
      this.powerBiServiceApi = powerBiServiceApi;
    }

    [AllowAnonymous]
    public IActionResult Index() {
      return View();
    }

    public string[] GetUserRoles() {      
   
      var roles = new List<string>();
   
      if (User.HasClaim("groups", "7341a19a-ce03-4aa2-82c7-cf880639eb85")) {
        roles.Add("Eastern Region");
      }

      if (User.HasClaim("groups", "7bb3a4a4-62fa-4070-b75e-d74e8a5630c6")) {
        roles.Add("Central Region");
      }

      if (User.HasClaim("groups", "fcd9ac74-b9c9-4ec0-8b5c-020d098262b7")) {
        roles.Add("Western Region");
      }

      return roles.ToArray();
    }

    public bool UserCanCustomizeReport() {
      return User.HasClaim("groups", "9cb0f004-d036-4797-9039-52917b048a57");
    }

    public async Task<IActionResult> EmbedWithRls() {
      
      bool customizationEnabled = UserCanCustomizeReport();

      string userName = User.Identity.Name;
      string[] userRoles = GetUserRoles();
      
      if(userRoles.Length == 0) {
        return View("Error");
      }

      var viewModel = await powerBiServiceApi.GetReportWithRls(userName, userRoles, customizationEnabled);

      return View(viewModel);
    }

    public async Task<IActionResult> EmbedWithRlsV2() {

      bool customizationEnabled = UserCanCustomizeReport();

      string userName = User.Identity.Name;
      string[] userRoles = GetUserRoles();

      if (userRoles.Length == 0) {
        return View("Error");
      }

      var viewModel = await powerBiServiceApi.GetReportWithRlsV2(userName, userRoles, customizationEnabled);

      return View(viewModel);
    }

    public IActionResult UserInfo() {
      return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
