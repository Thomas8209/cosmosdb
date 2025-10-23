# CosmosDb Supportportal

En lille Blazor Server-app til at oprette og se supporthenvendelser via Cosmos DB. 

## Funktioner

- Formular med validering, der gemmer direkte i Cosmos DB.
- Listevisning af alle henvendelser.
- Menu med hurtig adgang til forside, opret og oversigt.

## Kom i gang

1. Hent pakker og byg:
   bash terminal
   cd CosmosDb
   dotnet restore
   dotnet build
   ```
2. Læg Cosmos-info i user secrets:
   bash terminal
   dotnet user-secrets set "CosmosDb:ConnectionString" "<din-connection-string>"
   dotnet user-secrets set "CosmosDb:DatabaseName" "SupportDb"
   dotnet user-secrets set "CosmosDb:ContainerName" "SupportHenvendelser"
   ```
3. Start appen:
   bash terminal
   dotnet run
   
4. Åbn linket fra konsollen, opret en henvendelse eller se allerede oprettet henvendelser.

## Cosmos DB via `az`

bash terminal
az login
az account set --subscription "<subscription-navn-eller-id>"

az group create \
  --name support-rg \
  --location westeurope

az cosmosdb create \
  --name thomascosmosdb \
  --resource-group support-rg \
  --locations regionName=westeurope failoverPriority=0 isZoneRedundant=false \
  --kind GlobalDocumentDB

az cosmosdb sql database create \
  --account-name thomascosmosdb \
  --name SupportDb \
  --resource-group support-rg

az cosmosdb sql container create \
  --account-name thomascosmosdb \
  --database-name SupportDb \
  --name SupportHenvendelser \
  --partition-key-path "/id"

az cosmosdb keys list \
  --type connection-strings \
  --name thomascosmosdb \
  --resource-group support-rg
```
Jeg oprettede dog ikke selv en resource group osv. via terminallen, jeg klarede det via. azure's hjemmeside. Begge dele virker, det kommer lidt an på smag og behag hvad man vælger, men en rutineret ville nok gøre det i terminallen, da kommnadoerne nok næsten ligger i fingerne.

## Status
- I systemet kan man oprette supporthenvendelser + der er opsat minimalitisk validering på input, primært via indeni modelklassen supportHenvendelse. På en anden side kan man se de oprettede henvendelser som ligger gemt i cloud (cosmosDB), man kunne have lavet listen mere deltajleret, så al data blev vist. Fortrolige oplysninger/data som fx ConnectionString er gemt i user-secrets, så det fortrolige ikke er til at finde i mit projekt når jeg uploader det til fx github. Der er også andre måde at gøre det på via diverse azure services, men jeg valgte usersecrets metode 
Fejl/mangler: som systemet er nu burde alt spille ift. de stillede krav i opgaven.

Næste trin:
Det næste man burde udvikle i dette her system ville helt klar være en form for sorting af supporthenvendelserne, sådan at man som supporter nemt kan navigere rundt i tonsvis af supporthenvendsler. Det kunne man gøre ved fx at lave kategoriseret lister al efter henvendelsen er teknisk spørgsmål eller forslag til ændringer.


