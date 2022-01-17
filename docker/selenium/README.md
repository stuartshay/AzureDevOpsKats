# docker-selenium-screenshot

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/azuredevopskats-selenium.svg)](https://hub.docker.com/r/stuartshay/azuredevopskats-selenium/)

Container for taking screenshots using selenium
https://github.com/ubidots/docker-selenium-screenshot

## Allowed arguments

1. **url**: the URL of the page to take the screenshot
2. **file_name**: Name of the image which will be saved (e.g `/path/of/my/image.png`)
3. **width**: Initial width for the browser
4. **height**: Initial height for the browser
5. **app_selector_xpath** (_optional_): XPath selector for search the target content in the DOM. Default `//div[@id='root']`
6. **not_load_selector_xpath** (_optional_): XPath selector when it can't find the content. Default `div[contains(@class, 'selenium-data-not-loaded')]`
7. **timeout** (_optional_): The time to wait until the content load. Default `10s`

**Note**: Every argument is positional, each one must be sent in the order described above.

## Usage

To build the container, execute:

```
# Switch to Dockerfile path
cd docker-selenium-screenshot

# Run docker build
docker build --tag <tag name> .
```

The container has a volume where it will put the screenshots took `tmp/assets`

```
docker run -v /tmp/assets:/tmp/assets -v /dev/shm:/dev/shm <tag name> <url_param> <file_name_param> <width_param> <height_param> <app_selector_xpath_param> <not_load_selector_xpath_param>
```

e.g

```
docker run -v /tmp/assets:/tmp/assets -v /dev/shm:/dev/shm <target-name> https://ubidots.com /tmp/assets/ubidots.png "1000" "500" "//body" "//target"
```

**Note**: When executing docker run include `-v /dev/shm:/dev/shm` to use the host's shared memory.
