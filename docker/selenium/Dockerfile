FROM selenium/standalone-chrome:96.0

# Install packages
RUN sudo apt-get update && \
    sudo apt-get install --no-install-recommends -y python3-pip apt-utils

# Install selenium & pyvirtualdisplay
RUN pip3 install selenium pyvirtualdisplay

# Copy python scripts
COPY src/selenium_screenshot.py selenium_screenshot.py

USER root
RUN mkdir -p /output
RUN chmod 755 /output

# Run script
CMD ["python3", "selenium_screenshot.py"]
