REM ###Deklaration von Variablen###
set "base_path=%~dp0bin\Debug\net7.0\"

REM ###Deklaration von Environment Variablen die in Services verwendet werden
set "CONSUL_SERVER_URL=http://localhost:8500"
set "ConsulKeyValueService_URL=https://localhost:5064"
set "SupplierService_URL=https://localhost:5050"
set "ProductService_URL=https://localhost:5030"
set "ProductServiceFTP_URL=https://localhost:5031"
set "LocalDatastore_URL=https://localhost:5070"
set "ASPNETCORE_ENVIRONMENT=Development"


REM ###Starten von ConsulServiceDiscovery CreditCardService-Sidecar###
set "CREDITCARD_SERVICE_DISCOVERY_URL=https://localhost:5029"
start "" cmd /k dotnet "%base_path%ConsulServiceDiscovery.dll" --urls="%CREDITCARD_SERVICE_DISCOVERY_URL%"

REM ###Starten von CreditcardService Instanzen###
start "" cmd /k dotnet "%base_path%CreditcardService.dll" --urls="https://localhost:5020;http://localhost:5025"
start "" cmd /k dotnet "%base_path%CreditcardService.dll" --urls="https://localhost:5021;http://localhost:5026"

REM ###Starten zentraler LoggingService
start "" cmd /k dotnet "%base_path%LoggerService.dll" --urls="https://localhost:5010;http://localhost:5015"

REM ###Starten ProductService
start "" cmd /k dotnet "%base_path%ProductService.dll" --urls="https://localhost:5030;http://localhost:5035"

REM ###Starten ProductServiceFTP
start "" cmd /k dotnet "%base_path%ProductServiceFTP.dll" --urls="https://localhost:5031;http://localhost:5036"

REM ###Starten MeiShopService
start "" cmd /k dotnet "%base_path%MeiShop.dll" --urls="https://localhost:5040;http://localhost:5045"

REM ###Starten SupplierService
start "" cmd /k dotnet "%base_path%SupplierService.dll" --urls="https://localhost:5050;http://localhost:5055"

REM ###Starten ConsulKeyValueService
start "" cmd /k dotnet "%base_path%ConsulKeyValueService.dll" --urls="https://localhost:5064;http://localhost:5065"

REM ###Starten LocalDatastore
start "" cmd /k dotnet "%base_path%LocalDatastore.dll" --urls="https://localhost:5070;http://localhost:5075"
