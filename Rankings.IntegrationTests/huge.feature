Feature: Huge

Background:
	Given a player named Michel
	And a player named Geale
	And a player named Johannes
	And a player named Hans
	And a player named Irma
	And a player named Vink
	And a player named Matthias
	And a player named Tarik
	And a player named Remco
	And a player named Enrico
	And a player named Jeroen
	And a player named Yassine
	And a player named Eduard
	And a player named Arnold1
	And a player named Arnold2
	And a player named Tjardo
	And a player named Bertus
	And a player named Harro
	And a player named Eltjo
	And a player named Ricky
	And a player named Mulder
	And a player named Arjen
	And a player named Driek
	And a venue named Amsterdam
	And a game type named tafeltennis
	And the current user is Michel with role Admin
	And elo system with k-factor 50 and n is 400 and initial elo is 1200
	And margin of victory active
	And only in ranking with a minimum of 7 games
	And with 2019 elo calculator

Scenario: Really many games
	Given no games played
	When the following tafeltennis games are played in Amsterdam:
		| Registration Date | First Player | Second Player | S1 | S2 |
		| 2019-09-23 10:40  | Michel       | Remco         | 2  | 1  |
		| 2019-09-23 11:16  | Geale        | Johannes      | 1  | 3  |
		| 2019-09-23 12:34  | Johannes     | Hans          | 1  | 3  |
		| 2019-09-23 14:49  | Geale        | Irma          | 2  | 1  |
		| 2019-09-24 07:44  | Enrico       | Irma          | 0  | 2  |
		| 2019-09-24 07:47  | Vink         | Matthias      | 1  | 2  |
		| 2019-09-24 09:47  | Tarik        | Geale         | 1  | 2  |
		| 2019-09-24 10:29  | Remco        | Geale         | 2  | 1  |
		| 2019-09-24 11:42  | Johannes     | Geale         | 3  | 0  |
		| 2019-09-24 12:42  | Geale        | Tarik         | 3  | 0  |
		| 2019-09-24 14:03  | Geale        | Michel        | 0  | 3  |
		| 2019-09-24 15:02  | Michel       | Geale         | 1  | 2  |
		| 2019-09-24 15:12  | Geale        | Tarik         | 2  | 1  |
		| 2019-09-25 08:41  | Michel       | Geale         | 1  | 2  |
		| 2019-09-25 10:13  | Irma         | Enrico        | 2  | 0  |
		| 2019-09-25 10:14  | Irma         | Jeroen        | 2  | 0  |
		| 2019-09-25 10:15  | Irma         | Michel        | 0  | 2  |
		| 2019-09-25 10:46  | Geale        | Hans          | 0  | 3  |
		| 2019-09-25 10:59  | Michel       | Tarik         | 3  | 0  |
		| 2019-09-25 12:25  | Michel       | Hans          | 1  | 3  |
		| 2019-09-25 12:41  | Tarik        | Geale         | 0  | 3  |
		| 2019-09-25 13:41  | Michel       | Geale         | 0  | 3  |
		| 2019-09-25 14:56  | Michel       | Geale         | 2  | 1  |
		| 2019-09-26 09:09  | Irma         | Matthias      | 2  | 0  |
		| 2019-09-26 10:38  | Michel       | Remco         | 1  | 2  |
		| 2019-09-26 09:40  | Geale        | Michel        | 3  | 0  |
		| 2019-09-26 12:45  | Michel       | Geale         | 3  | 0  |
		| 2019-09-25 17:00  | Hans         | Geale         | 2  | 0  |
		| 2019-09-30 11:01  | Michel       | Yassine       | 1  | 0  |
		| 2019-09-30 11:06  | Hans         | Irma          | 3  | 0  |
		| 2019-10-01 09:27  | Matthias     | Eduard        | 1  | 2  |
		| 2019-10-01 11:07  | Eduard       | Matthias      | 2  | 1  |
		| 2019-10-01 13:07  | Irma         | Matthias      | 2  | 1  |
		| 2019-10-01 13:07  | Irma         | Matthias      | 2  | 1  |
		| 2019-10-02 09:22  | Matthias     | Bertus        | 0  | 3  |
		| 2019-10-02 09:31  | Arnold1      | Tjardo        | 3  | 0  |
		| 2019-10-02 09:57  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-02 09:57  | Arnold1      | Geale         | 1  | 2  |
		| 2019-10-02 10:38  | Harro        | Bertus        | 0  | 3  |
		| 2019-10-02 11:07  | Arnold1      | Tarik         | 3  | 0  |
		| 2019-10-02 11:35  | Eduard       | Bertus        | 1  | 3  |
		| 2019-10-02 13:19  | Matthias     | Eduard        | 0  | 3  |
		| 2019-10-02 14:31  | Tarik        | Tjardo        | 3  | 0  |
		| 2019-10-02 14:43  | Arnold1      | Geale         | 1  | 2  |
		| 2019-10-03 08:59  | Michel       | Geale         | 1  | 2  |
		| 2019-10-03 12:07  | Matthias     | Eduard        | 1  | 2  |
		| 2019-10-03 12:52  | Michel       | Geale         | 1  | 2  |
		| 2019-10-03 14:16  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-03 14:35  | Hans         | Eduard        | 3  | 0  |
		| 2019-10-04 08:27  | Michel       | Geale         | 3  | 0  |
		| 2019-10-04 09:46  | Michel       | Arnold1       | 1  | 2  |
		| 2019-10-04 11:09  | Johannes     | Bertus        | 3  | 0  |
		| 2019-10-04 11:25  | Tarik        | Geale         | 0  | 3  |
		| 2019-10-04 11:25  | Arnold1      | Geale         | 1  | 2  |
		| 2019-10-04 12:36  | Irma         | Bertus        | 2  | 0  |
		| 2019-10-04 13:07  | Geale        | Arnold1       | 3  | 0  |
		| 2019-10-04 15:26  | Michel       | Geale         | 0  | 3  |
		| 2019-10-07 09:36  | Arnold1      | Michel        | 3  | 0  |
		| 2019-10-07 10:41  | Arnold1      | Remco         | 3  | 0  |
		| 2019-10-07 10:54  | Arnold1      | Tarik         | 3  | 0  |
		| 2019-10-07 11:13  | Eduard       | Mulder        | 2  | 1  |
		| 2019-10-07 12:33  | Arnold1      | Hans          | 2  | 1  |
		| 2019-10-07 12:39  | Geale        | Enrico        | 2  | 0  |
		| 2019-10-07 14:29  | Michel       | Hans          | 1  | 2  |
		| 2019-10-07 14:44  | Johannes     | Irma          | 2  | 1  |
		| 2019-10-07 14:45  | Johannes     | Geale         | 3  | 0  |
		| 2019-10-07 16:47  | Irma         | Enrico        | 2  | 0  |
		| 2019-10-08 09:00  | Geale        | Michel        | 2  | 1  |
		| 2019-10-08 10:34  | Geale        | Remco         | 1  | 2  |
		| 2019-10-08 10:49  | Johannes     | Eduard        | 3  | 0  |
		| 2019-10-08 13:18  | Geale        | Michel        | 0  | 3  |
		| 2019-10-08 14:31  | Eduard       | Irma          | 2  | 1  |
		| 2019-10-08 15:31  | Hans         | Johannes      | 3  | 0  |
		| 2019-10-08 15:32  | Hans         | Johannes      | 0  | 3  |
		| 2019-10-09 10:12  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-09 10:45  | Geale        | Michel        | 0  | 3  |
		| 2019-10-09 14:14  | Michel       | Geale         | 1  | 2  |
		| 2019-10-09 14:59  | Johannes     | Harro         | 3  | 0  |
		| 2019-10-10 10:19  | Arnold1      | Tjardo        | 3  | 0  |
		| 2019-10-10 11:54  | Geale        | Hans          | 0  | 3  |
		| 2019-10-11 08:20  | Michel       | Geale         | 3  | 0  |
		| 2019-10-11 10:50  | Michel       | Geale         | 2  | 1  |
		| 2019-10-11 13:02  | Johannes     | Irma          | 3  | 1  |
		| 2019-10-11 13:20  | Michel       | Geale         | 0  | 3  |
		| 2019-10-11 14:13  | Hans         | Bertus        | 3  | 0  |
		| 2019-10-14 11:15  | Geale        | Hans          | 0  | 3  |
		| 2019-10-14 11:34  | Geale        | Arjen         | 2  | 1  |
		| 2019-10-14 13:18  | Hans         | Arjen         | 3  | 0  |
		| 2019-10-14 14:23  | Arjen        | Irma          | 2  | 1  |
		| 2019-10-15 09:07  | Michel       | Geale         | 1  | 2  |
		| 2019-10-15 10:03  | Arnold1      | Michel        | 2  | 1  |
		| 2019-10-15 10:55  | Michel       | Remco         | 2  | 1  |
		| 2019-10-15 11:27  | Johannes     | Hans          | 1  | 3  |
		| 2019-10-15 12:28  | Geale        | Arnold1       | 3  | 0  |
		| 2019-10-15 13:53  | Michel       | Geale         | 3  | 0  |
		| 2019-10-15 16:20  | Eduard       | Tjardo        | 3  | 0  |
		| 2019-10-16 08:45  | Bertus       | Matthias      | 3  | 0  |
		| 2019-10-16 08:56  | Irma         | Enrico        | 2  | 0  |
		| 2019-10-16 09:05  | Michel       | Arnold1       | 2  | 1  |
		| 2019-10-16 10:46  | Michel       | Remco         | 3  | 0  |
		| 2019-10-16 10:54  | Johannes     | Arnold1       | 3  | 0  |
		| 2019-10-16 11:05  | Johannes     | Geale         | 3  | 0  |
		| 2019-10-16 11:11  | Harro        | Bertus        | 3  | 0  |
		| 2019-10-16 12:06  | Arnold1      | Michel        | 1  | 2  |
		| 2019-10-16 12:07  | Harro        | Matthias      | 4  | 1  |
		| 2019-10-16 12:28  | Arnold1      | Geale         | 2  | 1  |
		| 2019-10-16 12:37  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-16 13:33  | Bertus       | Matthias      | 2  | 1  |
		| 2019-10-16 13:48  | Michel       | Geale         | 1  | 2  |
		| 2019-10-16 14:08  | Arnold1      | Tarik         | 3  | 0  |
		| 2019-10-16 15:43  | Harro        | Matthias      | 2  | 1  |
		| 2019-10-16 15:53  | Harro        | Johannes      | 2  | 1  |
		| 2019-10-16 18:54  | Arjen        | Eduard        | 2  | 0  |
		| 2019-10-16 19:04  | Harro        | Arjen         | 2  | 1  |
		| 2019-10-16 19:12  | Harro        | Hans          | 2  | 1  |
		| 2019-10-16 19:21  | Johannes     | Bertus        | 2  | 0  |
		| 2019-10-16 19:29  | Harro        | Bertus        | 2  | 0  |
		| 2019-10-17 09:09  | Michel       | Geale         | 3  | 0  |
		| 2019-10-17 09:21  | Remco        | Arnold1       | 2  | 1  |
		| 2019-10-17 10:23  | Johannes     | Geale         | 2  | 1  |
		| 2019-10-17 10:40  | Remco        | Irma          | 2  | 1  |
		| 2019-10-17 11:04  | Michel       | Geale         | 2  | 1  |
		| 2019-10-17 11:13  | Irma         | Jeroen        | 2  | 0  |
		| 2019-10-17 12:37  | Michel       | Arnold1       | 2  | 1  |
		| 2019-10-17 13:12  | Yassine      | Tjardo        | 2  | 1  |
		| 2019-10-17 13:27  | Johannes     | Michel        | 3  | 0  |
		| 2019-10-17 14:44  | Remco        | Arnold1       | 2  | 1  |
		| 2019-10-17 15:02  | Johannes     | Arjen         | 3  | 0  |
		| 2019-10-17 16:47  | Geale        | Tjardo        | 2  | 0  |
		| 2019-10-17 16:47  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-18 10:45  | Johannes     | Eduard        | 3  | 0  |
		| 2019-10-18 10:54  | Eduard       | Arjen         | 2  | 1  |
		| 2019-10-18 13:24  | Johannes     | Irma          | 3  | 0  |
		| 2019-10-18 14:18  | Geale        | Tarik         | 2  | 1  |
		| 2019-10-21 08:51  | Michel       | Arnold1       | 2  | 1  |
		| 2019-10-21 10:49  | Arnold1      | Michel        | 3  | 0  |
		| 2019-10-21 12:39  | Hans         | Irma          | 3  | 0  |
		| 2019-10-21 12:46  | Michel       | Arnold1       | 1  | 2  |
		| 2019-10-21 13:07  | Irma         | Enrico        | 3  | 0  |
		| 2019-10-21 15:02  | Enrico       | Irma          | 0  | 2  |
		| 2019-10-22 09:09  | Michel       | Arnold1       | 2  | 1  |
		| 2019-10-22 10:42  | Arnold1      | Remco         | 2  | 0  |
		| 2019-10-22 11:15  | Hans         | Irma          | 3  | 0  |
		| 2019-10-22 12:22  | Michel       | Geale         | 2  | 1  |
		| 2019-10-22 14:47  | Michel       | Geale         | 0  | 3  |
		| 2019-10-22 14:53  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-23 09:07  | Hans         | Michel        | 2  | 1  |
		| 2019-10-23 10:16  | Irma         | Enrico        | 2  | 0  |
		| 2019-10-23 10:25  | Michel       | Geale         | 3  | 0  |
		| 2019-10-23 10:41  | Hans         | Geale         | 2  | 0  |
		| 2019-10-23 10:41  | Hans         | Geale         | 3  | 0  |
		| 2019-10-23 10:45  | Bertus       | Enrico        | 3  | 0  |
		| 2019-10-23 11:02  | Arnold2      | Enrico        | 1  | 2  |
		| 2019-10-23 13:08  | Eltjo        | Michel        | 0  | 3  |
		| 2019-10-23 13:12  | Ricky        | Irma          | 0  | 3  |
		| 2019-10-23 13:56  | Michel       | Geale         | 3  | 0  |
		| 2019-10-23 13:59  | Bertus       | Ricky         | 2  | 0  |
		| 2019-10-23 14:03  | Geale        | Eltjo         | 3  | 0  |
		| 2019-10-23 14:04  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-23 14:21  | Bertus       | Arnold2       | 3  | 0  |
		| 2019-10-23 14:34  | Eltjo        | Jeroen        | 2  | 1  |
		| 2019-10-23 14:47  | Tarik        | Jeroen        | 2  | 1  |
		| 2019-10-24 09:14  | Michel       | Tarik         | 3  | 0  |
		| 2019-10-24 10:42  | Remco        | Arnold1       | 2  | 1  |
		| 2019-10-24 10:49  | Irma         | Ricky         | 2  | 0  |
		| 2019-10-24 11:50  | Eduard       | Matthias      | 2  | 1  |
		| 2019-10-24 11:53  | Arnold1      | Tarik         | 3  | 0  |
		| 2019-10-24 12:14  | Arnold1      | Geale         | 2  | 1  |
		| 2019-10-24 13:32  | Michel       | Arnold1       | 2  | 1  |
		| 2019-10-24 13:33  | Geale        | Michel        | 2  | 1  |
		| 2019-10-24 14:17  | Irma         | Matthias      | 3  | 0  |
		| 2019-10-24 14:47  | Eduard       | Ricky         | 3  | 0  |
		| 2019-10-24 15:12  | Geale        | Michel        | 3  | 0  |
		| 2019-10-25 08:43  | Geale        | Tarik         | 3  | 0  |
		| 2019-10-25 08:58  | Bertus       | Eduard        | 3  | 0  |
		| 2019-10-25 09:11  | Arnold1      | Michel        | 2  | 1  |
		| 2019-10-25 10:51  | Michel       | Geale         | 2  | 1  |
		| 2019-10-25 11:43  | Bertus       | Arnold2       | 3  | 0  |
		| 2019-10-25 12:42  | Irma         | Arnold2       | 3  | 0  |
		| 2019-10-25 12:55  | Geale        | Arnold1       | 2  | 1  |
		| 2019-10-25 14:46  | Michel       | Geale         | 2  | 1  |
		| 2019-10-28 10:11  | Geale        | Irma          | 2  | 1  |
		| 2019-10-28 10:23  | Arnold1      | Tarik         | 3  | 0  |
		| 2019-10-28 11:57  | Michel       | Remco         | 3  | 0  |
		| 2019-10-28 11:57  | Michel       | Arnold1       | 1  | 2  |
		| 2019-10-28 14:34  | Arnold1      | Tarik         | 3  | 0  |
		| 2019-10-28 16:36  | Irma         | Enrico        | 2  | 1  |
		| 2019-10-29 10:45  | Michel       | Arnold1       | 3  | 0  |
		| 2019-10-29 11:23  | Michel       | Geale         | 2  | 1  |
		| 2019-10-29 11:38  | Remco        | Jeroen        | 2  | 0  |
		| 2019-10-29 14:10  | Johannes     | Michel        | 2  | 0  |
		| 2019-10-29 14:22  | Johannes     | Arnold1       | 3  | 0  |
		| 2019-10-29 15:39  | Ricky        | Eltjo         | 3  | 0  |
		| 2019-10-29 15:44  | Irma         | Arnold2       | 3  | 0  |
		| 2019-10-29 15:56  | Arnold2      | Ricky         | 2  | 1  |
		| 2019-10-30 10:04  | Irma         | Enrico        | 2  | 0  |
		| 2019-10-30 10:23  | Arnold1      | Michel        | 3  | 0  |
		| 2019-10-30 11:30  | Irma         | Enrico        | 2  | 0  |
		| 2019-10-30 11:47  | Michel       | Hans          | 1  | 2  |
		| 2019-10-30 11:56  | Arnold1      | Irma          | 2  | 1  |
		| 2019-10-30 13:09  | Arnold1      | Hans          | 3  | 0  |
		| 2019-10-30 15:19  | Michel       | Geale         | 1  | 2  |
		| 2019-10-30 15:54  | Johannes     | Arnold1       | 3  | 0  |
		| 2019-10-31 09:22  | Michel       | Arnold1       | 2  | 1  |
		| 2019-10-31 11:31  | Michel       | Geale         | 1  | 2  |
		| 2019-10-31 11:53  | Irma         | Ricky         | 2  | 0  |
		| 2019-10-31 12:15  | Michel       | Eltjo         | 2  | 0  |
		| 2019-10-31 12:24  | Eltjo        | Jeroen        | 0  | 2  |
		| 2019-10-31 13:39  | Johannes     | Irma          | 3  | 0  |
		| 2019-10-31 14:39  | Arnold1      | Geale         | 2  | 1  |
		| 2019-10-31 16:01  | Michel       | Geale         | 2  | 1  |
		| 2019-11-01 12:26  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-01 14:34  | Tarik        | Driek         | 2  | 0  |
		| 2019-11-01 15:26  | Irma         | Enrico        | 3  | 0  |
		| 2019-11-04 09:44  | Geale        | Matthias      | 2  | 0  |
		| 2019-11-04 12:02  | Geale        | Johannes      | 0  | 3  |
		| 2019-11-04 13:45  | Johannes     | Hans          | 3  | 0  |
		| 2019-11-04 14:36  | Geale        | Enrico        | 2  | 0  |
		| 2019-11-04 15:46  | Johannes     | Geale         | 4  | 0  |
		| 2019-11-04 16:38  | Johannes     | Hans          | 3  | 2  |
		| 2019-11-05 14:24  | Johannes     | Hans          | 3  | 2  |
		| 2019-11-05 15:28  | Geale        | Tarik         | 3  | 0  |
		| 2019-11-06 09:44  | Matthias     | Eduard        | 2  | 0  |
		| 2019-11-06 10:12  | Jeroen       | Eltjo         | 2  | 0  |
		| 2019-11-06 11:50  | Johannes     | Geale         | 2  | 1  |
		| 2019-11-06 12:23  | Matthias     | Enrico        | 2  | 1  |
		| 2019-11-06 12:23  | Matthias     | Eduard        | 2  | 1  |
		| 2019-11-06 15:19  | Geale        | Tarik         | 2  | 1  |
		| 2019-11-07 12:02  | Geale        | Tjardo        | 3  | 0  |
		| 2019-11-07 12:06  | Johannes     | Hans          | 3  | 2  |
		| 2019-11-08 10:42  | Johannes     | Bertus        | 3  | 2  |
		| 2019-11-08 11:19  | Remco        | Jeroen        | 2  | 0  |
		| 2019-11-08 12:25  | Johannes     | Bertus        | 3  | 0  |
		| 2019-11-08 14:56  | Tarik        | Geale         | 1  | 2  |
		| 2019-11-11 10:24  | Michel       | Arnold1       | 0  | 3  |
		| 2019-11-11 11:39  | Remco        | Arnold1       | 2  | 1  |
		| 2019-11-11 12:11  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-11 13:41  | Irma         | Enrico        | 2  | 0  |
		| 2019-11-11 14:02  | Michel       | Arnold1       | 0  | 3  |
		| 2019-11-11 14:59  | Geale        | Enrico        | 2  | 0  |
		| 2019-11-12 12:06  | Irma         | Matthias      | 2  | 1  |
		| 2019-11-13 10:18  | Matthias     | Tjardo        | 3  | 0  |
		| 2019-11-13 11:51  | Michel       | Geale         | 3  | 0  |
		| 2019-11-13 12:02  | Johannes     | Harro         | 3  | 0  |
		| 2019-11-13 12:03  | Irma         | Ricky         | 3  | 0  |
		| 2019-11-13 12:07  | Geale        | Hans          | 0  | 3  |
		| 2019-11-13 12:16  | Michel       | Hans          | 0  | 3  |
		| 2019-11-13 13:58  | Michel       | Arnold2       | 2  | 0  |
		| 2019-11-13 14:49  | Geale        | Arnold2       | 3  | 0  |
		| 2019-11-13 14:53  | Irma         | Matthias      | 2  | 0  |
		| 2019-11-13 15:48  | Michel       | Geale         | 2  | 1  |
		| 2019-11-13 17:11  | Eltjo        | Tjardo        | 2  | 1  |
		| 2019-11-14 10:45  | Geale        | Tarik         | 3  | 0  |
		| 2019-11-14 10:56  | Johannes     | Arnold1       | 3  | 0  |
		| 2019-11-14 11:42  | Arnold1      | Remco         | 2  | 1  |
		| 2019-11-14 11:59  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-14 12:16  | Tarik        | Tjardo        | 3  | 0  |
		| 2019-11-14 12:19  | Irma         | Ricky         | 3  | 0  |
		| 2019-11-14 13:22  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-14 14:34  | Geale        | Arnold1       | 3  | 0  |
		| 2019-11-14 15:32  | Michel       | Geale         | 1  | 2  |
		| 2019-11-14 16:43  | Irma         | Ricky         | 3  | 0  |
		| 2019-11-15 11:08  | Johannes     | Bertus        | 3  | 0  |
		| 2019-11-15 12:55  | Bertus       | Matthias      | 4  | 0  |
		| 2019-11-15 14:25  | Geale        | Tarik         | 3  | 0  |
		| 2019-11-15 14:43  | Irma         | Matthias      | 3  | 0  |
		| 2019-11-18 10:46  | Michel       | Arnold1       | 1  | 2  |
		| 2019-11-18 12:16  | Irma         | Enrico        | 2  | 0  |
		| 2019-11-18 12:29  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-18 14:02  | Geale        | Irma          | 2  | 1  |
		| 2019-11-18 14:15  | Matthias     | Ricky         | 3  | 0  |
		| 2019-11-18 14:16  | Michel       | Tarik         | 3  | 0  |
		| 2019-11-18 14:16  | Michel       | Arnold1       | 3  | 0  |
		| 2019-11-18 15:13  | Ricky        | Arnold2       | 2  | 0  |
		| 2019-11-18 15:40  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-19 08:50  | Ricky        | Arnold2       | 2  | 0  |
		| 2019-11-19 10:32  | Johannes     | Hans          | 3  | 1  |
		| 2019-11-19 11:36  | Ricky        | Hans          | 0  | 3  |
		| 2019-11-19 11:54  | Irma         | Matthias      | 2  | 0  |
		| 2019-11-19 11:55  | Irma         | Ricky         | 2  | 1  |
		| 2019-11-19 12:09  | Hans         | Ricky         | 3  | 0  |
		| 2019-11-19 13:51  | Ricky        | Eltjo         | 3  | 0  |
		| 2019-11-19 14:05  | Ricky        | Matthias      | 1  | 2  |
		| 2019-11-19 14:52  | Irma         | Matthias      | 3  | 0  |
		| 2019-11-20 10:23  | Michel       | Geale         | 0  | 3  |
		| 2019-11-20 11:50  | Irma         | Matthias      | 3  | 0  |
		| 2019-11-20 11:55  | Matthias     | Eduard        | 2  | 1  |
		| 2019-11-20 12:11  | Geale        | Michel        | 1  | 2  |
		| 2019-11-20 12:24  | Michel       | Tarik         | 2  | 1  |
		| 2019-11-20 13:06  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-20 14:48  | Johannes     | Michel        | 3  | 0  |
		| 2019-11-20 16:06  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-20 16:06  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-20 16:36  | Johannes     | Michel        | 3  | 0  |
		| 2019-11-21 10:08  | Hans         | Michel        | 3  | 1  |
		| 2019-11-21 11:04  | Johannes     | Eduard        | 3  | 0  |
		| 2019-11-21 11:19  | Remco        | Michel        | 3  | 0  |
		| 2019-11-21 11:32  | Hans         | Michel        | 2  | 0  |
		| 2019-11-21 11:42  | Irma         | Ricky         | 2  | 1  |
		| 2019-11-21 11:43  | Irma         | Matthias      | 2  | 0  |
		| 2019-11-21 11:43  | Irma         | Matthias      | 2  | 0  |
		| 2019-11-21 11:46  | Hans         | Geale         | 3  | 0  |
		| 2019-11-21 14:16  | Michel       | Geale         | 2  | 1  |
		| 2019-11-21 15:19  | Michel       | Tjardo        | 3  | 0  |
		| 2019-11-21 16:12  | Michel       | Geale         | 2  | 1  |
		| 2019-11-22 09:52  | Irma         | Enrico        | 3  | 0  |
		| 2019-11-22 10:54  | Johannes     | Hans          | 3  | 2  |
		| 2019-11-22 15:16  | Geale        | Tarik         | 2  | 1  |
		| 2019-11-25 10:05  | Michel       | Arnold1       | 2  | 1  |
		| 2019-11-25 11:44  | Arnold1      | Remco         | 1  | 0  |
		| 2019-11-25 12:02  | Michel       | Arnold1       | 2  | 1  |
		| 2019-11-25 13:20  | Hans         | Geale         | 3  | 0  |
		| 2019-11-25 14:55  | Geale        | Johannes      | 0  | 3  |
		| 2019-11-25 15:14  | Irma         | Enrico        | 2  | 1  |
		| 2019-11-25 15:18  | Geale        | Johannes      | 0  | 3  |
		| 2019-11-25 15:24  | Michel       | Arnold1       | 1  | 2  |
		| 2019-11-25 15:32  | Ricky        | Arnold2       | 2  | 0  |
		| 2019-11-25 22:09  | Irma         | Enrico        | 3  | 0  |
		| 2019-11-26 09:57  | Irma         | Matthias      | 2  | 1  |
		| 2019-11-26 12:05  | Johannes     | Hans          | 3  | 2  |
		| 2019-11-26 15:46  | Johannes     | Hans          | 3  | 2  |
		| 2019-11-26 15:47  | Johannes     | Hans          | 2  | 1  |
		| 2019-11-27 10:03  | Arnold1      | Michel        | 2  | 1  |
		| 2019-11-27 10:04  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-27 11:11  | Michel       | Hans          | 0  | 3  |
		| 2019-11-27 11:32  | Bertus       | Harro         | 3  | 0  |
		| 2019-11-27 11:45  | Irma         | Matthias      | 2  | 1  |
		| 2019-11-27 11:56  | Harro        | Johannes      | 0  | 3  |
		| 2019-11-27 11:57  | Michel       | Arnold1       | 1  | 1  |
		| 2019-11-27 12:05  | Bertus       | Arnold2       | 3  | 0  |
		| 2019-11-27 12:23  | Ricky        | Eltjo         | 2  | 0  |
		| 2019-11-27 12:23  | Ricky        | Arnold2       | 2  | 0  |
		| 2019-11-27 13:34  | Hans         | Arnold1       | 2  | 1  |
		| 2019-11-27 13:37  | Bertus       | Ricky         | 2  | 0  |
		| 2019-11-27 14:05  | Irma         | Enrico        | 3  | 0  |
		| 2019-11-27 14:26  | Bertus       | Matthias      | 3  | 0  |
		| 2019-11-27 14:38  | Michel       | Arnold1       | 2  | 1  |
		| 2019-11-27 15:35  | Arnold2      | Eltjo         | 3  | 0  |
		| 2019-11-28 11:43  | Remco        | Jeroen        | 2  | 0  |
		| 2019-11-28 12:28  | Irma         | Ricky         | 3  | 0  |
		| 2019-11-28 12:33  | Irma         | Ricky         | 3  | 0  |
		| 2019-11-29 11:57  | Johannes     | Geale         | 3  | 0  |
		| 2019-11-29 11:57  | Johannes     | Arnold1       | 3  | 0  |
		| 2019-11-29 12:15  | Johannes     | Hans          | 3  | 2  |
		| 2019-11-29 13:26  | Arnold1      | Michel        | 2  | 1  |
		| 2019-11-29 13:40  | Arnold1      | Geale         | 2  | 1  |
		| 2019-11-29 15:06  | Michel       | Arnold1       | 2  | 1  |
		| 2019-11-29 15:18  | Remco        | Jeroen        | 2  | 0  |
		| 2019-11-29 20:01  | Michel       | Geale         | 2  | 1  |
		| 2019-11-29 20:02  | Michel       | Geale         | 3  | 0  |
		| 2019-12-02 12:14  | Irma         | Arnold2       | 3  | 0  |
		| 2019-12-02 12:29  | Irma         | Enrico        | 3  | 0  |
		| 2019-12-02 14:56  | Enrico       | Tjardo        | 3  | 0  |
		| 2019-12-02 15:05  | Irma         | Enrico        | 2  | 0  |
		| 2019-12-02 15:38  | Enrico       | Arnold2       | 3  | 0  |
		| 2019-12-03 09:30  | Michel       | Geale         | 3  | 0  |
		| 2019-12-03 11:30  | Geale        | Arnold1       | 3  | 0  |
		| 2019-12-03 11:30  | Michel       | Arnold1       | 2  | 1  |
		| 2019-12-03 12:02  | Irma         | Enrico        | 3  | 0  |
		| 2019-12-03 14:19  | Michel       | Geale         | 2  | 1  |
		| 2019-12-03 14:19  | Michel       | Arnold1       | 2  | 1  |
		| 2019-12-03 14:53  | Matthias     | Eduard        | 3  | 1  |
		| 2019-12-04 11:32  | Michel       | Geale         | 2  | 1  |
		| 2019-12-04 11:52  | Irma         | Matthias      | 3  | 0  |
		| 2019-12-04 12:02  | Tarik        | Tjardo        | 3  | 0  |
		| 2019-12-04 12:24  | Michel       | Geale         | 3  | 0  |
		| 2019-12-04 13:55  | Irma         | Arnold2       | 3  | 0  |
		| 2019-12-05 10:40  | Arnold1      | Tjardo        | 3  | 0  |
		| 2019-12-05 10:49  | Michel       | Arnold1       | 3  | 0  |
		| 2019-12-05 11:52  | Johannes     | Eduard        | 3  | 1  |
		| 2019-12-05 13:14  | Geale        | Arnold1       | 2  | 1  |
		| 2019-12-05 14:17  | Michel       | Geale         | 2  | 1  |
		| 2019-12-05 15:17  | Eduard       | Matthias      | 3  | 2  |
		| 2019-12-05 16:29  | Michel       | Geale         | 0  | 3  |
		| 2019-12-06 09:34  | Irma         | Matthias      | 3  | 0  |
		| 2019-12-06 13:52  | Irma         | Matthias      | 3  | 0  |
		| 2019-12-06 14:42  | Tarik        | Geale         | 1  | 2  |
		| 2019-12-09 12:08  | Hans         | Arnold1       | 3  | 0  |
		| 2019-12-09 12:10  | Michel       | Tarik         | 2  | 1  |
		| 2019-12-10 11:55  | Johannes     | Hans          | 3  | 1  |
	Then we have the following tafeltennis ranking with precision 0:
		| Ranking | NamePlayer | Points | WinPercentage |
		| 1       | Johannes   | 1782   | 94            |
		| 2       | Irma       | 1493   | 67            |
		| 3       | Hans       | 1493   | 78            |
		| 4       | Michel     | 1368   | 55            |
		| 5       | Bertus     | 1362   | 63            |
		| 6       | Remco      | 1299   | 62            |
		| 7       | Harro      | 1265   | 58            |
		| 8       | Geale      | 1259   | 48            |
		| 9       | Arnold1    | 1182   | 48            |
		| 10      | Arjen      | 1177   | 29            |
		| 11      | Ricky      | 1115   | 28            |
		| 12      | Eduard     | 1107   | 48            |
		| 13      | Jeroen     | 1090   | 20            |
		| 14      | Matthias   | 1073   | 23            |
		| 15      | Enrico     | 1022   | 11            |
		| 16      | Tarik      | 988    | 14            |
		| 17      | Eltjo      | 971    | 18            |
		| 18      | Arnold2    | 968    | 12            |
		| 19      | Tjardo     | 875    | 0             |