# NoteWebApp Project

**NoteWebApp** is a web application designed for managing and sharing notes. Users can easily create, update, delete, and share their notes, with the option to mark notes as public for broader sharing.

## Features

- **User Authentication:** Easily register, log in, and log out with secure user authentication.
- **Password Reset:** Forgot your password? Utilize the password reset functionality to regain access.
- **Create Note:** Users can create new notes, providing a title, description, and optional image.
- **Update Note:** Modify the title, description, or image of existing notes.
- **Delete Note:** Soft-delete notes, marking them as deleted.
- **Share Note:** Make a note public and generate a shareable link for collaborative use.
- **Public View:** Explore public notes and their details without requiring authentication.

## Project Structure

### Controllers

- **NoteController:** Manages actions related to notes, such as creating, updating, and deleting notes.
- **HomeController:** Handles the main page for authenticated users, displaying their notes.
- **AccountController:** Manages user authentication, registration, login, logout, and password-related actions.
- **ShareNoteController:** Allows public view of notes and handles note-sharing actions.

### Models

- **Note:** Represents a note with properties like Id, Title, Description, ImageUrl, DateCreated, IsDeleted, AppUserId, AppUser, and IsPublic.
- **AppUser:** Extends IdentityUser and includes a Name property and a collection of associated notes.

### Views

- **Home/Index:** Displays the authenticated user's notes.
- **Note/CreateNewNote:** Form for creating a new note.
- **Note/UpdateNote:** Form for updating an existing note.
- **Note/GetAllNotes:** Lists all notes for the authenticated user.
- **Account/Login:** Login form.
- **Account/Register:** Registration form.
- **Account/ForgotPassword:** Form for initiating a password reset.
- **Account/ResetPassword:** Form for resetting the password after receiving a reset link.
- **ShareNote/PublicNotes:** Lists public notes for all users.
- **ShareNote/PublicView:** Displays details of a public note.

### Data

- **AppDbContext:** Entity Framework DbContext for interacting with the database.

### Services

- **FileService:** Handles file-related operations, such as saving images to the server.

## How to Run the Project

1. Clone this repository.
2. Open the project in Visual Studio or your preferred IDE.
3. Set up the database connection in `appsettings.json` by modifying the `DefaultConnection` under `ConnectionStrings`.
4. Run the following commands in the terminal:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update4. Run the application.
   ```
   or
   ```bash
    add-migration InitialCreate
    update-database
   ```
 4. Run the application.

Please make sure to replace `YourDatabaseName` with the name you want to give to your database.

Enjoy managing and sharing your notes with NoteWebApp 😊
