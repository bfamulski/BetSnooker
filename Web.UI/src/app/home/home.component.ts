import { Component, OnInit } from '@angular/core';
import { forkJoin, Subscription, interval } from 'rxjs';

import { Match, RoundInfo, RoundBets, EventBets, DashboardItem, User, UserStats, Event, MatchStatus, EMatchStatus } from '../_models';
import { SnookerFeedService, BetsService, AuthenticationService } from '../_services';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {

  currentEvent: Event;
  eventName: string;
  eventVenue: string;

  matches: Match[];
  ongoingMatches: Match[];
  users: User[];
  usersSorted: User[];
  eventRounds: RoundInfo[];
  currentRound: RoundInfo;
  roundBets: RoundBets[];
  eventBets: EventBets[];

  dashboardItems: DashboardItem[];
  usersScores: { [userId: string]: UserStats } = {};

  loading = false;
  error = '';

  private updateSubscription: Subscription;
  updateIntervalMs = 1000 * 60 * 10; // 10 minutes
  lastRefreshAt = '';

  version = environment.version;

  constructor(private snookerFeedService: SnookerFeedService,
              private betsService: BetsService,
              private authenticationService: AuthenticationService,
              private router: Router) { }

  ngOnInit() {
    this.loading = true;
    this.getCurrentEvent();
    this.loadData();
    this.lastRefreshAt = `${this.convertToLocalTime(new Date(Date.now()))}`;

    this.updateSubscription = interval(this.updateIntervalMs).subscribe(() => {
      this.loading = true;
      this.loadData();
      this.lastRefreshAt = `${this.convertToLocalTime(new Date(Date.now()))}`;
    });
  }

  getCurrentEvent() {
    this.snookerFeedService.getCurrentEvent(false).subscribe(event => {
      this.currentEvent = event;
      this.eventName = `${event.sponsor} ${event.name}`.trim();
      this.eventVenue = `${event.venue}, ${event.city} (${event.country})`;
    }, error => {
      this.error = error;
      this.loading = false;
    });
  }

  loadData() {
    const eventRoundsRequest = this.snookerFeedService.getEventRounds();
    const eventMatchesRequest = this.snookerFeedService.getEventMatches();
    const ongoingMatchesRequest = this.snookerFeedService.getOngoingMatches();
    const usersRequest = this.authenticationService.getUsers();
    const eventBetsRequest = this.betsService.getEventBets();
    const currentRoundRequest = this.snookerFeedService.getCurrentRoundInfo();

    forkJoin([eventRoundsRequest, currentRoundRequest, eventMatchesRequest, ongoingMatchesRequest, usersRequest, eventBetsRequest]).subscribe(results => {
      this.eventRounds = results[0];
      this.currentRound = results[1];
      this.matches = results[2];
      this.ongoingMatches = results[3];
      this.users = results[4];

      if (!this.users) {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
        return;
      }

      if (!this.eventRounds || !this.matches) {
        this.error = 'No rounds or matches available for this event';
        this.loading = false;
        return;
      }

      this.eventBets = results[5];

      if (this.eventBets) {
        this.users.forEach(user => {
          const userEventBets = this.eventBets.filter(b => b.userId === user.username)[0];
          if (userEventBets) {
            this.usersScores[user.username] = {
              isWinner: userEventBets.isWinner,
              matchesFinished: userEventBets.matchesFinished,
              eventScore: userEventBets.eventScore,
              correctWinners: userEventBets.correctWinners,
              exactScores: userEventBets.exactScores,
              correctWinnersAccuracy: this.formatAccuracyValue(userEventBets.correctWinnersAccuracy),
              exactScoresAccuracy: this.formatAccuracyValue(userEventBets.exactScoresAccuracy),
              averageError: this.formatAverageError(userEventBets.averageError)
            };

            user.eventScore = this.usersScores[user.username].eventScore;
          }
        });
      }

      this.usersSorted = this.users.slice(0);
      this.usersSorted.sort(this.compareUsers);

      this.eventRounds.forEach(round => {
        round.startDate = this.convertToLocalDate(round.actualStartDate);
      });

      this.dashboardItems = [];
      this.matches.sort(this.compareMatches);
      this.matches.forEach(match => {
        if (this.ongoingMatches) {
          this.ongoingMatches.forEach(ongoingMatch => {
            if (ongoingMatch.matchId === match.matchId) {
              match = ongoingMatch;
            }
          });
        }

        const dashboardItem = new DashboardItem({
          roundId: match.round,
          matchId: match.matchId,
          player1Id: match.player1Id,
          player1Name: match.player1Name,
          score1: match.walkover1 ? 'w/o' : (match.walkover2 ? '...' : match.score1.toString()),
          player2Id: match.player2Id,
          player2Name: match.player2Name,
          score2: match.walkover2 ? 'w/o' : (match.walkover1 ? '...' : match.score2.toString()),
          winnerId: match.winnerId,
          winnerName: match.winnerName,
          roundDistance: match.distance,
          status: this.formatStatus(match),
          userBets: {}
        });

        this.users.forEach(user => {
          dashboardItem.userBets[user.username] = { betScore1: "", betScore2: "", scoreValue: undefined };
          if (this.eventBets) {
            const userEventBets = this.eventBets.filter(b => b.userId === user.username)[0];
            if (userEventBets) {
              this.roundBets = userEventBets.roundBets;
              this.roundBets.forEach(userRoundBet => {
                const bet = userRoundBet.matchBets.find(b => b.matchId === match.matchId);
                if (bet) {
                  dashboardItem.userBets[user.username] = {
                    betScore1: bet.score1 != null ? bet.score1.toString() : (bet.betPlaced ? '?' : ''),
                    betScore2: bet.score2 != null ? bet.score2.toString() : (bet.betPlaced ? '?' : ''),
                    scoreValue: bet.scoreValue
                  };
                }
              });
            }
          }
        });

        this.dashboardItems.push(dashboardItem);
      });

      this.loading = false;
    }, error => {
      this.error = error;
      this.loading = false;
    });
  }

  formatAccuracyValue(accuracyValue: number) {
    return Number((accuracyValue * 100).toFixed(1)).toString();
  }

  formatAverageError(averageError: number) {
    if (averageError) {
      return averageError.toFixed(2);
    } else {
      return '-';
    }
  }

  formatStatus(match: Match) {
    if (match.endDate) {
      return new MatchStatus({ status: EMatchStatus.Finished });
    }

    if (match.startDate == null) {
      if (match.scheduledDate == null || (!match.player1Name && !match.player2Name)) {
        return new MatchStatus({ status: EMatchStatus.NotStarted });
      }

      let currentDateTime = new Date(new Date(Date.now()).toISOString());
      let matchScheduledDate = new Date(match.scheduledDate);

      if (matchScheduledDate != null && matchScheduledDate < currentDateTime) {
        return new MatchStatus({ status: EMatchStatus.ScheduledNotStarted });
      }

      return new MatchStatus({
        status: EMatchStatus.NotStarted,
        description: `${this.convertToLocalDateTime(match.scheduledDate)}`
      });
    }

    if (match.onBreak) {
      return new MatchStatus({
        status: EMatchStatus.NotStarted,
        description: `${this.convertToLocalDateTime(match.scheduledDate)} (BREAK)`
      });
    }

    return new MatchStatus({ status: EMatchStatus.Ongoing });
  }

  private convertToLocalDate(dateTime: Date) {
    if (dateTime) {
      const date = new Date(dateTime);
      return `${date.toISOString().slice(0, 10)}`;
    } else {
      return 'N/A';
    }
  }

  private convertToLocalDateTime(dateTime: Date) {
    if (dateTime != null) {
      const localDateTime = new Date(dateTime);
      const day = localDateTime.getDate().toString().padStart(2, '0');
      const month = (localDateTime.getMonth() + 1).toString().padStart(2, '0');
      const time = localDateTime.toLocaleTimeString('en-GB', {hour: 'numeric', minute: 'numeric'});
      return `${day}/${month} ${time}`;
    } else {
      return 'N/A';
    }
  }

  private convertToLocalTime(dateTime: Date) {
    if (dateTime) {
      return `${dateTime.toISOString().slice(0, 10)} ${dateTime.toLocaleTimeString('en-GB')}`;
    } else {
      return 'N/A';
    }
  }

  private compareUsers(user1: User, user2: User) {
    if (user1.eventScore > user2.eventScore) {
      return -1;
    }
    if (user1.eventScore < user2.eventScore) {
      return 1;
    }
    return 0;
  }

  private compareMatches(match1: Match, match2: Match) {
    if (match1.actualStartDate < match2.actualStartDate) {
      return -1;
    }
    if (match1.actualStartDate > match2.actualStartDate) {
      return 1;
    }
    return 0;
  }
}
