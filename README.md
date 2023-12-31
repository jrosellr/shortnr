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

  - [x] The project runs locally using Docker Compose **only**
  - [ ] ~~The project supports the hot reloading of the Docker images while running~~
    - Finally I removed this feature as it is not worth pursuing. I got it working but I do not find it that useful.
  - [x] The project supports both Windows and Linux dev environments
  - [ ] All automated tests pass (Cypress, Postman, unit tests, etc.)
  - [ ] UAT is done
  - [ ] Automated code quality reports are all green
  - [ ] Automated security reports are all green

## Conclusions
### Docker as a development environment
After two days spent fighting against npm and MSBuild I think I formed an opinion around using Docker as a general purpose dev environment.

The whole point about trying this was to do development work without needing any kind of local SDK or runtime in order to ease the onboarding of new talent into the project and to simplify the workflow. Naivety aside, do you see the problem with my approach?

The thing is, being able to do development work without having any hard dependency installed is wishful thinking at best and a horrid overengineered mess at worst.

I do see the value of having everything as a reproducible Docker image, don't get me wrong here, but I find that it's ill suited for doing actual *development work*. Even with hot-reload enabled everywhere I still feel that something is not right.

The following questions arise:
  - How are we supposed to run the unit tests? Inside a container too?
    - We need a runtime to edit and run the tests in a reasonable time
  - What if we need to debug? Do we attach to the container and spin up a debugger supporting DAP?
    - Again, we need a runtime to debug the program unless our devs are familiar with tools like GDB, which would be rare
  - Are remote development technologies good enough to support a typical dev workflow?
    - I have not tried them extensively, but for what I've seen so far, they are limited.

My conclusion is this:
  - What is the point in working *inside* a container when we are going to need the dependencies in our machines anyway?

Sorry for the rambling, but I think that our focus from now on should be less about Dockerizing everything and more about improving the tooling and easing the distribution of runtimes/dependencies to our devs. We should use Docker containers to provision local infrastructure and to deploy our services, of course, but we should not use Docker containers as a development environment themselves (at least not for now!).

All in all, I'd say that this little experiment was fun and I consolidated a lot of sparse knowledge I had lying around about Docker Compose, Dockerfiles, layer caching, etc.

## Next Steps
The next steps of the project are:

  - Deploy the project to AWS using CI/CD
  - Provision the infrastructure using IaC
  - Implement URL analytics (click counting)
  - Implement user accounts for URL tracking / editing


