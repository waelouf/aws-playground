$projectPath = ".\src\WeatherLambdaFunction\src\WeatherLambdaFunction"
$projectPath2 = ".\src\WeatherSqsConsumerLambdaFunction\src\WeatherSqsConsumerLambdaFunction"

$originalPath = Get-Location
$packagePath = $originalPath.Path  + "\lambda\WeatherLambdaFunction.zip"
$packagePath2 = $originalPath.Path  + "\lambda\WeatherLambdaConsumer.zip"
$cdkProjectPath = ".\src\WeatherScheduledNotification"
# Save the current directory to return to it later


# Navigate to the project directory
Set-Location $projectPath

# Build the project
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Exiting." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Package the Lambda function
dotnet lambda package $packagePath
if ($LASTEXITCODE -ne 0) {
    Write-Host "Lambda packaging failed. Exiting." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Return to the original directory
Set-Location $originalPath

Set-Location $projectPath2

# Build the project
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Exiting." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Package the Lambda function
dotnet lambda package $packagePath2
if ($LASTEXITCODE -ne 0) {
    Write-Host "Lambda packaging failed. Exiting." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Return to the original directory
Set-Location $originalPath

Set-Location $cdkProjectPath

dotnet build

Set-Location $originalPath

#cd src

cdk deploy --profile iamadmin-general

#dotnet lambda package src/WeatherLambdaFunction/src/WeatherLambdaFunction.csproj 
# dotnet build src