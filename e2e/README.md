# E2E Tests

## Jmeter

### Prerequisites

```
Java 8+
Jmeter 5.4.3

https://jmeter.apache.org/download_jmeter.cgi
```

Plugins

- Jmeter Plugin Manager
- Custom Thread group

## Setup

- Download/Setup Java 8+
- Extract JMeter Binaries & Set Path
- Install the JMeter Plugin Manager

```
https://jmeter-plugins.org/install/Install/
```

- Add the Custom Thread Group Plugin

## JMeter GUI

![](/assets/jmeter.png)

## Command Line

**Load Test Report**

```
jmeter -n -t PathOfJmeterFile -l PathOfResultFile
jmeter -n -t script.jmx -l scriptresults.jtl
```
