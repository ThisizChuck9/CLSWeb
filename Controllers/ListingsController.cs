using CLSWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLSWeb.Controllers
{
    public class ListingsController : Controller
    {
        // Database context used to communicate with the CLS MySQL database.
        private readonly CLSDBContext _context;

        // ASP.NET Core provides the database context through dependency injection.
        public ListingsController(CLSDBContext context)
        {
            _context = context;
        }

        // Display all property listings stored in CLS.
        public async Task<IActionResult> Index()
        {
            // Load each listing along with its member and photos.
            var listings = await _context.Listings
                .Include(l => l.Member)
                .Include(l => l.Photos)
                .OrderByDescending(l => l.DateListed)
                .ToListAsync();

            return View(listings);
        }

        // Display only listings belonging to the currently logged-in member.
        public async Task<IActionResult> MyListings()
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            // A member must be logged in to access My Listings.
            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Filter the listings using the logged-in MemberID.
            var listings = await _context.Listings
                .Include(l => l.Member)
                .Include(l => l.Photos)
                .Where(l => l.MemberID == memberID.Value)
                .OrderByDescending(l => l.DateListed)
                .ToListAsync();

            return View(listings);
        }

        // Display the blank Create Listing form.
        [HttpGet]
        public IActionResult Create()
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            var memberName =
                HttpContext.Session.GetString("MemberName");

            // A member must be logged in to create a listing.
            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Display the logged-in member as the listing agent.
            ViewBag.ListingAgent = memberName;

            return View();
        }

        // Process a new property listing submitted from the Create form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Listing listing)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // The logged-in member becomes the owner of the listing.
            listing.MemberID = memberID.Value;

            // Record when the listing was created.
            listing.DateListed = DateTime.Now;

            // Find the highest existing CLS number.
            var highestCLSNumber = await _context.Listings
                .MaxAsync(l => (int?)l.CLSNumber);

            // Generate the next CLS number.
            listing.CLSNumber =
                (highestCLSNumber ?? 1000000) + 1;

            // Member is a navigation property and is not submitted by the form.
            ModelState.Remove("Member");

            if (ModelState.IsValid)
            {
                _context.Listings.Add(listing);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyListings));
            }

            // Restore the listing agent when validation fails.
            ViewBag.ListingAgent =
                HttpContext.Session.GetString("MemberName");

            return View(listing);
        }

        // Display the Edit form for a listing owned by the logged-in member.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Both ListingID and MemberID must match.
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l =>
                    l.ListingID == id &&
                    l.MemberID == memberID.Value);

            // Do not expose another member's listing through the Edit URL.
            if (listing == null)
            {
                return NotFound();
            }

            ViewBag.ListingAgent =
                HttpContext.Session.GetString("MemberName");

            return View(listing);
        }

        // Save changes made to an existing listing.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Listing listing)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Make sure the route ID matches the submitted ListingID.
            if (id != listing.ListingID)
            {
                return BadRequest();
            }

            // Retrieve the listing only when it belongs to the logged-in member.
            var existingListing = await _context.Listings
                .FirstOrDefaultAsync(l =>
                    l.ListingID == id &&
                    l.MemberID == memberID.Value);

            if (existingListing == null)
            {
                return NotFound();
            }

            // Member is not submitted by the Edit form.
            ModelState.Remove("Member");

            if (ModelState.IsValid)
            {
                // Update only fields that members are allowed to edit.
                existingListing.PropertyType =
                    listing.PropertyType;

                existingListing.PropertyStatus =
                    listing.PropertyStatus;

                existingListing.StreetAddress =
                    listing.StreetAddress;

                existingListing.City =
                    listing.City;

                existingListing.State =
                    listing.State;

                existingListing.ZipCode =
                    listing.ZipCode;

                existingListing.ListPrice =
                    listing.ListPrice;

                existingListing.Bedrooms =
                    listing.Bedrooms;

                existingListing.Bathrooms =
                    listing.Bathrooms;

                existingListing.SquareFeet =
                    listing.SquareFeet;

                existingListing.YearBuilt =
                    listing.YearBuilt;

                existingListing.HOAFees =
                    listing.HOAFees;

                existingListing.ListingDescription =
                    listing.ListingDescription;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyListings));
            }

            ViewBag.ListingAgent =
                HttpContext.Session.GetString("MemberName");

            // Restore values controlled by CLS rather than the Edit form.
            listing.CLSNumber =
                existingListing.CLSNumber;

            listing.MemberID =
                existingListing.MemberID;

            listing.DateListed =
                existingListing.DateListed;

            return View(listing);
        }

        // Display a confirmation page before deleting a listing.
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Only retrieve the listing when it belongs to the logged-in member.
            var listing = await _context.Listings
                .Include(l => l.Member)
                .Include(l => l.Photos)
                .FirstOrDefaultAsync(l =>
                    l.ListingID == id &&
                    l.MemberID == memberID.Value);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // Permanently delete a listing after confirmation.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check ownership again before deleting anything.
            var listing = await _context.Listings
                .Include(l => l.Photos)
                .FirstOrDefaultAsync(l =>
                    l.ListingID == id &&
                    l.MemberID == memberID.Value);

            if (listing == null)
            {
                return NotFound();
            }

            // Delete physical image files associated with the listing.
            if (listing.Photos != null &&
                listing.Photos.Any())
            {
                foreach (var photo in listing.Photos)
                {
                    var fileName =
                        Path.GetFileName(photo.PhotoURL);

                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        var filePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "listings",
                            fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                // Remove the related photo records from MySQL.
                _context.Photos.RemoveRange(
                    listing.Photos);
            }

            // Remove the listing itself.
            _context.Listings.Remove(listing);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyListings));
        }

        // Display the form for adding a photo to a member's listing.
        [HttpGet]
        public async Task<IActionResult> AddPhoto(int id)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // ListingID and MemberID must both match.
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l =>
                    l.ListingID == id &&
                    l.MemberID == memberID.Value);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // Receive and save a property photo uploaded from AddPhoto.cshtml.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoto(
            int id,
            IFormFile photoFile,
            string? caption)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Verify that the listing belongs to the logged-in member.
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l =>
                    l.ListingID == id &&
                    l.MemberID == memberID.Value);

            if (listing == null)
            {
                return NotFound();
            }

            // Make sure a file was actually selected.
            if (photoFile == null ||
                photoFile.Length == 0)
            {
                ModelState.AddModelError(
                    "",
                    "Please select a photo to upload.");

                return View(listing);
            }

            // Limit uploads to supported image formats.
            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension =
                Path.GetExtension(photoFile.FileName)
                    .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    "",
                    "Only JPG, JPEG, PNG, and WEBP images are allowed.");

                return View(listing);
            }

            // Use a unique filename so uploaded photos do not overwrite each other.
            var fileName =
                $"{Guid.NewGuid()}{extension}";

            // Store images under wwwroot/images/listings.
            var photoFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "listings");

            Directory.CreateDirectory(photoFolder);

            var filePath = Path.Combine(
                photoFolder,
                fileName);

            // Save the physical image to the server.
            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await photoFile.CopyToAsync(stream);
            }

            // MySQL stores the URL to the image instead of the image itself.
            var photo = new Photo
            {
                ListingID = listing.ListingID,

                PhotoURL =
                    $"/images/listings/{fileName}",

                Caption = caption,

                UploadDate = DateTime.Now
            };

            _context.Photos.Add(photo);

            await _context.SaveChangesAsync();

            // Return to the listing after the upload.
            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = listing.ListingID,
                    returnTo = "MyListings"
                });
        }

        // Delete one photo from a listing owned by the logged-in member.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var memberID =
                HttpContext.Session.GetInt32("MemberID");

            if (memberID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the photo only when its listing belongs to this member.
            var photo = await _context.Photos
                .Include(p => p.Listing)
                .FirstOrDefaultAsync(p =>
                    p.PhotoID == id &&
                    p.Listing.MemberID == memberID.Value);

            if (photo == null)
            {
                return NotFound();
            }

            // Save the ListingID so CLS can return to the correct property.
            var listingID =
                photo.ListingID;

            var fileName =
                Path.GetFileName(photo.PhotoURL);

            // Delete the physical image when it still exists.
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "listings",
                    fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // Remove only the photo record. The listing remains in CLS.
            _context.Photos.Remove(photo);

            await _context.SaveChangesAsync();

            // Return to the listing that contained the photo.
            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = listingID,
                    returnTo = "MyListings"
                });
        }

        // Display complete information about a property.
        public async Task<IActionResult> Details(
            int id,
            string? returnTo = null)
        {
            // Load the listing along with its member and photos.
            var listing = await _context.Listings
                .Include(l => l.Member)
                .Include(l => l.Photos)
                .FirstOrDefaultAsync(
                    l => l.ListingID == id);

            if (listing == null)
            {
                return NotFound();
            }

            // Remember whether Details was opened from My Listings
            // or from the main Property Listings page.
            ViewBag.ReturnTo = returnTo;

            return View(listing);
        }
    }
}