Feature: Ranking

Background:
    Given a player named Michel
    And a player named Geale
    And a venue named Amsterdam
    And a game type named tafeltennis

Scenario: No games played yet
	Given no games played
	When nothing happens
	Then we have the following ranking:
			| Rank| Name | Elo |

Scenario: First ever game played
	Given no games played
	And elo system with k-factor 50 and n is 400
	When the following tafeltennis games are played in Amsterdam:
			| Registration Date | First Player | Second Player | S1 | S2 |
			| 2019-11-23 16:04  | Michel       | Geale         | 2  | 1  |
	Then we have the following ranking:
			| Rank | Name   | Elo  |
			| 1    | Michel | 1230 | 
			| 2    | Geale	| 1170 | 

