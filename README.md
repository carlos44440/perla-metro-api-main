# parla-metro-api-main

##Tecnologias 

-.Net 9.0(backend) 
-ocelot(orquestador) 
-visual studio code(IDE)

## Arquitectura

La arquitectura usada es la arquitectura SOA, o Arquitectura Orientada a Servicios, es un estilo de arquitectura de software en el que la aplicación se construye como un conjunto de servicios independientes y reutilizables, que se comunican entre sí a través de protocolos estandarizados, generalmente HTTP/HTTPS con APIs REST o SOAP. Esto se puede ver reflejado en el proyecto al separar las responsabilidades, la api main, entre otras cosas, actua como apiGetaway y orquesta los servicios de usuarios, tickets, rutas y estaciones. Esto hace que la aplicación sea más escalable, flexible y mantenible, cumpliendo con buenas prácticas modernas en el desarrollo de sistemas distribuidos.

## Patron de diseño
El patron de diseño usado es el Modelo-Vista-Controlador (MVC), es un patrón de arquitectura de software que separa la aplicación en tres componentes principales, cada uno con responsabilidades bien definidas. Esta separación permite organizar mejor el código, facilitar el mantenimiento y mejorar la escalabilidad de la aplicación.

## Instalacion y configuracion

1) clonamos el proyecto con: git clone https://github.com/carlos44440/perla-metro-api-main.git

2) abrimos visual studio code en la carpeta del proyecto o entramos por comandos usando cd .\parla-metro-api-main\  y luego nuevamente ejecutamos cd .\parla-metro-api-main\

3) ejecutamos el comando: dotnet restore

4) iniciamos el proyecto con el comando: dotnet run
