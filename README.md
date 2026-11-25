# Chicago Listing Service (CLS)

## CIS 260 Final Project

**Author:** Chuck Smith  
**Course:** CIS 260  
**Project:** Chicago Listing Service (CLS)

## Project Overview

Chicago Listing Service (CLS) is a web-based property listing application created as my CIS 260 final project.

The project is designed as a simplified Multiple Listing Service (MLS) that allows authenticated real estate members to Create, edit, delete and search open listings.

The Chicago Listing Service was built to combine several concepts covered throughout the Cyber Security program, including web development, AWS Cloud computing, application security, and troubleshooting.

The finished application runs as an ASP.NET Core MVC application and uses an Amazon RDS MySQL database for data storage.

## Project Goals

The primary goals of the project were to:

- Build a functional database-driven web application.
- Create a relational database for members, property listings, and property photos.
- Authenticate members before allowing access to the application.
- Allow members to create and manage their own property listings.
- Allow all authenticated members to browse CLS property listings.
- Prevent members from editing or deleting listings owned by another member.
- Add property photo management.
- Store application data in a cloud-hosted MySQL database.
- Apply concepts learned throughout the CIS program to a larger end-to-end project.

## Main Features

### Member Authentication

CLS includes a member login system that uses the member's CLS credentials that are stored in MYSQL.

After a successful login, CLS stores information about the authenticated member in the ASP.NET Core session, including:

- MemberID
- MemberNumber
- MemberName

The session information allows CLS to identify the member while the member navigates through the dashboard.

The dashboard also has a logout function that clears the member's session.

### Dashboard

After signing in, members are taken to the CLS dashboard.

The dashboard provides navigation to the main parts of the system, including:

- Property Listings
- My Listings
- Logout


### Property Listings

The Property Listings page displays all property listings currently stored in CLS regardless of member.

Listings are displayed as property cards that include information such as:

- Property photo
- Street address
- City, state, and ZIP code
- List price
- Bedrooms
- Bathrooms
- Square footage
- Property type
- Property status
- CLS number
- Listing member

Members can open a listing to view its complete details including whether the listing is active, closed, or contingent.

### My Listings

The My Listings page displays only properties associated with the logged-in member.

The My Listings page provides different options for the member's listings, including:

- View Listings
- Edit Listings
- Delete Listings
- Add New Listings

The session is filtered by comparing the MemberID stored in the member's session with the MemberID associated with each listing.

### Create Listing

Authenticated members can create new property listings via a create listing form.

Members complete the form by entering the following:

- Property type
- Property status
- Street address
- City
- State
- ZIP code
- List price
- Bedrooms
- Bathrooms
- Square footage
- Year built
- HOA fees
- Property description

Some information is controlled automatically by CLS rather than entered by the member.

CLS automatically assigns:

- MemberID (which is gotten from MYSQL)
- CLSNumber (Which uniquely identifies the property listing and is assigned when a new listing is created..)
- DateListed (Assigned when a listing is created)


### Edit Listing

Members can update information for listings they own via the edit listings option.

Editable information includes:

- Property type
- Property status
- Address
- List price
- Bedrooms
- Bathrooms
- Square footage
- Year built
- HOA fees
- Listing description

The system controlled data values such as MemberID, CLSNumber, and Date Listed are non-editable by members.

### Delete Listing

Members can delete listings that they own.

A confirmation page is shown before a member can delete a listing.

Before deletion, the controller verifies that the ListingID and MemberID both match the currently logged-in member preventing members from deleting a listing they dont own.

When a listing containing photos is deleted, CLS also removes the related database records and other assoicated files.

### Property Photos

Owners of listings can upload photoes of a unit using the add photos option.

Supported image formats they can use are:

- JPG
- JPEG
- PNG
- WEBP

Uploaded images are assigned unique filenames using a GUID to reduce the possibility of duplicate filenames.

Physical image files are stored under:

```
wwwroot/images/listings (this location is on the ec2 instance)
```

The MySQL database stores information about the image, including its URL, caption, upload date, and associated ListingID but not the actual photo.

Members can also delete individual photos from listings they own.

### Listing Ownership Protection

One of the main security features implemented in this project is the ownership-based authorization.

A member is not allowed to edit, delete, or add photos to another member's listing.

For protected operations, CLS verifies both:

ListingID + Logged-in MemberID


For example: Member 1 owns 2 listings. They can edit, delete, and add photes to those 2 listings

Member 1 can view Member 2's listing via the property listings photo but, they can't edit, delete or add photos to the listinfs



These checks are performed in ListingsController rather than relying only on hiding buttons in the user interface.

## Technology Stack

| Component | Technology |
|---|---|
| Application Framework | ASP.NET Core MVC |
| Programming Language | C# |
| Front End | Razor Views |
| UI Framework | Bootstrap |
| ORM | Entity Framework Core |
| Database | MySQL |
| Cloud Database | Amazon RDS for MySQL |
| Application Hosting | Amazon EC2 |
| IDE | Visual Studio 2022 |
| Repository | GitHub |
| Session Management | ASP.NET Core Session | 

