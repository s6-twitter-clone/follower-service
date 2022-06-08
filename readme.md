# follower service
The follower service is in charge of managing followers for users.
The follower service owns the follower data.

## build instructions
There are two main ways to build and deploy the application.
For local development docker compose is used to run the containers for the service and the database.
For production kubernetes is used. Below there are instructions for both environments.

### Local development
To build and run the project locally you can run `docker compose up`.
This will build the API and run all the services necessary for it to function properly.

### Production
To get the project running in kubernetes there are a couple of steps:
1. build the image for the backend by running 

```bash
$ docker build -f follower-service/Dockerfile -t <HOST>/follower-service:<TAG>
```

2. Push the image to the docker registry by running 

```bash
$ docker push <HOST>/follower-service:<TAG>
```