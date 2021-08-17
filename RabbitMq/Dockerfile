FROM rabbitmq:3.9.3-management

# #Copy our custom config file containing default usernames and passwords
# ADD rabbitmq.conf /etc/rabbitmq/
# ADD definitions.json /etc/rabbitmq/
# RUN chown rabbitmq:rabbitmq /etc/rabbitmq/rabbitmq.conf /etc/rabbitmq/definitions.json
# CMD ["rabbitmq-server"]

# Define environment variables.
ENV RABBITMQ_USER eaglerockuser
ENV RABBITMQ_PASSWORD eaglerockuser
ENV RABBITMQ_PID_FILE /var/lib/rabbitmq/mnesia/rabbitmq

# Add script to create Rabbit Users before starting rabbit service
ADD init.sh /init.sh
RUN chmod +x /init.sh
EXPOSE 15672
EXPOSE 5672

# Define default command
CMD ["/init.sh"]