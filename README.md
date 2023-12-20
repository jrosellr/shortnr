# Shortnr - Simple URL Shortener
Shortnr is a simple yet complete URL shortener.

In this full stack project I am using the following technologies:

  - .NET 8 minimal APIs with AOT compilation
  - Angular 17
  - Docker and Docker Compose for local dev environments
  - DynamoDB
  - Redis / AWS MemoryDB
  - CI/CD with GitHub Actions
  - DevSecOps with GitHub Actions covering SAST, DAST and SCA

## Scope
The scope of this project is to build a little but feature-complete URL shortener using modern technology.
The needs the URL shortener must cover are the following:

  - Allow any user to shorten any URL into a shortened URL
  - Redirect any user when they click a shortened URL

That's it, the core capabilities of a URL shortener are those. URL analytics and such will not be covered for now.

Furthermore, as I want to learn and apply some DevSecOps concepts and AWS tech, I will integrate the project with GitHub Actions and some AWS services.
I'd like to deploy it to AWS but I'm leaving that as a next step, so for now I'll provision those services as Docker containers.

This project will be considered complete when the following requirements are met:

  - [ ] The project runs locally using Docker Compose **only**
  - [ ] The project supports the hot reloading of the Docker images while running
  - [ ] The project supports both Windows and Linux dev environments
  - [ ] All automated tests pass (Cypress, Postman, unit tests, etc.)
  - [ ] UAT is done
  - [ ] Automated code quality reports are all green
  - [ ] Automated security reports are all green

## Next Steps
The next steps of the project are:

  - Deploy the project to AWS using CI/CD
  - Provision the infrastructure using IaC
  - Implement URL analytics (click counting)
  - Implement user accounts for URL tracking / editing


