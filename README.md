# Contactly

Contactly is an ASP.NET Core MVC application for managing personal contacts through a simple web interface. It includes user accounts, contact management, and basic account settings.

The application is built with C# and .NET 10, using ASP.NET Core MVC, Razor Views, HTML, CSS, and JavaScript. It follows a Clean Architecture structure with separate Presentation, Application, Domain, and Infrastructure layers. Entity Framework Core is used for data access, with MySQL as the database. ASP.NET Core Identity is used for user authentication and account management.

## Live Website

https://contactly.runasp.net

Project Structure

The project is organized into four main layers: Presentation handles the MVC interface, Application contains the application logic, Domain contains the core models and rules, and Infrastructure handles database access and other external services.

## Security

User accounts are protected with ASP.NET Core Identity, and sensitive account actions require password confirmation.

## Data Management

Contactly uses MySQL to store application data and Entity Framework Core to manage communication between the application and the database. Contacts can also be imported and exported for easier data management.
