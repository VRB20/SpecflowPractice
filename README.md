BDD based UI Automation Framework built with C# and Specflow.

Unit tests of the framework components are performed by NUnit.

Given examples to read the test settings/configurations from App Config file and also from a Json Configuration File.

Implements Page Factory based Page Object Model pattern.

Implemented a separate Object Repository where locators are placed in a CSV file and then construct objects dynamically during run time. This helps to keep the Page class neat and simple.

Also given examples to showcase the traditional method of keeping the locators in the Page class as well.

Test Data will be read from a CSV File and used in the tests.

Implements Test Data Management, Environment Managment and Secrets(Credentials) Management based on the configuration/settings. For example if config is set as "Test", then URL and Credentials for the Test Environment will be used. Similarly Test Data Files for the corresponding environment will be used.

Chrome Driver is placed in the "Drivers" Folder. Download the latest version and place it there. 

Data is shared between various "Step Definition" classes using Context Injection functionality provided by Specflow. This avoids the use of static variables.

Uses WebDriver Manager, Page Object Manager and Test Context classes to keep the maintenance effort to minimum.
