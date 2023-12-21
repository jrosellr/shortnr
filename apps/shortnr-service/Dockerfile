# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/nightly/sdk:8.0-alpine-aot AS build
ARG TARGETARCH
WORKDIR /build

# copy csproj and restore as distinct layers
COPY **/*.csproj .
COPY *.props .
COPY *.config .
RUN dotnet restore -r "linux-musl-$TARGETARCH"

# copy and publish app and libraries
COPY **/* .
RUN dotnet publish -c "Debug" --no-restore -o "/app"
# RUN rm /app/*.dbg /app/*.Development.json


# final stage/image
FROM mcr.microsoft.com/dotnet/nightly/runtime-deps:8.0-alpine-aot

WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./shortnr.WebApi"]

