Feature: Venues

Scenario: No venues
	Given no venues registrated
	When nothing happens
	Then we have the following venues:
		| DisplayName | 

Scenario: First ever venue is registered
	Given no venues registrated
	When venue Groningen is registrated
	Then we have the following venues:
		| DisplayName | 
		| Groningen   | 

Scenario: Multiple venues are shown in correct order
	Given no venues registrated
	When venue Groningen is registrated
	And venue Almere is registrated
	And venue Joure is registrated
	Then we have the following venues:
		| DisplayName | 
		| Almere      | 
		| Groningen   | 
		| Joure       | 

Scenario: Cannot register venue with name whih is already used
	Given venue Almere exists
	When venue Almere is registrated
	Then we have the following venues:
		| DisplayName | 
		| Almere      | 

# unit test different invalid venue scenario's
Scenario: User cannot register invalid venue
	Given no venues registrated
	When incorrect venue is registrated
	Then we have the following venues:
		| DisplayName | 
