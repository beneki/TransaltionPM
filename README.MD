## Translation Project Management System

This project is powered by Angular, C#, and .NET, providing a comprehensive solution for translation project management.

### Technologies Used

#### Backend:
- **C#**
- **.NET 8**
- **Entity Framework Core 8.0.8**

#### Database:
- **Microsoft SQL Server** (running in Docker)

#### Frontend:
- **Angular 18**
- **TypeScript**
- **Tailwind CSS**
- **ng2-charts**

### Setup Instructions

Follow these steps to set up the project:

1. Clone the repository:
   ```bash
   git clone https://github.com/beneki/TransaltionPM.git
   ```
2. Rename `example.env` to `.env`.
3. Set your `DB_PASSWORD` in the `.env` file.
4. Install Docker if you haven't already.
5. Open PowerShell and navigate to the `/backups` directory:
   ```bash
   cd /path/to/backups
   ```
6. Run the backup script:
   ```bash
   ./restore-backup-in-docker.ps1
   ```
7. Open the solution file `TranslationProjectManagement.sln` in Visual Studio.
8. Run the .NET server.
9. Navigate to the `web` directory and install dependencies:
   ```bash
   npm install
   ```
10. And then run the Angular project:
   ```bash
   ng serve --open
   ```

### Additional Notes
- Ensure that Docker is running before executing the backup script.
- If you encounter any issues, please refer to the project's documentation or contact support.
