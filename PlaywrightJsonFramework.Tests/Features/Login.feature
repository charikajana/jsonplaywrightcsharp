@login @sanity
Feature: User Authentication
  Test user login functionality with JSON-driven approach

  Scenario: Successful Login
    Given User navigates to "https://the-internet.herokuapp.com/login"
    When User enters username "tomsmith" and password "SuperSecretPassword!"
    And User clicks login button
    Then User should see "You logged into a secure area!"
