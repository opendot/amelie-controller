# Amelie
 Amelie Suite is a set of software co-designed for people suffering from Rett's syndrome characterized by an innovative way of interaction between care-giver and care-receiver, both equipped with an instrument on their own device, and through the use of an eyetracker (which allows you to track the look of the subject and to determine which point on the screen is watching).


Amelie is an open source software and accessible to everyone, co-designed by designers, developers, university researchers, families and therapists. Amelie promotes communication and cognitive enhancement for learning and improving interaction and understanding skills.
The system integrates different technologies (mobile applications, cloud services and adaptive algorithms) to provide an innovative, comprehensive and easy-to-use service. 


The software was born from an idea of Associazione Italiana Rett - AIRETT Onlus and Opendot S.r.l., and was designed and developed by Opendot S.r.l., with the essential contribution of Associazione Italiana Rett - AIRETT Onlus.

This repository hosts the background process controlling the status of the Amelie server on desktop PCs.


# amelie-controller

This component of the Amelie suite acts as daemon for the airett rails server, loads it at startup, and enables the user to open the interface (airett-communicator).
It is a tray icon application, the menu can be accessed by clicking on it with a right click of the mouse (or a long touch). The icon reveals the current state of the server (running or down). 

Technologies
---

The controller is developed as a .NET c# project, and depends on the following nuGet packages:

- Tobii.Research.x64


Compile and run
---

- Open the project with visual studio 2017, install the required nuGet packages and run the compilation (Debug or Release, depending on your needs).
- Once compiled, in the destination folder open the file "amelieController.exe.config"
- in the appSettings section, modify the value with key "programFolder" to address the installation folder of the Amelie suite (this step is performed automatically by the Amelie installer)
- run the executable

