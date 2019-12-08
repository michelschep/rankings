Feature: Venues

Scenario: No venues yet
	Given no venues registrated
	When venue Groningen is registrated
	Then we have the following venues:
		| DisplayName | 
		| Groningen   | 