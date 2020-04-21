import { Component } from '@angular/core';
import { forkJoin } from 'rxjs';

import { Match, RoundInfo, RoundBets, EventBets, DashboardItem, User, UserStats } from '../_models';
import { SnookerFeedService, BetsService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent {

  private currentEvent: Event;
  private matches: Match[];
  private users: User[];
  private eventRounds: RoundInfo[];
  private roundBets: RoundBets[];
  private eventBets: EventBets[];

  private dashboardItems: DashboardItem[] = [];
  private usersScores: { [userId: string]: UserStats } = {};

  loading = false;
  error = '';

  constructor(private snookerFeedService: SnookerFeedService,
              private betsService: BetsService,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.loading = true;

    const currentEventRequest = this.snookerFeedService.getCurrentEvent();
    const eventRoundsRequest = this.snookerFeedService.getEventRounds();
    const eventMatchesRequest = this.snookerFeedService.getEventMatches();
    const usersRequest = this.authenticationService.getUsers();
    const eventBetsRequest = this.betsService.getEventBets();

    forkJoin([currentEventRequest, eventRoundsRequest, eventMatchesRequest, usersRequest, eventBetsRequest]).subscribe(results => {
      this.currentEvent = results[0];
      this.eventRounds = results[1];
      this.matches = results[2];
      if (!this.eventRounds || !this.matches) {
        this.error = 'No rounds or matches available for this event';
        this.loading = false;
        return;
      }

      this.users = results[3];
      this.eventBets = results[4];

      this.matches.forEach(match => {
        const dashboardItem = new DashboardItem({
          roundId: match.round,
          matchId: match.matchId,
          player1Id: match.player1Id,
          player1Name: match.player1Name,
          score1: match.score1,
          walkover1: match.walkover1,
          player2Id: match.player2Id,
          player2Name: match.player2Name,
          score2: match.score2,
          walkover2: match.walkover2,
          winnerId: match.winnerId,
          winnerName: match.winnerName,
          roundName: match.roundName, // TODO: is it needed?
          roundBestOf: match.distance * 2 - 1, // TODO: is it needed?
          userBets: {}
        });

        // TODO: optimize it
        if (this.users && this.eventBets) {
          this.users.forEach(user => {
            const userEventBets = this.eventBets.filter(b => b.userId === user.username)[0];
            if (userEventBets) {
              this.usersScores[user.username] = {
                matchesFinished: userEventBets.matchesFinished,
                eventScore: userEventBets.eventScore,
                correctWinners: userEventBets.correctWinners,
                exactScores: userEventBets.exactScores,
                correctWinnersAccuracy: Number((userEventBets.correctWinnersAccuracy * 100).toFixed(2)).toString(),
                exactScoresAccuracy: Number((userEventBets.exactScoresAccuracy * 100).toFixed(2)).toString()
              };

              this.roundBets = userEventBets.roundBets;
              this.roundBets.forEach(userRoundBet => {
                const bet = userRoundBet.matchBets.find(b => b.matchId === match.matchId);
                if (bet) {
                  dashboardItem.userBets[user.username] = { betScore1: bet.score1, betScore2: bet.score2, scoreValue: bet.scoreValue };
                }
              });
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
}
