# BeOrganized
The project BeOrganized is a web app, the goal of which is to make your life more organized. In this app, you can create future events and goals with habits. When creating an event, you can view the address in the Google Maps window. Creating a habit requires specifying the frequency, duration, and in what part of the day to be held. Habits are automatically generated four weeks ahead. There is a search field in which you can easily find any event.
In terms of infrastructure, two cloud providers are used. They are Heroku and Azure. Both the app and SQL database are hosted in Azure, while the Elasticsearch is in Heroku because of the more reasonable cost. For dragging and changing events and habits it uses SignalR, the data is transferred through WebSocket. SendGrid is used for confirming the email of the user. Google reCaptcha controls the contact form. Automated Pipeline in AzureDevOps builds the app which with every commit and notifies if there are some errors.

https://beorganized.azurewebsites.net

## Build status

[![Build Status](https://dev.azure.com/eleonormanolova/BeOrganized/_apis/build/status/BeOrganized%20CI?branchName=master)](https://dev.azure.com/eleonormanolova/BeOrganized/_build/latest?definitionId=1&branchName=master)

## Build with
* ASP.NET Core
* EF Core 3.1
* SignalR
* Sendgrid
* Elasticsearch
* Google Places
* reCaptcha v3
* FullCalendar
* Moq

## Hosted on
* Azure
* Heroku

## Authors

- [Eleonor Manolova](https://github.com/EleonorManolova)

## Calendar

![Calendar](/images/01.Calendar.jpg)


## Create Event

![Create Event](/images/02.EventCreate2.jpg)

## Create Goal

![Create Goal](/images/03.GoalCreate.jpg)

## Habit Details

![Habit Details](/images/04.HabitDetails.jpg)

## Search Results

![Search Results](/images/05.SearchResults.jpg)

## Admin Dashboard

![Admin Dashboard](/images/06.AdminDashboard.jpg)

## Admin Goals

![Admin Goals](/images/07.AdminGoals.jpg)
