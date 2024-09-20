$envFilePath = "..\.env"  # Adjust the path as necessary
$envVars = Get-Content $envFilePath | ForEach-Object {
    $key, $value = $_ -split '='
    [System.Environment]::SetEnvironmentVariable($key.Trim(), $value.Trim())
}

# Configuration Variables
$ContainerName = "sqlserver-container"
$DBUserName = [System.Environment]::GetEnvironmentVariable("DB_USERNAME")
$DBPassword = [System.Environment]::GetEnvironmentVariable("DB_PASSWORD")  # Read password from .env file
$DBName = [System.Environment]::GetEnvironmentVariable("DB_NAME") # Read DB_Name from .env file
$DBPort = [System.Environment]::GetEnvironmentVariable("DB_PORT") # Read DB_Port from .env file
$BackupPath = "/var/opt/mssql/backup"
$LocalBackupFile = ".\${DBName}.bak"
$LogicalDataFile = $DBName   # Logical data file name
$LogicalLogFile = "${DBName}_log" # Logical log file name


# Pull the SQL Server Docker image
# Write-Host "Pulling SQL Server Docker image..."
# docker pull mcr.microsoft.com/mssql/server

# Start the SQL Server container (if not already running)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$DBPassword" `
   -p 1433:1433 --name $ContainerName `
   -d mcr.microsoft.com/mssql/server


Write-Host "Backup path is $LocalBackupFile"
# Wait for SQL Server to start (sleep for 20 seconds)
Write-Host "Waiting for SQL Server to start..."
Start-Sleep -Seconds 20
docker exec -it $ContainerName mkdir -p $BackupPath

# Copy the backup file from the local machine to the Docker container
Write-Host "Copying the backup file to the container..."
docker cp $LocalBackupFile "$($ContainerName):$($BackupPath)/"
# Check if the container is running
if (docker ps -q -f name=$ContainerName) {
  Write-Host "SQL Server container is running."
} else {
  Write-Host "SQL Server container failed to start."
  exit 1
}

# (Optional) Get the logical names of the files in the .bak file
Write-Host "Retrieving logical file names from the backup file..."
docker exec -it $ContainerName /opt/mssql-tools18/bin/sqlcmd `
   -S localhost -U $DBUserName -P $DBPassword `
   -N -C `
   -Q "RESTORE FILELISTONLY FROM DISK = N'$BackupPath/TranslationPM.bak'"

# Perform the restore operation
Write-Host "Restoring the database..."
docker exec -it $ContainerName /opt/mssql-tools18/bin/sqlcmd `
   -S localhost -U $DBUserName -P $DBPassword `
   -N -C `
   -Q "RESTORE DATABASE [$DBName] FROM DISK = N'$BackupPath/$DBName.bak' `
   WITH MOVE '$LogicalDataFile' TO '/var/opt/mssql/data/$DBName.mdf', `
   MOVE '$LogicalLogFile' TO '/var/opt/mssql/data/$DBName_log.ldf'"

# Final message
Write-Host "Database [$DBName] has been successfully restored."

