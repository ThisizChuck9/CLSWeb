using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CLSWeb.Models;

namespace CLSWeb.Controllers;

public class HomeController : Controller
{

    // Handles requests to:
    // /Home
    // /Home/Index
    // If the member is already logged in, the member is
    // redirected to the Dashboard.
    // If no member is logged in, the user is redirected
    // to the Login page.

    public IActionResult Index()
    {
        // Retrieves the MemberID stored in the ASP.NET Core
        // session when the member is successfully logged in.
        var memberID =
            HttpContext.Session.GetInt32("MemberID");

        // If MemberID does not exist in the session,
        // then there is no authenticated member.

        if (memberID == null)
        {
            // Sends the user to the Login page.
            return RedirectToAction(
                "Login",
                "Account");
        }

        // If user is logged in, sends them directly to the Dashboard.
        return RedirectToAction(
            "Dashboard",
            "Home");
    }

    // Displays the main Dashboard for the currently
    // logged-in member.
    public IActionResult Dashboard()
    {
        // Retrieves MemberID from the current session.
        var memberID =
            HttpContext.Session.GetInt32("MemberID");

        // Retrieves the member's full name from the session.
        var memberName =
            HttpContext.Session.GetString("MemberName");


  
        // Requires both MemberID and MemberName.
        // This prevents a logged-out user from manually
        // navigating to: Home/Dashboard

        if (memberID == null ||
            string.IsNullOrEmpty(memberName))
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        // The Dashboard.cshtml displays  Welcome, Member Name :
        ViewBag.MemberName = memberName;


        // Displays: // Views/Home/Dashboard.cshtml
        return View();
    }



    // Displays the default application Privacy page.

    public IActionResult Privacy()
    {
        return View();
    }

    // Displays application error information when an
    // unhandled error occurs.

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
    }
}