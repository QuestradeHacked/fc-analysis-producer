FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
EXPOSE 80
EXPOSE 443

# Copy project resources
COPY src src
COPY nuget.config ./
COPY .editorconfig ./
COPY Questrade.FinCrime.Analysis.Producer.sln Questrade.FinCrime.Analysis.Producer.sln
RUN dotnet restore --locked-mode ./src/Questrade.FinCrime.Analysis.Producer/Questrade.FinCrime.Analysis.Producer.csproj --configfile nuget.config

# Test steps
FROM build as test
RUN dotnet restore --locked-mode src/Questrade.FinCrime.Analysis.Producer.Tests.Unit/Questrade.FinCrime.Analysis.Producer.Tests.Unit.csproj --configfile nuget.config
RUN dotnet restore --locked-mode src/Questrade.FinCrime.Analysis.Producer.Tests.Integration/Questrade.FinCrime.Analysis.Producer.Tests.Integration.csproj --configfile nuget.config
ENTRYPOINT ["dotnet", "test" ]

# Publishing the application
FROM build AS publish
RUN dotnet publish src/Questrade.FinCrime.Analysis.Producer/Questrade.FinCrime.Analysis.Producer.csproj -c Release -o /app/Questrade.FinCrime.Analysis.Producer --no-restore

# Final image wrap-up
FROM gcr.io/qt-shared-services-3w/dotnet:6.0 as runtime
WORKDIR /app
COPY --from=publish /app/Questrade.FinCrime.Analysis.Producer .
USER dotnet
CMD [ "dotnet", "Questrade.FinCrime.Analysis.Producer.dll" ]
