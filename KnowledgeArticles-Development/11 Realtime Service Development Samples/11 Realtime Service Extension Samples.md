## Table of Contents
- [Topic](#topic)
- [Background](#Background)
  - [Subsection 2.1](#subsection-21)
  - [Subsection 2.2](#subsection-22)
- [Section 3](#section-3)

## Topic
Deep Dive D365 Commerce Realtime Service:  from single value communication to array communication.

## Background 
Real-time Service enables clients to interact with Commerce functionality in real time. Finance and Operation databases and classes can’t be accessed directly from Retail server. You should access them through the CDX class extension using the finance and operations and Commerce Runtime extension, see [Commerce Data Exchange - Real-time Service](https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/extend-commerce-data-exchange)

But it is unfortunately, the samples provided in the official document is too simple,  it only demonstrates one single string value communicatation scenario, actually in real world,  the scenario is much more complex, Object Array is a very Commmon Scenario.

In this article,  I will demonstration two Realtime Service Scenario:<br/>
Scenario #1：RetailServer read the data from F&O Database table and render the date on POS view.<br/>
Scenario #2: Create a new record in F&O database table,  and show the record in FO form, grid view.<br/>
