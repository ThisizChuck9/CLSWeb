using Microsoft.AspNetCore.Mvc;
using CLSWeb.Models;
using CLSWeb.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace CLSWeb.Controllers
{
    public class AccountController : Controller
    {
        // Database context used to communicate with
        // MY CLS MySQL database.
        private readonly CLSDBContext _context;



        // CONSTRUCTOR

        // ASP.NET Core provides CLSDBContext through
        // dependency injection.

        public AccountController(CLSDBContext context)
        {
            _context = context;
        }

        // Displays My CLS login page.

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Processes the Member Number and Password submitted
        // on the CLS login page.

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {

            // Search the Members table in MYSQL for the Member Number
            // entered on the login page.

            var member = _context.Members
                .FirstOrDefault(m =>
                    m.MemberNumber == model.MemberNumber);


            // If no member exists with the entered Member Number,
            // do not allow login.
            //
            // The error doesn't' does not reveal whether the
            // Member Number or Password was incorrect.
            // ---------------------------------------------------------
            if (member == null)
            {
                ViewBag.Message =
                    "Invalid Member Number or Password";

                return View();
            }

            // Uses the ASP.NET Core's PasswordHasher for the
            // Member login experience.

            var passwordHasher =
                new PasswordHasher<Member>();

            // Compares the password entered by the member against
            // the hashed password stored in the Members table in MYSQL.

            var result = passwordHasher.VerifyHashedPassword(
                member,
                member.PasswordHash,
                model.Password);



            // Prevent login if password verification fails.

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Message =
                    "Invalid Member Number or Password";

                return View();
            }


            // If the password has been verified successfully,
            // stores information about the member in the
            // ASP.NET Core session.
            // Other CLS pages use these session values to determine
            // which member is currently logged in.

            // Internal database MemberID.
            HttpContext.Session.SetInt32(
                "MemberID",
                member.MemberID);

            // CLS Member Number used to log in.
            HttpContext.Session.SetInt32(
                "MemberNumber",
                member.MemberNumber);

            // Member's full name.
            // Used by pages such as Create Listing and Edit Listing
            // to display the Listing Agent.
            HttpContext.Session.SetString(
                "MemberName",
                $"{member.FirstName} {member.LastName}");


            // Once the member is completely signed in,
            // send the member to the CLS Dashboard to show listings.
            return RedirectToAction(
                "Dashboard",
                "Home");
        }

        // Ends the current member's session.
        //
        // Logging out and clearing the session removes:
        // MemberID, MemberNumber, MemberName
        [HttpGet]
        public IActionResult Logout()
        {
            // Remove all information associated with the
            // current member's session.
            HttpContext.Session.Clear();

            // After logging out, sends the user back to the
            // CLS Login screen.
            return RedirectToAction(
                "Login",
                "Account");
        }
    }
}