## Application Architecture

CLS uses a multi-tier application structure.


ASP.NET Core MVC separates the application into Models, Views, and Controllers.

**Models** represent the application's data.

**Views** provide the user interface using Razor views and HTML.

**Controllers** process requests, controls application logic, interact with the database, and determine which views should be returned.

Entity Framework Core provides the connection between the C# application and the MySQL database.

## Database Structure

The primary CLS data model contains three main tables:

Members &gt; Listings &gt; Photos

### Members

The Members table contains information associated with CLS members inclusing their MemberNumber which functions as their username.

A member can own as many listings as they require.

Important fields that distinguish a member are:

- MemberNumber
- FirstName
- LastName
- Password (hashed)
- BillType
- MemberStatus

### Listings

The Listings table contains property information.

Each listing belongs to a member through their MemberID.

Important listing fields include:

- ListingID
- MemberID
- CLSNumber
- PropertyType
- PropertyStatus
- StreetAddress
- City
- State
- ZipCode
- ListPrice
- Bedrooms
- Bathrooms
- SquareFeet
- YearBuilt
- HOAFees
- ListingDescription
- DateListed

### Photos

The Photos table stores information about property images.

Each photo is connected to a listing through its ListingID.

Important photo fields include:

- PhotoID
- ListingID
- PhotoURL
- Caption
- UploadDate


