# SandboxBerry
A tool for copying test data into Salesforce sandboxes

# What is this for?

During Salesforce development you will generally create Sandboxes based on your production environment, and then 
do development in the Sandbox. Most Salesforce Sandboxes are blank (except for the more expensive ones). Generally to do
some development you're going to want some data in there, for example

* Any reference tables/objects you've created (e.g. Country__c - a list of countries)
* Some sample data in the main tables to experiment with during development.

So ... you can try and copy some data accross using DataLoader or an ETL tool, but you'll quickly find:

* Ids of each row of data change when you transfer it to the Sandbox, which makes things difficult with related data 
(e.g. Accounts and Contacts)
* You will sometimes have different fields in your Sandbox (old fields removed, new fields added)
* Users who are owners of records may have been deactivated in production or might be missing from the Sandbox, 
making it difficult to transfer their records

SandboxBerry can help with these problems.

This is how SandboxBerry works:

* You give it a list of Salesforce objects (in the form of an XML file)
* optionally you can specify filters for those objects in SOQL syntax
* SandboxBerry will then transfer the data for those objects from your Source (usually Production) to a Target (always a Sandbox)
* While transferring the data, it fixes the following problems:
  * Relationships between objects will automatically be preserved, substituting new Ids for the old Ids
  * Any inactive or missing users will be defaulted to the user specified by you
  * Only the objects fields that are present in both Source and Target will be transferred
* Optionally, you can skip specific fields or hard-code their values in the Target - this can be useful for anonymisation

Please note that SandboxBerry takes some liberties with the data to get it to fit into your sandbox (e.g. changing the owner 
of records). The idea is: Its test data, so its better to have something rather than nothing. However this means that SandboxBerry 
should not be used for transferring **into** a production instance. Because of this, SandboxBerry will only allow you to specify Sandboxes 
as the target.

# License

SandboxBerry is Open Source under the GPL2 license.

# Downloading

SandboxBerry is a windows application written in C# and uses the .NET Framework 4.5.  
Visual Studio 2012 was used for the initial project but it should be easily loadable in more recent versions of VS.  
Download the solution and ooen it in VS

# Getting Started

Refer to the [Wiki](https://github.com/codeulike/SandboxBerry/wiki) for instructions on how to use SandboxBerry

