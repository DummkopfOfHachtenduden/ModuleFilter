# ModuleFilter
XFilter based port of VSRO-Module-Sniffer

## Configuration

### Loggers
> Work in progress

### Plugins
`Name` specifies the assembly name of your plugin dll   
`ServiceType` represents the services the plugin will be added to.

```xml
<Plugin Name="Silkroad.Plugin.Global" ServiceType="GlobalManager" />
```

### Services
`Security` should not be touched.  
`Certificator` represents the end point the service certifying against which the module would normally do.  

It's possible to have none or also multiple redirections.  
`CoordID` is the NodeLinkID between module and certificator.  
`MachineID` is not yet supported.  
`Port` for service to be redirected (self or dedicated-service)  

```xml
  <Service Name="GlobalManager #01" Type="GlobalManager" IP ="192.168.178.15" Port="20000">
    <Security Blowfish="true" CRC="false" Handshake="true" />
    <Certificator IP="192.168.178.15" Port="32000" />
    <redirections>
      <Redirect CoordID="1227" MachineID="335" Port="20000" />      
    </redirections>
  </Service>
```

### Service Overview
(Click for fullscreen)
![ModuleFilter](https://rawgit.com/DummkopfOfHachtenduden/ModuleFilter/master/ModuleFilter.svg)
