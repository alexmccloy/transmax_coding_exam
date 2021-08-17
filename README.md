# About
This is an implementation of the 'EagleRock' backend server for a Practical coding exercise.

# Structure

The Solution contains the following items:

- EagleRock - This the the primary codebase for the EagleRock server. It implements a REST API for EagleBots and an 
Operator to retrieve data. It stores its data in a Redis cache and publishes all received data to RabbitMQ.
- EagleBotSimulator - This is a basic console app that publishes data from a number of fake EagleBots periodically. 
It is used for testing.
- EagleRock.Test - A unit testing project for EagleRock using MSTest.
- RabbitMQ - This is a Dockerfile and startup script to assist in setting up the RabbitMQ container to work out of the
box.


# How to run this code
This solution can be run with docker compose. Use `docker-compose up` to build and run the solution. The following 
containers will be created:
- EagleRock (Exposes port `5000` to the host for REST)
- EagleBot Simulator
- Redis (Exposes port `6397` to the host)
- RabbitMQ (Exposes port `5672` to the host for AMQP and `15672` for management)