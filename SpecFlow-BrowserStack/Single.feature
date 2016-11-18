Feature: Google

Scenario: Can find search results
	Given I am on the google page
	When I search for "BrowserStack"
	Then I get search results