```

## Project Structure

The CLSWeb project is organized approximately as follows:

```text
CLSWeb/
|
|-- Controllers/
|   |-- AccountController.cs
|   |-- HomeController.cs
|   |-- ListingsController.cs
|
|-- Models/
|   |-- CLSDBContext.cs
|   |-- ErrorViewModel.cs
|   |-- Listing.cs
|   |-- Member.cs
|   |-- Photo.cs
|
|-- ViewModels/
|   |-- LoginViewModel.cs
|
|-- Views/
|   |-- Account/
|   |   |-- Login.cshtml
|   |
|   |-- Home/
|   |   |-- Dashboard.cshtml
|   |   |-- Index.cshtml
|   |   |-- Privacy.cshtml
|   |
|   |-- Listings/
|   |   |-- AddPhoto.cshtml
|   |   |-- Create.cshtml
|   |   |-- Delete.cshtml
|   |   |-- Details.cshtml
|   |   |-- Edit.cshtml
|   |   |-- Index.cshtml
|   |   |-- MyListings.cshtml
|   |
|   |-- Shared/
|
|-- wwwroot/
|   |-- css/
|   |-- images/
|   |   |-- listings/
|   |-- js/
|   |-- lib/
|
|-- appsettings.json
|-- Program.cs
|-- CLSWeb.csproj
|-- .gitignore
|-- README.md
```

## Security

Security was an important consideration during development of CLS.

### Password Protection

Member's passwords are stored as password hashes and not plain text. During authentication, the portal validates the password entered
by the member against the stored hash in MYSQL

The intended way for member data to get into MYSQL is through an API which was not completed in this project

Authentication uses stored password values to validate the member attempting to access CLS.

### Session-Based Authentication

After successful authentication, CLS stores member information in the ASP.NET Core session.

Protected operations verify that a valid MemberID exists in the user's session.

### Listing Authorization

Sensitive listing operations verify that the listing belongs to the logged-in member.

This includes:

- Editing listings
- Deleting listings
- Adding photos
- Deleting photos

Authorization is performed in the controller so manually entering another listing's URL does not bypass the ownership verification.

### Anti-Forgery Protection

POST requests that change application data use ASP.NET Core anti-forgery protection.


### Database Credentials

Database credentials are not stored in the GitHub repository.

During development, the database connection string is stored using ASP.NET Core User Secrets.

The application expects the following configuration key:

ConnectionStrings:CLSDatabase

The public `appsettings.json` file does not contain the database password.

## Development Setup

### Requirements

To work with the CLS project, the following are required:

- Visual Studio 2022
- .NET SDK compatible with the project
- MySQL database
- Git
- Access to the required database configuration

The original project uses an Amazon RDS MySQL database.

## Quick Start

### 1. Clone the Repository

powershell
git clone <a href="https://github.com/ThisizChuck9/CLSWeb.git" target="_blank" rel="noopener noreferrer" title="https://github.com/ThisizChuck9/CLSWeb.git" class="fai-ChatInputEntity__text ___6erqso0 fyind8e f1tx3yz7 f1deo86v f1eh06m1 f1iescvh">https://github.com/ThisizChuck9/CLSWeb.git</a>


Move into the project directory:

powershell
cd CLSWeb


### 2. Configure the Database Connection

The database password and connection string are intentionally excluded from the repository.

Initialize ASP.NET Core User Secrets if to access if needed:

powershell
dotnet user-secrets init


Set the CLS database connection string:

powershell
dotnet user-secrets set "ConnectionStrings:CLSDatabase" "YOUR_CONNECTION_STRING"


A MySQL connection string will generally contain the following information:

Server=SERVER;
Port=3306;
Database=DATABASE;
User=USERNAME;
Password=PASSWORD;


Do not commit real database credentials to the repository.

### 3. Restore Dependencies

powershell
dotnet restore


### 4. Build the Application

powershell
dotnet build


A successful build should finish without compilation errors.

### 5. Run CLS

powershell
dotnet run


Open the local address displayed by ASP.NET Core in a web browser.

## Testing

Testing was performed throughout development instead of waiting until completion of the web app.

### Authentication Testing

Testing included:

- Successful login
- Invalid login attempts
- Session creation
- Logout
- Access after logout

### Listing Testing

Testing included:

- Creating a listing
- Viewing a listing
- Editing a listing
- Deleting a listing
- Changing property status
- Viewing all property listings
- Viewing only listings belonging to the logged-in member

### Multi-Member Authorization Testing

Multiple member accounts were used to test ownership restrictions.

A second member account was used to verify that one member could not manage another member's listing.

Testing verified that:

- My Listings displays only the current member's listings.
- A member cannot edit another member's listing.
- A member cannot delete another member's listing.
- A member cannot add a photo to another member's listing.
- A member cannot delete a photo belonging to another member's listing.

Controller-side ownership checks protect these operations even when a user attempts to manually access a protected URL.

### Photo Testing

Testing included:

- Uploading property photos
- Displaying uploaded photos
- Entering photo captions
- Deleting photos
- Rejecting unsupported image extensions
- Preventing unauthorized photo management
- Removing associated photos during listing deletion

### Navigation Testing

Navigation between Property Listings, My Listings, and Listing Details was also tested.

The Listing Details page remembers whether it was opened from:

### SQL Injection attacks

The CLS login interface was tested using multiple SQL injection attempts to detrmine whether a malicious actor could bypass authnetication
and query the the underlying SQL tables. The tests did not succesfully bypass the authentication process and coulndt query the database

## Troubleshooting

### Application Cannot Connect to MySQL

Confirm that:

- The MySQL server is available.
- The database exists.
- The connection string uses the correct server.
- Port 3306 is available when required.
- The database username is correct.
- The password is correct.
- The Amazon RDS security configuration allows the required connection.

Also confirm the User Secret exists:

powershell
dotnet user-secrets list


Be careful when displaying the output because the connection string contains sensitive information.

### User Secrets Are Not Configured

Initialize User Secrets:

powershell
dotnet user-secrets init


Then configure the connection:

powershell
dotnet user-secrets set "ConnectionStrings:CLSDatabase" "YOUR_CONNECTION_STRING"


### Images Do Not Display

Verify that uploaded images exist under:


wwwroot/images/listings


Also verify that the PhotoURL stored for the photo uses the expected path:


/images/listings/filename.jpg


### Member Cannot Edit a Listing

CLS intentionally allows only the owner of a listing to edit it.

The MemberID stored in the current session must match the MemberID assigned to the listing.

### Member Receives a 404 When Editing Another Member's Listing

This is expected behavior.

Protected controller actions retrieve a listing using both its ListingID and the logged-in MemberID.

If the combination does not match, the controller does not return the listing.

## Development Challenges

Several parts of CLS required troubleshooting and redesign during development.

Ran out of time to implement API Calls

Intended to have a third msearch listings feature that would allow a member to search for a listing

### Cloud Architecture

The project architecture changed during development because of cloud resource and cost constraints.

The final project uses MySQL with Amazon RDS and an ASP.NET Core application environment on Amazon EC2.

### Database Integration

Configuring Entity Framework Core to work correctly with the MySQL database required:

- Entity models
- Database relationships
- Connection configuration
- Navigation properties
- Database queries

### Authentication and Sessions

CLS needed to maintain the identity of the authenticated member across multiple requests.

ASP.NET Core Session was used to retain the member information needed by the application.

### Ownership and Authorization

Displaying an Edit or Delete button only to an owner is not sufficient security.

The controller also needed to confirm ownership whenever protected operations were requested.

This became an important part of the project's security design.

### Photo Management

Photo functionality required coordination between two types of storage:

Physical Image

wwwroot/images/listings

Database Record

Photos table


Deleting photos and listings required keeping both locations synchronized.

## Known Limitations

CLS was developed as a course project and is not intended to function as a production MLS.

Current limitations include:

- Limited number of test members and property listings
- No public member registration
- No password reset interface
- No administrative management portal
- Limited property search and filtering
- No interactive mapping
- No automated external AMS synchronization via API
- Photo storage is handled directly by the application server instead of an S3 bucket
- No formal penetration testing
- Limited reporting and analytics



**Chuck Smith**  
CIS 260 Final Project  
Chicago Listing Service (CLS)
