@functionaltesting
Feature: OfficeWorksSearch
	As a shopper of officeworks website
	I want to search for brother laser printers
	So I can order them if the required model is available

@smoketesting
Scenario Outline: Search for printer
	Given I am on the officeworks homepage '<TestcaseName>'
	When I search for the product
	Then I look for the required model
	And I found it is unavailable online
Examples:
	| TestcaseName |
	| TC001        |
