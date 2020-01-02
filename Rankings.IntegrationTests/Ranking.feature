Feature: Ranking

Background:
	Given a player named Michel
	And a player named Geale
	And a venue named Amsterdam
	And a game type named tafeltennis
	And the current user is Michel with role Admin
	And elo system with k-factor 50 and n is 400 and initial elo is 1200
	And margin of victory active

Scenario: No games played yet
	Given no games played
	When I view the tafeltennis ranking
	Then we have the following tafeltennis ranking with precision 0:
		| Ranking | NamePlayer | Points |
		| 1       | Geale      | 1200   |
		| 1       | Michel     | 1200   |

Scenario: First ever game played
	Given no games played
	When the following tafeltennis games are played in Amsterdam:
		| Registration Date | First Player | Second Player | S1 | S2 |
		| 2019-11-23 16:04  | Michel       | Geale         | 2  | 1  |
	Then we have the following tafeltennis ranking with precision 0:
		| Ranking | NamePlayer | Points |
		| 1       | Michel     | 1217   |
		| 2       | Geale      | 1183   |

Scenario: Player played two games
	Given no games played
	When the following tafeltennis games are played in Amsterdam:
		| Registration Date | First Player | Second Player | S1 | S2 |
		| 2019-11-23 16:04  | Michel       | Geale         | 2  | 1  |
		| 2019-11-23 17:04  | Michel       | Geale         | 1  | 2  |
	Then we have the following tafeltennis ranking with precision 0:
		| Ranking | NamePlayer | Points |
		| 1       | Geale      | 1202   |
		| 2       | Michel     | 1198   |

Scenario: Two games played between the same players but registered by different players 
	Given no games played
	When the following tafeltennis games are played in Amsterdam:
		| Registration Date | First Player | Second Player | S1 | S2 |
		| 2019-11-23 16:04  | Michel       | Geale         | 2  | 1  |
		| 2019-11-23 17:04  | Geale        | Michel        | 2  | 1  |
	Then we have the following tafeltennis ranking with precision 0:
		| Ranking | NamePlayer | Points |
		| 1       | Geale      | 1202   |
		| 2       | Michel     | 1198   |

