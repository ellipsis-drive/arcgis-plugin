# arcgis-plugin
Ellipsis plugin for ArcGIS

# Installation Instructions:
* Step 1: Download the file "ellipsis_drive_addin.esriAddIn"
![step1](https://user-images.githubusercontent.com/55835594/198554472-620f82bd-5efe-47bc-890a-78591d1af5d6.JPG)
* Step 2: Extract the file and double click, then click on "Install Add-In"
![step2](https://user-images.githubusercontent.com/55835594/198554995-041cc00d-b581-4d13-954e-ee191635e4a4.JPG)
* Step 3: Open ArcMap
* Step 4: Click on "Customize" in the menu bar
![step4](https://user-images.githubusercontent.com/55835594/173599821-445704e4-3907-4497-8a3a-b242086bbb2a.PNG)
* Step 5: Hover over "Toolbars" and in the drop down menu, click "Customize..."
![step5](https://user-images.githubusercontent.com/55835594/173600111-faf89ded-0d25-4a6d-ba12-089b24aa87f1.png)
* Step 6: In the new window that popped up, click on the "Commands" tab
![step6](https://user-images.githubusercontent.com/55835594/173600349-c7f72a5b-55c6-46c0-8808-bc6cfed62679.PNG)
* Step 7: In the "Categories:" box, scroll down and click on the "EDrive" category
![step7](https://user-images.githubusercontent.com/55835594/198558191-49a4727a-2ec7-4b8b-8084-e6079c6f6736.png)
* Step 8: Drag the "Ellipsis Drive" command to one of the toolbars of the main ArcMap window.
* Step 9: Click on the icon in the toolbar, and the arcgis plugin should appear
![step9](https://user-images.githubusercontent.com/55835594/198557800-b73a56ad-33c9-469c-bf34-93a2650a3e35.png)

# Using the plugin
Enter your username and password in the login form. The filesystem of your Ellipsis Drive account will be loaded:
![use1](https://user-images.githubusercontent.com/55835594/173602992-bbb533e6-9719-42fe-9516-7ebcc03a711d.PNG)
Use the dropdown menu to navigate to the requested layer: 
![use2](https://user-images.githubusercontent.com/55835594/173602787-42886a27-2faf-4d21-a3a7-cc7ec26872f4.PNG)
Double click the requested layer. It will be loaded in the Data View of ArcMap: (NB: The ArcMap plugin only supports .wmts layers)
![use3](https://user-images.githubusercontent.com/55835594/173602653-a31e3ba0-286f-41c9-826f-609fa9b7553b.PNG)

# Uninstalling the plugin
* Step 1: Open ArcMap
* Step 2: In the top menu, click "Customize", then click "Add-In Manager..." in the dropdown menu
![step2](https://user-images.githubusercontent.com/55835594/198555700-81697f3c-5737-4460-bdfe-d83a7dc5b0a0.png)
* Step 3: In the newly opened window, select "elipsis_drive_addin" in "My Add-Ins" and click "Delete this Add-In"
![step3](https://user-images.githubusercontent.com/55835594/198556266-27699cfb-a519-4036-8b8a-cb983d9e5f4e.png)

The plugin icon will still remain in one of the toolbars (In my case it is the "Tools" toolbar that is highlighted in red).
To remove this icon, click on "Customize" in the top menu, then click "Customize Mode...". In the "Toolbars" tab, scroll down to the toolbar where you dragged 
the plugin to and click on reset.
![remove_icon](https://user-images.githubusercontent.com/55835594/198557414-eb3e05db-07fe-411d-815e-cee721d7c5c4.png)



