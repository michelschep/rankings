Feature: Elo

Background:
	Given a player named Amy
	And a player named Brad
	And a player named Cindy
	And a player named Dirk
	And a venue named Amsterdam
	And a game type named tafeltennis
	And the current user is Amy with role Admin
	And elo system with k-factor 5 and n is 50 and initial elo is 100

Scenario: At the start of the ranking
	Given no games played
	Then we have the following tafeltennis ranking with precision 1:
		| Ranking | NamePlayer | Points |
		| 1       | Amy        | 100.0  |
		| 2       | Brad       | 100.0  |
		| 3       | Cindy      | 100.0  |
		| 4       | Dirk       | 100.0  |
		

Scenario: First round
	Given no games played
	When the following tafeltennis games are played in Amsterdam:
		| Registration Date | First Player | Second Player | S1 | S2 |
		| 2019-11-23 16:04  | Amy		   | Brad          | 1  | 0  |
	Then we have the following tafeltennis ranking with precision 1:
		| Ranking | NamePlayer | Points |
		| 1       | Amy        | 102.5  |
		| 2       | Cindy      | 100.0  |
		| 3       | Dirk       | 100.0  |
		| 4       | Brad       |  97.5  |

Scenario: Second round
	Given no games played
	When the following tafeltennis games are played in Amsterdam:
		| Registration Date | First Player | Second Player | S1 | S2 |
		| 2019-11-23 16:04  | Amy		   | Brad          | 1  | 0  |
		| 2019-11-23 16:04  | Dirk		   | Cindy         | 1  | 0  |
	Then we have the following tafeltennis ranking with precision 1:
		| Ranking | NamePlayer | Points |
		| 1       | Amy        | 102.5  |
		| 2       | Dirk       | 102.5  |
		| 3       | Brad       |  97.5  |
		| 4       | Cindy      |  97.5  |

Scenario: Third round
	Given no games played
	When the following tafeltennis games are played in Amsterdam:
		| Registration Date | First Player | Second Player | S1 | S2 |
		| 2019-11-23 16:04  | Amy		   | Brad          | 1  | 0  |
		| 2019-11-23 16:04  | Dirk		   | Cindy         | 1  | 0  |
		| 2019-11-23 16:04  | Amy		   | Cindy         | 1  | 0  |
	Then we have the following tafeltennis ranking with precision 2:
		| Ranking | NamePlayer | Points |
		| 1       | Amy        | 104.71 |
		| 2       | Dirk       | 102.50 |
		| 3       | Brad       |  97.50 |
		| 4       | Cindy      |  95.29 |