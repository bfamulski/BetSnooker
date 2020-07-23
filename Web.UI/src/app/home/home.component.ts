import { Component } from '@angular/core';
import { forkJoin } from 'rxjs';

import { Match, RoundInfo, RoundBets, EventBets, DashboardItem, User, UserStats, Event } from '../_models';
import { SnookerFeedService, BetsService, AuthenticationService } from '../_services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent {

  currentEvent: Event;
  eventName: string;
  eventVenue: string;

  matches: Match[];
  users: User[];
  eventRounds: RoundInfo[];
  roundBets: RoundBets[];
  eventBets: EventBets[];

  dashboardItems: DashboardItem[] = [];
  usersScores: { [userId: string]: UserStats } = {};

  loading = false;
  error = '';

  constructor(private snookerFeedService: SnookerFeedService,
              private betsService: BetsService,
              private authenticationService: AuthenticationService,
              private router: Router) { }

  ngOnInit() {
    this.loading = true;

    this.snookerFeedService.getCurrentEvent().subscribe(event => {
      this.currentEvent = event;
      this.eventName = `${event.sponsor} ${event.name}`.trim();
      this.eventVenue = `${event.venue}, ${event.city} (${event.country})`;
    });

    const eventRoundsRequest = this.snookerFeedService.getEventRounds();
    const eventMatchesRequest = this.snookerFeedService.getEventMatches();
    const usersRequest = this.authenticationService.getUsers();
    const eventBetsRequest = this.betsService.getEventBets();

    forkJoin([eventRoundsRequest, eventMatchesRequest, usersRequest, eventBetsRequest]).subscribe(results => {
      this.eventRounds = results[0];
      this.matches = results[1];
      this.users = results[2];

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

      this.eventBets = results[3];

      this.eventRounds.forEach(round => {
        round.startDate = this.convertToLocalDate(round.actualStartDate);
      });

      this.matches.forEach(match => {
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

        if (this.users) {
          this.users.forEach(user => {
            dashboardItem.userBets[user.username] = { betScore1: null, betScore2: null, scoreValue: null };
            if (this.eventBets) {
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
                };

                this.roundBets = userEventBets.roundBets;
                this.roundBets.forEach(userRoundBet => {
                  const bet = userRoundBet.matchBets.find(b => b.matchId === match.matchId);
                  if (bet) {
                    dashboardItem.userBets[user.username] = { betScore1: bet.score1, betScore2: bet.score2, scoreValue: bet.scoreValue };
                  }
                });
              }
            }
          });
        }

        this.dashboardItems.push(dashboardItem);
      });

      this.loading = false;
    }, error => {
      this.error = error;
      this.loading = false;
    });
  }

  formatAccuracyValue(accuracyValue: number) {
    return Number((accuracyValue * 100).toFixed(2)).toString();
  }

  formatStatus(match: Match) {
    if (match.endDate) {
      return 'FINISHED';
    }

    if (match.startDate == null) {
      if (match.scheduledDate == null || (!match.player1Name && !match.player2Name)) {
        return 'TBD';
      }

      return this.convertToLocalDateTime(match.scheduledDate);
    }

    if (match.onBreak) {
      return `${this.convertToLocalDateTime(match.scheduledDate)} (BREAK)`;
    }

    return 'ONGOING';
  }

  convertToLocalDate(dateTime: Date) {
    const date = new Date(dateTime);
    return `${date.toISOString().slice(0, 10)}`;
  }

  convertToLocalDateTime(dateTime: Date) {
    const localDateTime = new Date(dateTime);
    return `${localDateTime.toISOString().slice(0, 10)} ${localDateTime.toLocaleTimeString('en-GB', {hour: 'numeric', minute: 'numeric'})}`;
  }
}
