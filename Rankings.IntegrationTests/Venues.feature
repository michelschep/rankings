Feature: Venues

Scenario: First venue is added
	Given no venues registrated
	When nothing happens
	Then we have the following venues:
		| DisplayName | 

Scenario: No venues yet
	Given no venues registrated
	When venue Groningen is registrated
	Then we have the following venues:
		| DisplayName | 
		| Groningen   | 

Scenario: Multiple venues are shown in correct order
	Given no venues registrated
	When venue Groningen is registrated
	And venue Almere is registrated
	Then we have the following venues:
		| DisplayName | 
		| Almere      | 
		| Groningen   | 
