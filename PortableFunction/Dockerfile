FROM fnproject/dotnet:8.0-1.0.46-dev as build-stage
WORKDIR /function
COPY . .
RUN dotnet sln add OCIFunction/OCIFunction.csproj CommonFunction/Common.Support.csproj
RUN dotnet build -c Release
RUN dotnet test -c Release
RUN dotnet publish OCIFunction/OCIFunction.csproj -c Release -o out
FROM fnproject/dotnet:8.0-1.0.46
WORKDIR /function
COPY --from=build-stage /function/out/ /function/
ENTRYPOINT ["dotnet", "OCIFunction.dll"]
CMD ["OCIFunction:Function:FunctionHandler"]
