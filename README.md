# Introduction
BetSnooker is a web application for betting in snooker events.

# Dependency
BetSnooker relies on API provided by http://api.snooker.org/

# Deployment
Create Resource Group called 'BetSnookerResourceGroup' on Azure portal.\
Choose the location that best fits, i.e. Germany West Central or West Europe.

## Backend service
Publish from Visual Studio.

1. Check appsettings.json file and make sure all settings are correct.
2. Create new publish profile:
   - Select App Service -> Create New
     - Name: BetSnookerApi
     - Subscription: your subscription
     - Resource group: created 'BetSnookerResourceGroup'
     - Hosting Plan - create one:
       - Hosting Plan: BetSnookerApiPlan
       - Location: same as resource group
       - Size: B1 or S1
       - Application Insights: not None
   - Hit Create button
3. Click Publish button next to created profile (leave the default configuration)
4. After successful publish
   - Check following URL: https://betsnookerapi.azurewebsites.net/swagger
   - Go to Configuration -> General settings and set 'Always on' to On -> Save

## Website
Publish from Visual Studio Code with Azure extension.

1. Check environment.prod.ts file and make sure apiUrl is correct.
2. Go to Web.UI folder and build angular app: ng build --prod
   - output will be created in Web.UI/dist/angular directory
3. Create a Web App in Azure portal
   - Create a resource -> Web App
   - Subscription: same as for BetSnookerApi
   - Resource Group: created 'BetSnookerResourceGroup'
   - Name: BetSnooker
   - Publish: Code
   - Runtime stack: .NET Core
   - Operating System: Windows
   - Region: same as resource group
   - App Service Plan - make sure you create a new one
     - Name: BetSnookerPlan
     - Sku and size: F1 (Free)
   - Enable Application Insights
4. Once Web App resource is created, go to Azure extension in VS Code
   - Find the created App Service Plan under proper subscription
   - Right click -> Deploy to Web App...
   - Browse to the directory from 2. point
   - Deploy
5. Go to: https://betsnooker.azurewebsites.net/