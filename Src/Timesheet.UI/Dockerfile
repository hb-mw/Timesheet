# ==========================
# 1. Build the Blazor WASM app
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only project files first for efficient caching
COPY Src/Timesheet.UI/Timesheet.UI.csproj Src/Timesheet.UI/
COPY Src/Timesheet.Shared/Timesheet.Shared.csproj Src/Timesheet.Shared/
RUN dotnet restore Src/Timesheet.UI/Timesheet.UI.csproj

# copy everything and build
COPY . .
WORKDIR /src/Src/Timesheet.UI
RUN dotnet publish -c Release -o /app/dist

# ==========================
# 2. Use nginx to serve the Blazor files
# ==========================
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# remove the default nginx static files
RUN rm -rf ./*

# copy published Blazor files
COPY --from=build /app/dist/wwwroot .
COPY Src/Timesheet.UI/nginx.conf /etc/nginx/conf.d/default.conf
# optional: expose nginx port
EXPOSE 80

# start nginx
CMD ["nginx", "-g", "daemon off;"]
