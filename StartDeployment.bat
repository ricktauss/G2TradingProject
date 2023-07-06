REM ###Deklaration von Variablen###
set "base_path=%~dp0bin\Debug\net7.0\"

REM ###Deklaration von Environment Variablen die in Services verwendet werden
set "CONSUL_SERVER_URL=http://localhost:8500"


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

